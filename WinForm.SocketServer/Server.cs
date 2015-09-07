using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm.SocketServer {
    public partial class Server : Form {

        #region host, port

        private static readonly string _host = "127.0.0.1";
        private static int _port = 1314;

        #endregion

        /// <summary>
        /// 保存客户端连接
        /// </summary>
        private readonly Dictionary<string, Socket> dicSockets = new Dictionary<string, Socket>(); 
 
        public Server() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            //delegate
            _socketServer.ExceptionAction = DealException;
            _socketServer.LogAction = AppendLog;
            _socketServer.RemoveClientAction = RemoveClient;
        }
        //
        private readonly SocketServer _socketServer = new SocketServer(_host, _port);

        private void Server_Load(object sender, EventArgs e) {
            _socketServer.Listen();//

            _socketServer.AcceptAction = client => {
                AddClient(client); 
                AppendLog(client.RemoteEndPoint.ToString() + " 已连接");
            };

            _socketServer.RecieveAction = (client, recieveMsg) => AppendLog(client.RemoteEndPoint + ": " + recieveMsg);
       
        }

        private void RemoveClient(Socket client) {
            if (client == null) return;
            string clientEp = client.RemoteEndPoint.ToString();
            dicSockets.Remove(clientEp);
            listBox1.Items.Remove(clientEp);
            //client.Close();
           // client.Dispose();
        }

        private void AppendLog(string log) {
            richTextBox1.AppendText(log + Environment.NewLine);
        }

        //添加新的客户端
        private void AddClient(Socket client) {

            dicSockets.Add(client.RemoteEndPoint.ToString(), client);
            listBox1.Items.Add(client.RemoteEndPoint.ToString());
        }

        //处理异常
        private void DealException(Exception ex, Socket client) {
            if (ex is System.ObjectDisposedException) {
                return;
            }

            AppendLog(client.RemoteEndPoint.ToString() + ": " + ex.Message);

            if (ex is SocketException) {
                SocketException socketException = ex as SocketException;
                if (socketException.SocketErrorCode == SocketError.ConnectionAborted) {
                    //远程主机关闭了...

                }
                else if (socketException.SocketErrorCode == SocketError.ConnectionReset) {
                    //链接重置 
                }
                RemoveClient(client);
            }

        }

        private void 发送消息ToolStripMenuItem_Click(object sender, EventArgs e) {
            string selText = listBox1.Text;
            if (string.IsNullOrEmpty(selText)) {
                MessageBox.Show("请选择一个终端");
                return;
            }
            Socket client = dicSockets[selText];
            Send sendForm = new Send(_socketServer, client);
            sendForm.Show();
        }

        private void 断开连接ToolStripMenuItem_Click(object sender, EventArgs e) {
            string selText = listBox1.Text;
            if (string.IsNullOrEmpty(selText)) {
                MessageBox.Show("请选择一个终端");
                return;
            }
            Socket client = dicSockets[selText];
           // using (client) {
                RemoveClient(client);
            //}
        }

        private void 发送文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            string selText = listBox1.Text;
            if (string.IsNullOrEmpty(selText)) {
                MessageBox.Show("请选择一个终端");
                return;
            }
            Socket client = dicSockets[selText];
            SendFile sendFile = new SendFile(client, _socketServer);
            sendFile.Show();
        }
    }
}
