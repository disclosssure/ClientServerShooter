using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class Server
    {
        public static int Port { get; private set; }
        
        public static int MaxPlayers { get; private set; }
        public static Dictionary<int, Client> Clients = new();

        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener _tcpListener { get; set; }
        private static UdpClient _udpListener { get; set; }

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            InitServerData();
            InitTcpListener();
        }

        private static void InitTcpListener()
        {
            Console.WriteLine("Starting server...");

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

            _udpListener = new UdpClient(Port);
            _udpListener.BeginReceive(UdpReceiveCallback, null);
            
            Console.WriteLine($"Server started on {Port}.");
            
            void TcpConnectCallback(IAsyncResult result)
            {
                TcpClient client = _tcpListener.EndAcceptTcpClient(result);
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
                Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

                for (int i = 1; i <= MaxPlayers; i++)
                {
                    if (Clients[i].Tcp.Socket == null)
                    {
                        Clients[i].Tcp.Connect(client);
                        return;
                    }
                }

                Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server is full!");
            }
            
            void UdpReceiveCallback(IAsyncResult result)
            {
                try
                {
                    IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
                    _udpListener.BeginReceive(UdpReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        return;
                    }

                    using (Packet packet = new Packet(data))
                    {
                        int clientId = packet.ReadInt();

                        if (clientId == 0)
                        {
                            return;
                        }

                        if (Clients[clientId].Udp.EndPoint == null)
                        {
                            Clients[clientId].Udp.Connect(clientEndPoint);
                            return;
                        }

                        if (Clients[clientId].Udp.EndPoint.ToString() == clientEndPoint.ToString())
                        {
                            Clients[clientId].Udp.HandleData(packet);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving UDP data: {e}");
                    throw;
                }
            }
        }

        public static void SendUdpData(IPEndPoint endPoint, Packet packet)
        {
            try
            {
                if (endPoint != null)
                {
                    _udpListener.BeginSend(packet.ToArray(), packet.Length(), endPoint, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending data to {endPoint} via UDP: {e}");
                throw;
            }
        }

        private static void InitServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.PlayerMovement, ServerHandle.PlayerMovement },
            };
            
            Console.WriteLine("Initialize packets.");
        }
    }
}