using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PROGRAMMATION_SYST_ME.Model;
using System.ComponentModel;
using PROGRAMMATION_SYST_ME.View;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PROGRAMMATION_SYST_ME.ViewModel
{
    internal class SaveWindowViewModel
    {
        private Thread tSocket;
        public ServSocket serv = new();
        public Socket socket1;
        public Socket Connected { get; set; }
        public bool StopConnexion { get; set; }
        public SaveWindowViewModel()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Loopback, 11111);
            socket1 = serv.Connect();
            socket1.Bind(localEndPoint);
            Connected = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tSocket = new Thread(() =>
            {
                while (!StopConnexion)
                {
                    Connected = serv.AllowConnexion(socket1);
                }
            });
            tSocket.Start();
        }
        public void SendInfoToSocket(List<Item> info)
        {
            var toSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<List<Item>>(info));
            serv.SendToNetwork(Connected, toSend);
        }
    }
}
