using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace VideoReceiver
{
    class Program
    {
        static TcpClient client;
        

        //static string IP = "127.0.0.1";
        static int Port = 63999;

        static int bufferLength = 200;
        static byte[] buffer = new byte[bufferLength];

        static NetworkStream networkStream;

        static void Main(string[] args)
        {

            Console.WriteLine("TcpServer is already start!"+ DateTime.Now.ToString());
            Console.WriteLine("等待客户端连接" + DateTime.Now.ToString());

            ConnectAndListen();
            
        }

        static async void ConnectAndListen()
        {

            //建立监听对象，监听任何ip，端口为63999
            TcpListener listener = new TcpListener(IPAddress.Any, Port);

            //开始监听
            listener.Start();

            while (true)
            {
                
                //线程会挂在在这儿，直到客户端发来连接请求
                client = listener.AcceptTcpClient();

                Console.WriteLine("客户端已连接"+client.Client.RemoteEndPoint+DateTime.Now.ToString());

                //得到从客户端传来的网络流
                networkStream = client.GetStream();

                DateTime Now = DateTime.Now;

                FileStream fileStream = new FileStream(
                @"D:\vsProject\VS2019Project\FileStorage\" + Now.Year + Now.Month + Now.Day + Now.Hour + Now.Minute + Now.Second + ".jpg", FileMode.Create);

                //如果网络流中有数据
                while (networkStream.DataAvailable)
                {
                    
                    //异步读取网络流中的byte信息
                    var byteLength = await networkStream.ReadAsync(buffer, 0, bufferLength);

                    //异步写入数据到文件流中
                    fileStream.Write(buffer, 0, byteLength);

                }

                //判断是否和客户端还存在连接
                if (!IsConnected(client))
                {
                    Console.WriteLine("已经与客户端断开连接" + DateTime.Now.ToString());
                }

                client.Close();
                fileStream.Dispose();
                fileStream.Close();
                networkStream.Dispose();

            }
        }

        /// <summary>
        /// 判断socket是否还连接
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        static bool IsConnected(TcpClient tcpClient)
        {
            return !((tcpClient.Client.Poll(1000, SelectMode.SelectRead) && (tcpClient.Client.Available == 0)) || !tcpClient.Client.Connected);
        }
    }
}
