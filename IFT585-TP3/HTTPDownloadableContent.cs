using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    public abstract class HTTPDownloadableContent
    {

        private readonly string GetRequest = "GET {0} HTTP/1.1" + Environment.NewLine +
                                          "Host: {1}" + Environment.NewLine +
                                          Environment.NewLine;
        protected Uri Uri;
        protected IPAddress IpAdress;
        protected string Header { get; private set; }

        private NetworkStream stream;
        private int bufferSize;

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
                stream = tcpClient.GetStream();

                var byteRequest = Encoding.ASCII.GetBytes(string.Format(GetRequest, Uri.LocalPath, Uri.Host));
                stream.Write(byteRequest, 0, byteRequest.Length);
                bufferSize = tcpClient.ReceiveBufferSize;


                List<byte> srcByte = new List<byte>();
                while (!stream.DataAvailable) ;
                while (stream.DataAvailable)
                {
                    byte[] data = new byte[bufferSize];
                    int read = stream.Read(data, 0, bufferSize);
                    srcByte.AddRange(data.Take(read));
                }

                ParseRequest(srcByte.ToArray());
            }
        }
        private void ParseRequest(byte[] receivedData)
        {
            var text = Encoding.Default.GetString(receivedData);
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            Header = String.Join(Environment.NewLine, lines.TakeWhile(x => x != ""));
            int code = GetHttpResponseCode();
            if (code == 200)
            {
                var content = ExtractContent(String.Join(Environment.NewLine, lines.SkipWhile(x => x != "").SkipWhile(x => x == "")));
                SaveFile(content);
            }
            else
            {
                Console.WriteLine(Header);
            }

        }
        protected abstract void SaveFile(string content);

        protected virtual string ExtractContent(string content)
        {
            return content;
        }

        private int GetHttpResponseCode()
        {
            return int.Parse(Header.Split(' ').ElementAt(1));
        }

        protected int ContentLength()
        {
            if (Header == null)
            {
                throw new Exception("The header is currently null");
            }
            if (Header.Contains("Content-Length"))
            {
                string line = Header.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).First(str => str.StartsWith("Content-Length"));
                return int.Parse(Regex.Match(line, @"\d+").Value);
            }
            else
            {
                return 0;
            }
        }

        protected byte[] DownloadMissingContent(int bytesToDownload)
        {
            int numberOfByteDownloaded = 0;
            var downloadedBytes = new List<byte>(bytesToDownload);
            while (!stream.DataAvailable) ;
            while (numberOfByteDownloaded < bytesToDownload)
            {
                byte[] data = new byte[bufferSize];
                int read = stream.Read(data, 0, bufferSize);
                numberOfByteDownloaded += read;
                downloadedBytes.AddRange(data.Take(read));
            }
            return downloadedBytes.Take(bytesToDownload).ToArray();
        }
    }
}
