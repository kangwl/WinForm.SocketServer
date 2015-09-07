using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinForm.SocketServer {
    public class SocketServer {

        public SocketServer(string host,int port) {
            Host = host;
            Port = port;
        }
        public string Host { get; set; }
        public int Port { get; set; }

        private readonly Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// 处理收到的客户端连接
        /// </summary>
        public Action<Socket> AcceptAction;

        /// <summary>
        /// 写日志
        /// </summary>
        public Action<string> LogAction;

        /// <summary>
        /// 处理异常
        /// </summary>
        public Action<Exception,Socket> ExceptionAction;

        /// <summary>
        /// 处理接收到的客户端信息
        /// </summary>
        public Action<Socket, string> RecieveAction;


        public Action<Socket> RemoveClientAction; 

        /// <summary>
        /// 监听客户端
        /// </summary>
        public void Listen() {
            server.Bind(new IPEndPoint(IPAddress.Parse(Host), Port));
            server.Listen(100);
            LogAction("监听中...");
            AcceptAsync();
        }

        public void AcceptAsync(string acceptMsg = "已连接 Server") {
            server.BeginAccept(ac => {
                Socket client = null;
                try {
                    client = server.EndAccept(ac);
                    AcceptAction(client);
                    RecieveAsync(client);
                    SendAsync(client, acceptMsg);
                    AcceptAsync(acceptMsg);
                }
                catch (Exception ex) {
                    ExceptionAction(ex, client);
                } 

            }, null);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public void SendAsync(Socket client, string message) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            try {

                client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                    try {
                        int len = client.EndSend(ac);
                    }
                    catch (Exception ex) {
                        ExceptionAction(ex, client);
                    }
                }, null);

            }
            catch (Exception ex) {
                ExceptionAction(ex, client);
            }
        }

        public void Send(Socket client, string message) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            try {

                client.Send(bytes, 0, bytes.Length, SocketFlags.None);
            }
            catch (Exception ex) {
                ExceptionAction(ex, client);
            }
        }

        public void SendAsync(Socket client, byte[] bytes) {
           // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                try {
                    int len = client.EndSend(ac);
                }
                catch (Exception ex) {
                    ExceptionAction(ex, client);
                }
            }, null);
        }



        /// <summary>
        /// 接收客户端信息
        /// </summary>
        /// <param name="client"></param>
        public void RecieveAsync(Socket client) {
            byte[] bytes = new byte[5*1024*1024];
            client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                try {

                    int read = client.EndReceive(ac);
                    if (read == 0) {
                        //客户端已关闭
                        RemoveClientAction(client);
                    }
                    else { 
                        string message = System.Text.Encoding.UTF8.GetString(bytes, 0, read);
                        RecieveAction(client, message);
                        RecieveAsync(client);
                    }
                }
                catch (Exception ex) {
                    ExceptionAction(ex, client);
                }

            }, null);
        }

    }
}
