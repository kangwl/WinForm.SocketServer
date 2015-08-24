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

        private Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// 处理收到的客户端连接
        /// </summary>
        public Action<Socket> AcceptAction;

        /// <summary>
        /// 写日志
        /// </summary>
        public Action<string> LogAction; 
       
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
 
                Socket client = server.EndAccept(ac);
                AcceptAction(client);
                RecieveAsync(client);
                SendAsync(client, acceptMsg);
                AcceptAsync(acceptMsg);
            }, null);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public void SendAsync(Socket client,string message) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                int len = client.EndSend(ac);
            }, null);
        }

        /// <summary>
        /// 处理接收到的客户端信息
        /// </summary>
        public Action<Socket,string> RecieveAction;
        /// <summary>
        /// 处理删除客户端操作
        /// </summary>
        public Action<Socket> RemoveAction; 

        /// <summary>
        /// 接收客户端信息
        /// </summary>
        /// <param name="client"></param>
        public void RecieveAsync(Socket client) {
            byte[] bytes = new byte[2048]; 
            client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ac => {
                try {

                    int read = client.EndReceive(ac);
                    string message = System.Text.Encoding.UTF8.GetString(bytes, 0, read);
                    RecieveAction(client, message);
                    RecieveAsync(client);
                }
                catch (SocketException ex) {
                    if (ex.ErrorCode == 10054) {
                        //远程主机强迫关闭了一个连接
                        RemoveAction(client);
                    }
                }

            }, null);

        }

    }
}
