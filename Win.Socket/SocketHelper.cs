using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Win.Socket {
    public class SocketHelper {

        System.Net.Sockets.Socket connectionSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

        public SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();

        public SocketHelper() {
            asyncEventArgs.AcceptSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            asyncEventArgs.Completed += asyncEventArgs_Completed;
        }

        void asyncEventArgs_Completed(object sender, SocketAsyncEventArgs e) {
             
        }

        public void Bind() {
            connectionSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3345));
            connectionSocket.Listen(10);
            bool s = connectionSocket.AcceptAsync(asyncEventArgs);
        }
    }
}
