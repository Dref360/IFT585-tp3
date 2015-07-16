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
            File.WriteAllBytes(Uri.LocalPath.Substring(1).Replace('/', '_'), Encoding.Default.GetBytes(content));
        }
    }
}
