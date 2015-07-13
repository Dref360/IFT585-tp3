using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    class Program
    {
        static IEnumerable<string> GetAllImageUrl(string page)
        {
            var htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(page);
            return htmlSnippet.DocumentNode.SelectNodes("//img[@src]").Select(x => x.Attributes["src"].Value);
        }
        static void Main(string[] args)
        {
            DNSPacket packet = new DNSPacket();
            UdpClient client = new UdpClient();
            client.Connect(IPAddress.Parse("132.210.7.13"), 53);
            var pkt = packet.CreatePacket("wallpaperswide.com");
            int size = client.Send(pkt, pkt.Length);
            IPEndPoint end = new IPEndPoint(IPAddress.Any,0);
            var data = client.Receive(ref end);
            var ip = data.Skip(pkt.Length + 12).Take(4);
            IPAddress addr = new IPAddress(ip.ToArray());
            Console.Read();
        }
    }
}
