using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance;

    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;

    public int localID = 0;
    public TCP tcp;

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
    }

    public void ConnectToServer()
    {
        InitClientData();
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
                    // TODO: disconnect
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
                // TODO: disconnect
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
    }

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandle.Welcome }
        };

        Debug.Log("Initialized packets.");
    }
}

