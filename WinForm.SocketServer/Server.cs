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

        private Dictionary<string, Socket> dicSockets = new Dictionary<string, Socket>();  
        public Server() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            _socketServer.LogAction = AppendLog;
            _socketServer.RemoveAction = RemoveClient;
        }

        private readonly SocketServer _socketServer = new SocketServer("127.0.0.1", 1314);
        private void Server_Load(object sender, EventArgs e) {
            _socketServer.Listen();

            _socketServer.AcceptAction = client => {
                AddClient(client); 
                AppendLog(client.RemoteEndPoint.ToString() + " 已连接");
            };

            _socketServer.RecieveAction = (client, recieveMsg) => AppendLog(client.RemoteEndPoint + ": " + recieveMsg);
       
        }

        private void RemoveClient(Socket client) {
            string clientEp = client.RemoteEndPoint.ToString();
            dicSockets.Remove(clientEp);
            listBox1.Items.Remove(clientEp);
        }

        private void AppendLog(string log) {
            richTextBox1.AppendText(log + Environment.NewLine);
        }

        private void AddClient(Socket client) {

            dicSockets.Add(client.RemoteEndPoint.ToString(), client);
            listBox1.Items.Add(client.RemoteEndPoint.ToString());
        }


        private void 发送消息ToolStripMenuItem_Click(object sender, EventArgs e) {
            string selText = listBox1.Text;
            Socket client = dicSockets[selText];
            Send sendForm = new Send(_socketServer, client);
            sendForm.Show();
        }
    }
}
