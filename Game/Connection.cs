using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Chibre_Server.Game
{
    class Connection
    {
        private StreamSocket socket;
        private Player player;
        private bool _receiveData = false;

        public Connection(StreamSocket socket)
        {
            this.socket = socket;
        }

        public void SetPlayer(ref Player player)
        {
            this.player = player;
        }

        public bool IsReceiving
        {
            set
            {
                if (_receiveData ^ value) // Value changed
                {
                    if (value == true)
                    {
                        ReceiveLoop();
                    }
                }

                _receiveData = value;
            }
            get { return _receiveData; }
        }

        public async void ReceiveLoop()
        {
            if (_receiveData == true)
                return;

            _receiveData = true;

            while (_receiveData)
            {
                DataReader reader = new DataReader(socket.InputStream);
                // Set inputstream options so that we don't have to know the data size
                reader.InputStreamOptions = InputStreamOptions.Partial;

                uint count = await reader.LoadAsync(sizeof(int));
                if (count < sizeof(int))
                {
                    Debug.WriteLine("Socket closed");
                    return;
                }
                int payloadLength = reader.ReadInt32();

                count = await reader.LoadAsync((uint)payloadLength);
                if (count < sizeof(int))
                {
                    Debug.WriteLine("Socket closed");
                    return;
                }

                string received = reader.ReadString((uint)payloadLength);

                ProcessData(received);
            }
        }

        public async void SendPayload(string payload)
        {
            int length = payload.Length;

            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteInt32(length);
            writer.WriteString(payload);
            await writer.StoreAsync();
        }

        private void ProcessData(string received)
        {
            Debug.WriteLine("Data received: " + received);
            SendPayload(received);
        }
    }
}
