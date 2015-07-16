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
        
        static void Main(string[] args)
        {
            var page = new WebPage(new Uri(connectionUrl.StartsWith("http") ? connectionUrl : "http://" + connectionUrl));
            page.Download();


            Console.WriteLine("IT'S DONE!");
            Console.Read();
        }
    }
}
