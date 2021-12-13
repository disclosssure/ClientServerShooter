using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        public int Id;
        public TCP Tcp;

        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(Id);
        }
        
        public class TCP
        {
            public TcpClient socket;

            private readonly int _id;

            private NetworkStream _stream;
            private byte[] _receiveBuffer;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                this.socket = socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                _stream = socket.GetStream();

                _receiveBuffer = new byte[dataBufferSize];

                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                
                //TODO: send welcome packet
                
                void ReceiveCallback(IAsyncResult result)
                {
                    try
                    {
                        int _byteLenght = _stream.EndRead(result);
                        if (_byteLenght <= 0)
                        {
                            // TODO: disconnect
                            return;
                        }

                        byte[] _data = new byte[_byteLenght];
                        Array.Copy(_receiveBuffer, _data, _byteLenght);
                        
                        //TODO: Handle data
                        _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error receiving TCP data: {e}");
                        // TODO: disconnect
                    }
                }
            }
        }
    }
}