using Boosters;
using UnityEngine;

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

    #region UDP

    public static void SpawnPlayer(int toClient, PlayerModel playerModel)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerSpawn))
        {
            packet.Write(playerModel.Id);
            packet.Write(playerModel.Username);
            packet.Write(playerModel.transform.position);
            packet.Write(playerModel.transform.rotation);

            SendTcpData(toClient, packet);
        }
    }

    private static void SendUdpData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.Clients[toClient].Udp.SendData(packet);
    }

    private static void SendUdpDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.Clients[i].Udp.SendData(packet);
        }
    }

    private static void SendUdpDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i == exceptClient) continue;

            Server.Clients[i].Udp.SendData(packet);
        }
    }

    #endregion

    #region TCP

    private static void SendTcpData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.Clients[toClient].Tcp.SendData(packet);
    }

    private static void SendTcpDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.Clients[i].Tcp.SendData(packet);
        }
    }

    private static void SendTcpDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i == exceptClient) continue;

            Server.Clients[i].Tcp.SendData(packet);
        }
    }

    #endregion

    public static void PlayerPosition(PlayerMovementController playerMovementController)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
        {
            packet.Write(playerMovementController.Id);
            packet.Write(playerMovementController.transform.position);

            SendUdpDataToAll(packet);
        }
    }

    public static void PlayerRotation(PlayerMovementController playerMovementController)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
        {
            packet.Write(playerMovementController.Id);
            packet.Write(playerMovementController.transform.rotation);

            SendUdpDataToAll(packet);
        }
    }

    public static void PlayerDisconnected(int playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerDisconnected))
        {
            packet.Write(playerId);
            SendTcpDataToAll(packet);
        }
    }

    public static void CameraPosition(int playerId, Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.CameraPosition))
        {
            packet.Write(playerId);
            packet.Write(position);
            SendUdpData(playerId, packet);
        }
    }

    public static void SpawnBullet(int bulletId, Vector3 position, Quaternion rotation)
    {
        using (Packet packet = new Packet((int)ServerPackets.BulletSpawn))
        {
            packet.Write(bulletId);
            packet.Write(position);
            packet.Write(rotation);
            SendTcpDataToAll(packet);
        }
    }

    public static void BulletPosition(int bulletId, Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.BulletPosition))
        {
            packet.Write(bulletId);
            packet.Write(position);
            SendUdpDataToAll(packet);
        }
    }

    public static void BulletDestroy(int bulletId)
    {
        using (Packet packet = new Packet((int)ServerPackets.BulletDestroy))
        {
            packet.Write(bulletId);
            SendTcpDataToAll(packet);
        }
    }

    public static void BoosterSpawn(int boosterId, BoosterType boosterType, Vector3 position, Quaternion rotation)
    {
        using (Packet packet = new Packet((int)ServerPackets.BoosterSpawn))
        {
            packet.Write(boosterId);
            packet.Write((int)boosterType);
            packet.Write(position);
            packet.Write(rotation);
            
            SendTcpDataToAll(packet);
        }
    }

    public static void BoosterUse(int boosterId)
    {
        using (Packet packet = new Packet((int)ServerPackets.BoosterUse))
        {
            packet.Write(boosterId);
            
            SendTcpDataToAll(packet);
        }
    }
}