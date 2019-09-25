using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCPClientForImage
{
    class Program
    {

        static int Port = 63999;

        static void Main(string[] args)
        {
            SendImageToServer(@"E:\个人\本人.jpg");
        }

        static void SendImageToServer(string imgURI)
        {
            if (!File.Exists(imgURI))
            {
                //停止函数
                return;
            }

            //创建一个文件流打开图片
            FileStream fileStream = new FileStream(imgURI, FileMode.Open);

            //声明一个byte[]数组接收图片字节信息
            byte[] fileBytes = new byte[fileStream.Length];

            using (fileStream)
            {
                fileStream.Read(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            //服务器IP地址
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");

            //创建TCPClient对象，以实现与服务器的连接。
            TcpClient tcpClient = new TcpClient();

            //连接服务器
            tcpClient.Connect(iPAddress, Port);

            using (tcpClient)
            {
                //连接完服务器后在客户端和服务端之间产生一个流的通道
                NetworkStream networkStream = tcpClient.GetStream();

                using (networkStream)
                {
                    //将图片的字节数据写入流中
                    networkStream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
        }
    }
}
