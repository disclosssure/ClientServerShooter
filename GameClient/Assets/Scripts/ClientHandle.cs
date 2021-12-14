using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message from server: {message}");
        Client.Instance.localID = id;
        ClientSend.WelcomeReceived();
        
        Client.Instance.udp.Connect(((IPEndPoint)Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UdpTest(Packet packet)
    {
        string message = packet.ReadString();
        Debug.Log($"Received packet via UDP. Contains message: {message}");
        ClientSend.UdpTestReceived();
    }
}
