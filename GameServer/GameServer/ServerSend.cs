namespace GameServer
{
    public class ServerSend
    {
        public static void Welcome(int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                SendTcpData(toClient, packet);
            }
        }

        private static void SendTcpData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClient].Tcp.SendData(packet);
        }

        private static void SendTcpDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                Server.Clients[i].Tcp.SendData(packet);
            }
        }
        
        private static void SendTcpDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                if (i == exceptClient) continue;

                Server.Clients[i].Tcp.SendData(packet);
            }
        }
    }
}