using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private int _port = 26950;
    [SerializeField] private int _id;

    public int Id => _id;
    public string IP => _ip;
    public int Port => _port;
    
    public TCP Tcp { get; private set; }
    public UDP Udp { get; private set; }

    private bool _isConnected;
    private static int dataBufferSize = 4096;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> _packetHandlers;

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
        Tcp = new TCP();
        Udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitClientData();

        _isConnected = true;
        Tcp.Connect();
    }

    public void SetID(int id)
    {
        _id = id;
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream _stream;
        private Packet _receivedData;
        private byte[] _receiveBuffer;
        
        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            _receiveBuffer = new byte[dataBufferSize];

            socket.BeginConnect(Instance._ip, Instance._port, ConnectCallback, socket);
            
            void ConnectCallback(IAsyncResult result)
            {
                socket.EndConnect(result);

                if (!socket.Connected) return;

                _stream = socket.GetStream();

                _receivedData = new Packet();

                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            
                void ReceiveCallback(IAsyncResult result)
                {
                    try
                    {
                        int _byteLenght = _stream.EndRead(result);
                        if (_byteLenght <= 0)
                        {
                            Instance.Disconnect();
                            return;
                        }

                        byte[] _data = new byte[_byteLenght];
                        Array.Copy(_receiveBuffer, _data, _byteLenght);

                        _receivedData.Reset(HandleData(_data));
                        _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error receiving TCP data: {e}");
                        Disconnect();
                    }
                }
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(socket != null)
                {
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Error sending data to server via TCP: {e}");
                throw;
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLenght = 0;

            _receivedData.SetBytes(data);

            if (_receivedData.UnreadLength() >= 4)
            {
                packetLenght = _receivedData.ReadInt();

                if (packetLenght <= 0)
                {
                    return true;
                }
            }

            while (packetLenght > 0 && packetLenght <= _receivedData.UnreadLength())
            {
                byte[] packetBytes = _receivedData.ReadBytes(packetLenght);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        _packetHandlers[packetID](packet);
                    }
                });

                packetLenght = 0;
                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLenght = _receivedData.ReadInt();

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

            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(Instance._ip), Instance._port);
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
                packet.InsertInt(Instance.Id);

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
                    _packetHandlers[packetId](packet);
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
        _packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandle.Welcome },
            { (int)ServerPackets.PlayerSpawn, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.PlayerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.PlayerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.PlayerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.CameraPosition, ClientHandle.CameraPosition },
        };

        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            Tcp.socket.Close();
            Udp.socket.Close();
            
            Debug.Log("Disconnected from server.");
        }
    }
}

