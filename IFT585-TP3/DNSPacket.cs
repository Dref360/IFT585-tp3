using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    /*ID - ushort
      Flag - ushort
     *Number of Questions - ushort (1)
     *Number of Answer - ushort (0)
     *Number of authority - ushort (0)
     *Number of additionnal RR - ushort (0)
     *Questions ->NbLabel - byte (1) 
     *            Question*
     *Question -> url (ASCII)
     *            type - ushort (1)
     *            class - ushort (1)
     *            TTL - 32bit (0) Dont cache
     */
    class DNSPacket
    {
        private byte[] CreatePacket(string url)
        {
            Random random = new Random();
            IEnumerable<byte> id = new byte[] {0,0x0f};// BitConverter.GetBytes((ushort)random.Next());
            IEnumerable<byte> flag = BitConverter.GetBytes((ushort)0x0100).Reverse();
            IEnumerable<byte> nbQuest = BitConverter.GetBytes((ushort)1).Reverse();
            IEnumerable<byte> nbAns = BitConverter.GetBytes((ushort)0), nbAuth = BitConverter.GetBytes((ushort)0), nbAdd = BitConverter.GetBytes((ushort)0);
            IEnumerable<byte> nbLabel = new byte[] { ChangeUrl(ref url) };
            
            IEnumerable<byte> nameBytes = System.Text.Encoding.ASCII.GetBytes(url); //new byte[] {0x77,0x77,0x77,0x06,0x67,0x6f,0x6f,0x67,0x6c,0x65,0x03,0x63,0x6f,0x6d};//System.Text.Encoding.ASCII.GetBytes(url);
            IEnumerable<byte> type = BitConverter.GetBytes((ushort)1).Reverse(), pktClass = BitConverter.GetBytes((ushort)1).Reverse();
            //byte[] ttl = BitConverter.GetBytes((ushort)0);
            return IFT585Helper.Flatten(id, flag, nbQuest, nbAns, nbAuth, nbAdd, nbLabel, nameBytes, new byte[] { 0 }, type, pktClass).ToArray();
        }

        

        private byte ChangeUrl(ref string url)
        {
            if (url.Contains("www."))
                url = url.Substring(url.IndexOf("www.", StringComparison.InvariantCultureIgnoreCase) + 4);
            Regex reg = new Regex("^(.*)[.](.*)");
            char ack = (char) 6;
            char end = (char) 3;
            int name = url.LastIndexOf('.');
            url = reg.Replace(url, string.Format("$1{0}$2", end));
            return (byte)name;
        }

        public IPAddress SendDNSRequest(string url)
        {
            UdpClient client = new UdpClient();
            client.Connect(IPAddress.Parse("8.8.8.8"), 53);
            var pkt = CreatePacket("http://www.usherbrooke.ca/");
            int size = client.Send(pkt, pkt.Length);
            IPEndPoint end = new IPEndPoint(IPAddress.Any, 0);
            var data = client.Receive(ref end);
            return new IPAddress(data.Skip(pkt.Length + 6 * 2).Take(4).ToArray());
        }
    }
}
