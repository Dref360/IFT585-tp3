﻿using System.Windows.Forms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    class Program
    {
        //private const string GetRequest = "GET /store/te-news.png HTTP/1.1 \r\n"+
        //                                  " Host: imgs.xkcd.com";
        private const string GetRequest = "GET {0} HTTP/1.1 \r\n" +
                                          " Host: {1}";

        private static UdpClient dnsClient;
        static IEnumerable<string> GetAllImageUrl(string page)
        {
            var htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(page);
            return htmlSnippet.DocumentNode.SelectNodes("//img[@src]").Select(x => x.Attributes["src"].Value);
        }
        static void Main(string[] args)
        {
            DNSPacket packet = new DNSPacket();
            dnsClient = new UdpClient();
            dnsClient.Connect(IPAddress.Parse("132.210.7.13"), 53);
            var pkt = packet.CreatePacket("xkcd.com");
            int size = dnsClient.Send(pkt, pkt.Length);
            IPEndPoint end = new IPEndPoint(IPAddress.Any,0);
            var data = dnsClient.Receive(ref end);
            var ip = data.Skip(pkt.Length + 12).Take(4);
            IPAddress addr = new IPAddress(ip.ToArray());

            Console.Read();
        }

        public void DownloadImage(IEnumerable<string> img)
        {
            foreach (string url in img)
            {
                Uri urlHost = new Uri(url,UriKind.Absolute);
                string host=urlHost.Host;
                string ressouce=urlHost.Query;
                string request = string.Format(GetRequest, ressouce, host);
                var hostIp = GetIPForImg(host);
                using (TcpClient tcpClient = new TcpClient(new IPEndPoint(hostIp, 80)))
                {
                    var stream = tcpClient.GetStream();
                    var byteRequest = Encoding.ASCII.GetBytes(request);
                    stream.Write(byteRequest, 0, byteRequest.Length);

                    List<byte> imgByte = new List<byte>();
                    while (stream.DataAvailable)
                    {
                        byte[] data = new byte[500];
                        int read = stream.Read(data, 0, 500);
                        imgByte.AddRange(data.Take(read));
                    }
                    File.WriteAllBytes(ressouce, imgByte.ToArray());
                }
               
            }
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
