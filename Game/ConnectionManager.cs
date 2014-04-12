using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Chibre_Server
{
    class ConnectionManager
    {
        private static ConnectionManager instance = null;
        private bool _acceptConnections;
        private StreamSocketListener _socketListener;
        private bool _receiveData = false;
        private StreamSocket socket;

        private ConnectionManager()
        {
            _acceptConnections = false;
        }

        public static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConnectionManager();

                return instance;
            }
        }

        public async void SetAcceptConnections(bool value)
        {
            if(_acceptConnections ^ value) // Value changed
            {
                if(value == true)
                {
                    _socketListener = new StreamSocketListener();
                    _socketListener.ConnectionReceived += OnConnectionReceived;
                    await _socketListener.BindEndpointAsync(null, "24273");
                }
                else
                {
                    _socketListener.Dispose();
                }

            }

            _acceptConnections = value;
        }

        public async void ReceiveLoop()
        {
            if (_receiveData == true)
                return;

            _receiveData = true;
            while(_receiveData)
            {
                DataReader reader = new DataReader(socket.InputStream);
                // Set inputstream options so that we don't have to know the data size
                reader.InputStreamOptions = InputStreamOptions.Partial;
                var data = await reader.LoadAsync(reader.UnconsumedBufferLength);
                Debug.WriteLine(data.GetType());
                
            }
        }

        void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Someone connected");

            socket = args.Socket;
            ReceiveLoop();
        }
    }
}
