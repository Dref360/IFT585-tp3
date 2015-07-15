using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace IFT585_TP3
{
    class Program
    {
        private const string GetRequest = "GET {0} HTTP/1.1 \r\n" +
                                          "Host: {1}\r\n\r\n";

        private const string connectionUrl = "reedit.com";

        private static UdpClient dnsClient;
        
        static void Main(string[] args)
        {
            DNSPacket packet = new DNSPacket();
            dnsClient = new UdpClient();
            dnsClient.Connect(IPAddress.Parse("8.8.8.8"), 53);
            var pkt = packet.CreatePacket(connectionUrl);
            int size = dnsClient.Send(pkt, pkt.Length);
            IPEndPoint end = new IPEndPoint(IPAddress.Any,0);
            var data = dnsClient.Receive(ref end);
            var ip = data.Skip(pkt.Length + 12).Take(4);
            IPAddress addr = new IPAddress(ip.ToArray());

            DownloadSource(addr);

            Console.WriteLine("IT'S DONE!");
            Console.Read();
        }

        static public void DownloadSource(IPAddress addr)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(addr, 80);
                var stream = tcpClient.GetStream();

                var byteRequest = Encoding.ASCII.GetBytes(string.Format(GetRequest, "/", connectionUrl));
                stream.Write(byteRequest, 0, byteRequest.Length);

                System.Threading.Thread.Sleep(1000);

                List<byte> srcByte = new List<byte>();
                while (!stream.DataAvailable) ;
                while (stream.DataAvailable)
                {
                    byte[] data = new byte[500];
                    int read = stream.Read(data, 0, 500);
                    srcByte.AddRange(data.Take(read));
                }
                var webpage = new WebPage(srcByte.ToArray());
                DownloadImage(webpage.GetAllImageUrl());
                File.WriteAllBytes("lol.htm", srcByte.ToArray());
            }
        }

        public static void DownloadImage(IEnumerable<string> images)
        {
            foreach (string img in images)
            {
                var url = img.StartsWith("http") ? img : "http://" + connectionUrl + img;
                Uri urlHost = new Uri(url,UriKind.Absolute);
                string host=urlHost.Host;
                string ressouce=urlHost.LocalPath;
                string request = string.Format(GetRequest, ressouce, host);
                var hostIp = GetIPForImg(host);
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(new IPEndPoint(hostIp, 80));
                    var stream = tcpClient.GetStream();
                    var byteRequest = Encoding.ASCII.GetBytes(request);
                    stream.Write(byteRequest, 0, byteRequest.Length);
                    stream.Flush();
                    List<byte> imgByte = new List<byte>();
                    while (!stream.DataAvailable);
                    int lenght = 0;
                    {//Tente pas de renommer
                        byte[] data = new byte[500];
                        int read = stream.Read(data, 0, 500);
                        imgByte.AddRange(data.Take(read));
                        string head = Encoding.ASCII.GetString(data);
                        var contentLenght = (head.Skip(head.IndexOf("Content-Length",StringComparison.InvariantCultureIgnoreCase) + 15).TakeWhile(c => c != '\r').ToArray());
                        lenght = int.Parse(new string(contentLenght));
                    }

                    while (stream.DataAvailable || imgByte.Count < lenght)
                    {
                        byte[] data = new byte[32000];
                        int read = stream.Read(data, 0, 32000);
                        imgByte.AddRange(data.Take(read));
                    }
                    imgByte = RemoveHeader(imgByte.ToArray()).ToList();
                    File.WriteAllBytes(ressouce.Substring(1).Replace('/','_'), imgByte.ToArray());
                }
               
            }
        }

        private static byte[] RemoveHeader(byte[] response)
        {
            string resString = Encoding.ASCII.GetString(response);
            var sccc = resString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).SkipWhile(x => x != "").Skip(1);
            return Encoding.ASCII.GetBytes(sccc.First());

        }

        private static IPAddress GetIPForImg(string host)
        {
            var packet = new DNSPacket().CreatePacket(host);
            int size = dnsClient.Send(packet, packet.Length);
            IPEndPoint end = new IPEndPoint(IPAddress.Any, 0);
            var data = dnsClient.Receive(ref end);
            var ip = data.Skip(packet.Length + 12).Take(4);
            return new IPAddress(ip.ToArray());
        }
    }
}
