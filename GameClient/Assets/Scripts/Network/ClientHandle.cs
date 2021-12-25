using System;
using System.Net;
using UnityEngine;
using Object = UnityEngine.Object;

public class ClientHandle
{
    public static Action<Vector3> OnCameraPositionChanged;

    public static Action<int, Vector3> OnBulletPositionChanged;
    public static Action<int, Vector3, Quaternion> OnBulletSpawn;
    public static Action<int> OnBulletDestroyed;
    public static Action<int, int, Vector3, Quaternion> OnBoosterSpawn;
    public static Action<int> OnBoosterUsed;

    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        Debug.Log($"Message from server: {message}");

        Client.Instance.SetID(id);
        ClientSend.WelcomeReceived();

        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Instance.InitPlayer(id, username, position, rotation);
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

    public static void PlayerDisconnected(Packet packet)
    {
        int playerId = packet.ReadInt();

        Object.Destroy(GameManager.Players[playerId].gameObject);
        GameManager.Players.Remove(playerId);
    }

    public static void CameraPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        OnCameraPositionChanged?.Invoke(position);
    }

    public static void BulletSpawn(Packet packet)
    {
        int bulletId = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        
        OnBulletSpawn?.Invoke(bulletId, position, rotation);
    }

    public static void BulletPosition(Packet packet)
    {
        int bulletId = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        
        OnBulletPositionChanged?.Invoke(bulletId, position);
    }

    public static void BulletDestroy(Packet packet)
    {
        int bulletId = packet.ReadInt();
        OnBulletDestroyed?.Invoke(bulletId);
    }

    public static void BoosterSpawn(Packet packet)
    {
        int boosterId = packet.ReadInt();
        int boosterType = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        
        OnBoosterSpawn?.Invoke(boosterId, boosterType, position, rotation);
    }

    public static void BoosterUse(Packet packet)
    {
        int boosterId = packet.ReadInt();

        OnBoosterUsed?.Invoke(boosterId);
    }
}