using System;
using System.Linq;
using UnityEngine;

public class ClientSend
{
    private static void SendTcpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Tcp.SendData(packet);
    }

    private static void SendUdpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Udp.SendData(packet);
    }

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.Id);
            packet.Write(UIManager.Instance.Username);

            SendTcpData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs, Vector3 mousePosition)
    {
        using (Packet packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            packet.Write(inputs.Length);
            inputs.ToList().ForEach(i => packet.Write(i));
            packet.Write(mousePosition);

            SendUdpData(packet);
        }
    }
}