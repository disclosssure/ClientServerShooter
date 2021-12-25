using System;
using UnityEngine;

public class ServerHandle
{
    public static Action<int, string> OnWelcomeReceived;
    public static Action<int, bool[], Vector3> OnPlayerInputChanged;
    public static Action<Vector3, Quaternion> OnPlayerShoot;

    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
        if (fromClient != clientId)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
        }

        OnWelcomeReceived?.Invoke(fromClient, username);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        var mousePosition = packet.ReadVector3();
        
        OnPlayerInputChanged?.Invoke(fromClient, inputs, mousePosition);
    }

    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        
        OnPlayerShoot?.Invoke(position, rotation);
    }
}
