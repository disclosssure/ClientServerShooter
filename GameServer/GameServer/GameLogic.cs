namespace GameServer
{
    public class GameLogic
    {
        public static void Update()
        {
            foreach (var client in Server.Clients.Values)
            {
                client.Player?.Update();
            }

            ThreadManager.UpdateMain();
        }
    }
}