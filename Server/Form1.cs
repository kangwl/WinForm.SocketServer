using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win.Socket;

namespace Server {
    public partial class Form1 : Form {

        System.Net.Sockets.Socket connectionSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream,
        ProtocolType.Tcp);

        public SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
        public Form1() {
            InitializeComponent();

            asyncEventArgs.AcceptSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream,
            ProtocolType.Tcp);

            asyncEventArgs.Completed += asyncEventArgs_Completed;

            Bind();
        }

        private void asyncEventArgs_Completed(object sender, SocketAsyncEventArgs e) {
            string jhhhs = e.AcceptSocket.RemoteEndPoint.ToString();
        }

        public void Bind() {
            connectionSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3345));
            connectionSocket.Listen(10);
            bool s = connectionSocket.AcceptAsync(asyncEventArgs);
        }
    }
}
