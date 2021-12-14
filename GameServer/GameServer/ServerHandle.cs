using System;

namespace GameServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientID = packet.ReadInt();
            string username = packet.ReadString();
            
            Console.WriteLine($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
            if (fromClient != clientID)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientID})!");
            }
            
            //TODO: send player into game
        }

        public static void UdpTestReceived(int fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Console.WriteLine($"Received packet via UDP.Contains message: {message}");
        }
    }
}