using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            IEnumerable<byte> id = BitConverter.GetBytes((ushort)random.Next());
            IEnumerable<byte> flag = BitConverter.GetBytes((ushort)0x0100).Reverse();
            IEnumerable<byte> nbQuest = BitConverter.GetBytes((ushort)1).Reverse();
            IEnumerable<byte> nbAns = BitConverter.GetBytes((ushort)0), nbAuth = BitConverter.GetBytes((ushort)0), nbAdd = BitConverter.GetBytes((ushort)0);
            IEnumerable<byte> nbLabel = new byte[] { 3 };
            IEnumerable<byte> nameBytes = System.Text.Encoding.UTF7.GetBytes(url);
            IEnumerable<byte> type = BitConverter.GetBytes((ushort)1).Reverse(), pktClass = BitConverter.GetBytes((ushort)1).Reverse();
            //byte[] ttl = BitConverter.GetBytes((ushort)0);

            return id.Concat(flag).Concat(nbQuest).Concat(nbAns).Concat(nbAuth).Concat(nbAdd).Concat(nbLabel).Concat(nameBytes).Concat(new byte[]{0}).Concat(type).Concat(pktClass).ToArray();
        }
    }
}
