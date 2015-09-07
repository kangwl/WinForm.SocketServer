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

namespace Client {
    public partial class Form1 : Form {
        Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();

        public Form1() {
            InitializeComponent();

            asyncEventArgs.AcceptSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            asyncEventArgs.Completed += asyncEventArgs_Completed;

            asyncEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3345);
            bool s = socket.ConnectAsync(asyncEventArgs);

        }

        private void asyncEventArgs_Completed(object sender, SocketAsyncEventArgs e) {

        }
    }
}
