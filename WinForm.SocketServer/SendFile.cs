using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm.SocketServer {
    public partial class SendFile : Form {
        public SendFile(Socket _socket,SocketServer _socketServer) {
            InitializeComponent();
            openFileDialog1.FileName = "";
            socket = _socket;
            socketServer = _socketServer;
        }

        private Socket socket;

        private SocketServer socketServer;

        private string fileName = "";
        private void btn_Browser_Click(object sender, EventArgs e) {
            System.Windows.Forms.DialogResult result = openFileDialog1.ShowDialog();
            fileName = openFileDialog1.FileName;
            label1.Text = fileName;
        }

        private void btn_SendFile_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(fileName)) {
                MessageBox.Show("请选择文件");
                return;
            }
            using (System.IO.FileStream fileStream = System.IO.File.OpenRead(fileName)) {
                string fname = Path.GetFileName(fileName);
                SendFileStream(fname, fileStream);
            }
        }

        
        private void SendFileStream(string _fileName,FileStream fileStream) {
            long fileLen = fileStream.Length;
            string sendStr = CreateSendFileStr(_fileName, fileLen);
            //send fileinfo
            socketServer.Send(socket, sendStr);
            //send file bytes
            byte[] bytes = new byte[4096];
            int read = 0;
            do {

                read = fileStream.Read(bytes, 0, bytes.Length);

                socketServer.SendAsync(socket,bytes.Take(read).ToArray());

            } while (read > 0);
        }

        private string CreateSendFileStr(string filename,long fileLen) {
            string str = string.Format("FILE{0}|{1}", filename, fileLen);
            return str;
        }
 
    }
}
