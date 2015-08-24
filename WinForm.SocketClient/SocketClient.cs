using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm.SocketClient {
    public class SocketClient {
        public SocketClient(string host,int port) {
            Host = host;
            Port = port;
        }
        public string Host { get; set; }
        public int Port { get; set; }

 

        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void ConnectServer() {

            client.BeginConnect(new IPEndPoint(IPAddress.Parse(Host), Port), ac => {

                client.EndConnect(ac);
                RecieveAsync();

            }, null);

        }

        public void SendAsync(string message) {
            if (!client.Connected) {
                client.BeginConnect(new IPEndPoint(IPAddress.Parse(Host), Port), ac => {
                    client.EndConnect(ac);
 
                    //send msg
                    SendMessageAsync(message);
                }, null);
            }
            else {
                //send msg
                SendMessageAsync(message);
            }

        }

        private void SendMessageAsync(string message) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

            client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                int read = client.EndSend(ac);

            }, null);
        }

        public Action<string> recordLog;

        public void RecieveAsync() {
            byte[] bytes = new byte[2048];
            client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ac => {

                int len = client.EndReceive(ac);
                string message = System.Text.Encoding.UTF8.GetString(bytes, 0, len);
                recordLog(message);
                RecieveAsync();

            }, null);
        }



    }
}
