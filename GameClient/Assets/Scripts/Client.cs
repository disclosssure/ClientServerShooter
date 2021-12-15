using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;

    public int localID = 0;
    public TCP tcp;
    public UDP udp;

    private bool _isConnected;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitClientData();

        _isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;


        public void Connect()
        {
            socket = new TcpClient()
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];

            socket.BeginConnect(Instance.ip, Instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected) return;

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Error sending data to server via TCP: {e}");
                throw;
            }
        } 

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int _byteLenght = stream.EndRead(result);
                if (_byteLenght <= 0)
                {
                    Instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLenght];
                Array.Copy(receiveBuffer, _data, _byteLenght);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receiving TCP data: {e}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLenght = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLenght = receivedData.ReadInt();

                if (packetLenght <= 0)
                {
                    return true;
                }
            }

            while (packetLenght > 0 && packetLenght <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLenght);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet);
                    }
                });

                packetLenght = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    packetLenght = receivedData.ReadInt();

                    if (packetLenght <= 0)
                    {
                        return true;
                    }
                }
            }

            if(packetLenght <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);
            socket.Connect(endPoint);

            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
            
            void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    byte[] data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(Instance.localID);

                socket?.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to server via UDP: {e}");
                throw;
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLenght = packet.ReadInt();
                data = packet.ReadBytes(packetLenght);
            }
            
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandle.Welcome },
            { (int)ServerPackets.PlayerSpawn, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.PlayerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.PlayerRotation, ClientHandle.PlayerRotation },
        };

        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
            
            Debug.Log("Disconnected from server.");
        }
    }
}

