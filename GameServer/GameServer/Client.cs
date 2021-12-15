﻿using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
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
            private Packet receivedData;
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

                receivedData = new Packet();
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
                            // TODO: disconnect
                            return;
                        }

                        byte[] data = new byte[byteLenght];
                        Array.Copy(_receiveBuffer, data, byteLenght);
                        
                        receivedData.Reset(HandleData(data));
                        _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error receiving TCP data: {e}");
                        // TODO: disconnect
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
                            Server.packetHandlers[packetID](_id, packet);
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
                        Server.packetHandlers[packetId](_id, packet);
                    }
                });
            }
        }

        public void SendIntoGame(string playerName)
        {
            Player = new Player(Id, playerName, Vector3.Zero);

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
    }
}