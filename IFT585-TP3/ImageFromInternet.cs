using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    class ImageFromInternet : HTTPDownloadableContent
    {
        public ImageFromInternet(Uri connectionUrl) : base(connectionUrl)
        {

        }

        protected override void SaveFile(string content)
        {
            Console.WriteLine("Le header pour l'image " + Uri.LocalPath + " est : ");
            Console.WriteLine(Header+"\n");
            var fileName = Uri.LocalPath.Substring(1).Replace('/', '_');
            var bytes = Encoding.Default.GetBytes(content);
            
            int contentLength = ContentLength();
            if(contentLength > content.Length)
            {
                bytes = bytes.Concat(DownloadMissingContent(contentLength - content.Length)).ToArray();
            }

            File.WriteAllBytes(fileName, bytes);
            
        }
    }
}
