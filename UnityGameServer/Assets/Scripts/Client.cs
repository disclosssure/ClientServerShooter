using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;
        public int Id { get; private set; }
        public Player Player { get; private set; }
        public TCP Tcp { get; private set; }
        public UDP Udp { get; private set; }

        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(Id);
            Udp = new UDP(Id);
        }
        
        public class TCP
        {
            public TcpClient Socket { get; private set; }

            private readonly int _id;

            private NetworkStream _stream;
            private Packet _receivedData;
            private byte[] _receiveBuffer;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                _stream = socket.GetStream();

                _receivedData = new Packet();
                _receiveBuffer = new byte[dataBufferSize];

                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                
                ServerSend.Welcome(_id, "Welcome the server!");
                
                void ReceiveCallback(IAsyncResult result)
                {
                    try
                    {
                        int byteLenght = _stream.EndRead(result);
                        if (byteLenght <= 0)
                        {
                            Server.Clients[_id].Disconnect();
                            return;
                        }

                        byte[] data = new byte[byteLenght];
                        Array.Copy(_receiveBuffer, data, byteLenght);
                        
                        _receivedData.Reset(HandleData(data));
                        _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error receiving TCP data: {e}");
                        Server.Clients[_id].Disconnect();
                    }
                }
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending data to player {_id} via TCP: {e}");
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
                            int packetId = packet.ReadInt();
                            Server.PacketHandlers[packetId](_id, packet);
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

            public void Disconnect()
            {
                Socket.Close();
                _stream = null;
                _receivedData = null;
                _receiveBuffer = null;
                Socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint EndPoint;

            private int _id;

            public UDP(int id)
            {
                _id = id;
            }

            public void Connect(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
            }

            public void SendData(Packet packet)
            {
                Server.SendUdpData(EndPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLenght = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLenght);
                
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.PacketHandlers[packetId](_id, packet);
                    }
                });
            }
            
            public void Disconnect()
            {
                EndPoint = null;
            }
        }

        public void SendIntoGame(string playerName)
        {
            Player = NetworkManager.Instance.InstantiatePlayer();
            Player.Init(Id, playerName);

            foreach (var client in Server.Clients.Values)
            {
                if (client.Player != null)
                {
                    if (client.Id != Id)
                    {
                        ServerSend.SpawnPlayer(Id, client.Player);
                    }
                }
            }
            
            foreach (var client in Server.Clients.Values)
            {
                if (client.Player != null)
                {
                    ServerSend.SpawnPlayer(client.Id, Player);
                }
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

            ThreadManager.ExecuteOnMainThread(() =>
            {
                UnityEngine.Object.Destroy(Player.gameObject);
                Player = null;
            });
            
            Tcp.Disconnect();
            Udp.Disconnect();
        }
}
