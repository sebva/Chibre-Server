using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Chibre_Server.Game
{
    class Connection
    {
        private StreamSocket socket;
        private bool _receiveData = false;

        public Connection(StreamSocket socket)
        {
            this.socket = socket;
        }

        public Player Player
        {
            get;
            set;
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

                try
                {
                    ProcessData(received);
                }
                catch(NotImplementedException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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

            JsonObject jsonRes;
            bool ok = JsonObject.TryParse(received, out jsonRes);
            if (ok)
            {
                // Rails-like automatic method matching
                TypeInfo t = typeof(Protocol).GetTypeInfo();
                string methodName = UnderscoreToCamel(jsonRes.GetNamedString("action"));
                MethodInfo m = t.GetDeclaredMethod(methodName);
                if (m != null)
                    m.Invoke(null, new object[] { jsonRes, this });
                else
                    throw new NotImplementedException("The method " + methodName + " does not exist in Protocol.cs");
            }
            else
                Debug.WriteLine("Invalid JSON received");
        }

        public static string UnderscoreToCamel(string underscore)
        {
            StringBuilder result = new StringBuilder(underscore);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (result[i - 1] == '_')
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.Replace("_", string.Empty).ToString();
        }
    }
}
