using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace IFT585_TP3
{
    public class WebPage
    {
        private IPAddress Ip;
        public WebPage(IPAddress ip)
        {
            Ip = ip;
        }
        private string BuildRequest()
        {
            return
@"GET /
Host: google.com";
                
        }
        public string DownloadPage()
        {
            var a = (HttpWebRequest)WebRequest.Create("http://google.com");
            var b =(HttpWebResponse)a.GetResponse();
            using (TcpClient client = new TcpClient("google.com",80))
            {
                //client.Connect(Ip, 80);
                var request = Encoding.ASCII.GetBytes(BuildRequest());
                client.Client.ReceiveTimeout = 2000;
                using(var tcpStream = client.GetStream())
                {
                    tcpStream.Write(request, 0, request.Length);
                    var buffer = new byte[client.ReceiveBufferSize];

                    //while (!tcpStream.DataAvailable)
                    //{
                    //    Thread.Yield();
                    //}

                    int bytes = tcpStream.Read(buffer, 0, buffer.Length);
                    return Encoding.ASCII.GetString(buffer, 0, bytes);
                }
            }
        }
    }
}
