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
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener _tcpListener { get; set; }

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
            
            Console.WriteLine($"Server started on {Port}.");
            
            void TcpConnectCallback(IAsyncResult result)
            {
                TcpClient client = _tcpListener.EndAcceptTcpClient(result);
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
                Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

                for (int i = 0; i <= MaxPlayers; i++)
                {
                    if (Clients[i].Tcp.Socket == null)
                    {
                        Clients[i].Tcp.Connect(client);
                        return;
                    }
                }

                Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server is full!");
            }
        }

        private static void InitServerData()
        {
            for (int i = 0; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived }
            };
            
            Console.WriteLine("Initialize packets.");
        }
    }
}