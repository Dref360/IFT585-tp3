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
            var ip = packet.SendDNSRequest("google.com");
            Console.WriteLine(ip);
            var page = new WebPage(IPAddress.Parse("206.167.212.123"));
            Console.WriteLine(page.DownloadPage());
            Console.Read();
        }
    }
}
