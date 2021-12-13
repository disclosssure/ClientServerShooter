using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public enum ServerPackets
    {
        Welcome = 0,
    }

    public enum ClientPackets
    {
        WelcomeReceived = 0,
    }

    public class Packet : IDisposable
    {
        private List<byte> _buffer;
        private byte[] _readableBuffer;
        private int _readPos;
        
        private bool _disposed;

        public Packet()
        {
            _buffer = new List<byte>();
            _readPos = 0;
        }

        public Packet(int id)
        {
            _buffer = new List<byte>();
            _readPos = 0;

            Write(id);
        }
        
        public Packet(byte[] data)
        {
            _buffer = new List<byte>();
            _readPos = 0;

            SetBytes(data);
        }

        #region Functions
        public void SetBytes(byte[] data)
        {
            Write(data);
            _readableBuffer = _buffer.ToArray();
        }

        public void WriteLength()
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }

        public void InsertInt(int value)
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(value));
        }
        
        public byte[] ToArray()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }
        
        public int Length()
        {
            return _buffer.Count;
        }

        public int UnreadLength()
        {
            return Length() - _readPos;
        }

        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                _buffer.Clear();
                _readableBuffer = null;
                _readPos = 0;
            }
            else
            {
                _readPos -= 4;
            }
        }
        #endregion

        #region Write Data
        public void Write(byte value)
        {
            _buffer.Add(value);
        }
        
        public void Write(byte[] value)
        {
            _buffer.AddRange(value);
        }
        
        public void Write(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        
        public void Write(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        
        public void Write(long _value)
        {
            _buffer.AddRange(BitConverter.GetBytes(_value));
        }
        
        public void Write(float _value)
        {
            _buffer.AddRange(BitConverter.GetBytes(_value));
        }
        
        public void Write(bool _value)
        {
            _buffer.AddRange(BitConverter.GetBytes(_value));
        }
        
        public void Write(string _value)
        {
            Write(_value.Length);
            _buffer.AddRange(Encoding.ASCII.GetBytes(_value));
        }
        #endregion

        #region Read Data
        public byte ReadByte(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                byte value = _readableBuffer[_readPos];
                if (moveReadPos)
                {
                    _readPos += 1;
                }
                return value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                byte[] value = _buffer.GetRange(_readPos, length).ToArray();
                if (moveReadPos)
                {
                    _readPos += length;
                }
                return value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                short value = BitConverter.ToInt16(_readableBuffer, _readPos);
                if (moveReadPos)
                {
                    _readPos += 2;
                }
                return value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                int value = BitConverter.ToInt32(_readableBuffer, _readPos);
                if (moveReadPos)
                {
                    _readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                long value = BitConverter.ToInt64(_readableBuffer, _readPos);
                if (moveReadPos)
                {
                    _readPos += 8;
                }
                return value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                float value = BitConverter.ToSingle(_readableBuffer, _readPos);
                if (moveReadPos)
                {
                    _readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public bool ReadBool(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                bool value = BitConverter.ToBoolean(_readableBuffer, _readPos);
                if (moveReadPos)
                {
                    _readPos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(_readableBuffer, _readPos, length);
                if (moveReadPos && value.Length > 0)
                {
                    _readPos += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        #endregion
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _buffer = null;
                    _readableBuffer = null;
                    _readPos = 0;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}