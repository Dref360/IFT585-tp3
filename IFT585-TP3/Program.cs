﻿using System;
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
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Veuillez entrer l'url du site que vous voulez accéder");
                string connectionUrl = Console.ReadLine();
                var page = new WebPage(new Uri(connectionUrl.StartsWith("http") ? connectionUrl : "http://" + connectionUrl));
                page.Download();
            }

        }
    }
}
