using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PROGRAMMATION_SYST_ME.View;

namespace PROGRAMMATION_SYST_ME.Model
{
    internal class ServSocket
    {
        public ServSocket()
        {

        }
        public Socket Connect()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Socket AllowConnexion(Socket listener)
        {
            listener.Listen(1);
            Socket connect = null;
            try
            {
                connect = listener.Accept();
            }
            catch (Exception ex)
            {

            }
            return connect;
        }
        public void SendToNetwork(Socket client, byte[] toSend)
        {
            try
            {
                client.SendAsync(toSend);
            }
            catch
            {

            }
        }
        public void Disconnect(Socket client)
        {
            Console.WriteLine("Connexion ended");
            client.Close();
        }
    }
}
