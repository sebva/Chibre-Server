﻿using Chibre_Server.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking;
using Windows.Networking.Connectivity;
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
        private HashSet<Guid> clients;

        public interface ClientConnectionListener
        {
            void OnClientConnected(int number);
        }

        private ConnectionManager()
        {
            clients = new HashSet<Guid>();
            _acceptConnections = false;
        }

        public ClientConnectionListener ClientListener
        {
            private get;
            set;
        }

        public static ConnectionManager Instance
        {
            get
            {
                if(instance == null)
                    instance = new ConnectionManager();

                return instance;
            }
        }

        public async void SetAcceptConnections(bool value)
        {
            if (_acceptConnections ^ value) // Value changed
            {
                if (value == true)
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

        private void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Someone connected");

            StreamSocket socket = args.Socket;
            Connection connection = new Connection(socket);
            connection.IsReceiving = true;
        }

        public void OnHelloReceived(Guid uuid)
        {
            Debug.WriteLine(uuid.ToString());
            if(clients.Add(uuid))
                ClientListener.OnClientConnected(clients.Count);
        }

        public static string[] GetIPs()
        {
            var ips = NetworkInformation.GetHostNames();
            List<string> ipstrings = new List<string>(ips.Count);
            foreach(HostName host in ips)
            {
                if(host.IPInformation != null)
                    ipstrings.Add(host.DisplayName);
            }
            return ipstrings.ToArray();
        }
    }
}
