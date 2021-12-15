using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
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

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        
        GameManager.Instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameManager.Players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Players[id].transform.rotation = rotation;
    }
}
