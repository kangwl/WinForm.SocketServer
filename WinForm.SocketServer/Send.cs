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
    public partial class Send : Form {
        public Send(SocketServer server, Socket client) {
            InitializeComponent();
            SocketServer = server;
            Client = client;
        }

        private SocketServer SocketServer { get; set; }
        private Socket Client { get; set; }

        private void button_send_Click(object sender, EventArgs e) {
            string message = txt_msg.Text.Trim();
            SocketServer.SendAsync(Client, message);
        }
    }
}
