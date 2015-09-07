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

 

        public Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void ConnectServer() {

            client.BeginConnect(new IPEndPoint(IPAddress.Parse(Host), Port), ac => {

                client.EndConnect(ac);
                RecieveAsync();

            }, null);

        }

        public void SendAsync(string message) {
            if (!client.Connected) {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(new IPEndPoint(IPAddress.Parse(Host), Port), ac => {
                    try {
                        client.EndConnect(ac);
                    }
                    catch (Exception ex) {
                        ExceptionAction(ex);
                    }

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
            try { 
                client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                    try {

                        int read = client.EndSend(ac);
                    }
                    catch (Exception ex) {
                        ExceptionAction(ex);
                    }

                }, null);
            }
            catch (Exception ex) {
                ExceptionAction(ex);
            }
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        public Action<string> recordLog;

        /// <summary>
        /// 处理发生的异常
        /// </summary>
        public Action<Exception> ExceptionAction;

        public void RecieveAsync() {

            byte[] bytes = new byte[2048];
            client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                try {
                    int len = client.EndReceive(ac);
                    string message = System.Text.Encoding.UTF8.GetString(bytes, 0, len);
                    recordLog(message);
                    RecieveAsync();
                }
                catch (Exception ex) {
                    ExceptionAction(ex);
                }

            }, null);
        }



    }
}
