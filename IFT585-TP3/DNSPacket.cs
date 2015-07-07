﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public byte[] CreatePacket(string url)
        {
            Random random = new Random();
            IEnumerable<byte> id = new byte[] {0,0x0f};// BitConverter.GetBytes((ushort)random.Next());
            IEnumerable<byte> flag = BitConverter.GetBytes((ushort)0x0100).Reverse();
            IEnumerable<byte> nbQuest = BitConverter.GetBytes((ushort)1).Reverse();
            IEnumerable<byte> nbAns = BitConverter.GetBytes((ushort)0), nbAuth = BitConverter.GetBytes((ushort)0), nbAdd = BitConverter.GetBytes((ushort)0);
            IEnumerable<byte> nbLabel = new byte[] { 3 };
            ChangeUrl(ref url);
            IEnumerable<byte> nameBytes = System.Text.Encoding.ASCII.GetBytes(url); //new byte[] {0x77,0x77,0x77,0x06,0x67,0x6f,0x6f,0x67,0x6c,0x65,0x03,0x63,0x6f,0x6d};//System.Text.Encoding.ASCII.GetBytes(url);
            IEnumerable<byte> type = BitConverter.GetBytes((ushort)1).Reverse(), pktClass = BitConverter.GetBytes((ushort)1).Reverse();
            //byte[] ttl = BitConverter.GetBytes((ushort)0);

            return id.Concat(flag).Concat(nbQuest).Concat(nbAns).Concat(nbAuth).Concat(nbAdd).Concat(nbLabel).Concat(nameBytes).Concat(new byte[]{0}).Concat(type).Concat(pktClass).ToArray();
        }

        private void ChangeUrl(ref string url)
        {
            Regex reg = new Regex("^(.*?)[.](.*)[.](.*)");
            char ack = (char) 6;
            char end = (char) 3;
            url = reg.Replace(url, string.Format("$1{0}$2{1}$3", ack, end));

        }
    }
}
