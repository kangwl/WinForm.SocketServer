using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm.SocketClient {
    public partial class Client : Form {
        public Client() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private readonly SocketClient _client = new SocketClient("127.0.0.1", 1314);

        private void Form1_Load(object sender, EventArgs e) {

            _client.ConnectServer();

            _client.recordLog = AppendLog;
            _client.RecieveAsync();

        }

        private void AppendLog(string log) {
            richTextBox1.AppendText("Server: " + log + Environment.NewLine);
        }

        private void button_send_Click(object sender, EventArgs e) {
            _client.SendAsync(txt_msg.Text.Trim());
        }
    }
}
