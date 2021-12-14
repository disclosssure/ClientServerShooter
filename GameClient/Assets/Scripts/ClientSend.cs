using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTcpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.tcp.SendData(packet);
    }

    private static void SendUdpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.udp.SendData(packet);
    }

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.localID);
            packet.Write(UIManager.Instance.usernamField.text);

            SendTcpData(packet);
        }
    }

    public static void UdpTestReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.UdpTestReceived))
        {
            packet.Write("Received a UDP packet.");
            SendUdpData(packet);
        }
    }
}
