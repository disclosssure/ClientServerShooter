using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static void PlayerMovement(bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            packet.Write(inputs.Length);
            inputs.ToList().ForEach(i => packet.Write(i));
            packet.Write(GameManager.Players[Client.Instance.localID].transform.rotation);

            SendUdpData(packet);
        }
    }
}