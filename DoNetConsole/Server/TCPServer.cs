using Cofdream.DotNetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cofdream.Server
{
    public class TCPServer
    {
        private Socket client2;
        public void Init()
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipe);
            socket.Listen(int.MaxValue);

            socket.BeginAccept((asyncResult) =>
            {
                // 获取客户端套接字  
                Socket client = client2 = socket.EndAccept(asyncResult);
                Console.WriteLine(string.Format("客户端{0}请求连接...", client.RemoteEndPoint));

                AsyncSend(client, "服务器收到连接请求");
                AsyncSend(client, string.Format("欢迎你{0}", client.RemoteEndPoint));

                AsyncReveive(client);

            }, null);
        }

        public void Send(string str)
        {
            AsyncSend(client2, str);
        }


        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="client"></param>  
        private void AsyncReveive(Socket socket)
        {
            byte[] data = new byte[1024];
            try
            {
                //开始接收消息  
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    int length = socket.EndReceive(asyncResult);
                    Console.WriteLine(string.Format("客户端发送消息:{0}", Encoding.UTF8.GetString(data)));
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>  
        /// 发送消息  
        /// </summary>  
        /// <param name="client"></param>  
        /// <param name="p"></param>  
        private void AsyncSend(Socket client, string p)
        {
            if (client == null || p == string.Empty) return;
            //数据转码  
            byte[] data = new byte[1024];
            data = Encoding.UTF8.GetBytes(p);
            try
            {
                //开始发送消息  
                client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成消息发送  
                    int length = client.EndSend(asyncResult);
                    //输出消息  
                    Console.WriteLine(string.Format("服务器发出消息:{0}", p));
                }, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
