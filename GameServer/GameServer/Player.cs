using System;
using System.Numerics;

namespace GameServer
{
    public class Player
    {
        public int Id { get; private set; }
        public string Username { get; private set; }

        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        private readonly float _moveSpeed = 5f / Constants.k_ticksPerSecond;
        private bool[] _inputs; 

        public Player(int id, string username, Vector3 spawnPosition)
        {
            Id = id;
            Username = username;
            Position = spawnPosition;
            Rotation = Quaternion.Identity;

            _inputs = new bool[4];
        }

        public void Update()
        {
            Vector2 direction = Vector2.Zero;

            if (_inputs[0])     // W
            {
                direction.Y += 1;
            }
            if (_inputs[1])     // A
            {
                direction.X += 1;
            }
            if (_inputs[2])     // S
            {
                direction.Y -= 1;
            }
            if (_inputs[3])     // D
            {
                direction.X -= 1;
            }

            Move(direction);
        }

        private void Move(Vector2 direction)
        {
            Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), Rotation);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

            Vector3 moveDirection = right * direction.X + forward * direction.Y;
            Position += moveDirection * _moveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] inputs, Quaternion rotation)
        {
            _inputs = inputs;
            Rotation = rotation;
        }
    }
}