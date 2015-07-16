using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    public abstract class HTTPDownloadableContent
    {

        private const string GetRequest = "GET {0} HTTP/1.1 \r\n" +
                                          "Host: {1}\r\n\r\n";
        protected Uri Uri;
        protected IPAddress IpAdress;
        protected string Header { get; private set; }


        public HTTPDownloadableContent(Uri uri)
        {
            Uri = uri;
            IpAdress = DNS.GetIp(Uri.Host);
        }

        public void Download()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(IpAdress, 80);
                var stream = tcpClient.GetStream();

                var byteRequest = Encoding.ASCII.GetBytes(string.Format(GetRequest, Uri.LocalPath, Uri.Host));
                stream.Write(byteRequest, 0, byteRequest.Length);



                List<byte> srcByte = new List<byte>();
                while (!stream.DataAvailable) ;
                while (stream.DataAvailable)
                {
                    byte[] data = new byte[tcpClient.ReceiveBufferSize];
                    int read = stream.Read(data, 0, tcpClient.ReceiveBufferSize);
                    srcByte.AddRange(data.Take(read));
                }

                SaveFile(ParseRequest(srcByte.ToArray()));
            }
        }
        private string ParseRequest(byte[] receivedData)
        {
            var text = Encoding.Default.GetString(receivedData);
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            Header = String.Join(Environment.NewLine, lines.TakeWhile(x => x != ""));
            return ExtractContent(String.Join(Environment.NewLine, lines.SkipWhile(x => x != "").SkipWhile(x => x == "")));
        }
        protected abstract void SaveFile(string content);

        protected virtual string ExtractContent(string content)
        {
            return content;
        }
    }
}
