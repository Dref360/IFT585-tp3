using System.Runtime.InteropServices;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IFT585_TP3
{
    class WebPage : HTTPDownloadableContent
    {

        public WebPage(Uri connectionUrl) : base(connectionUrl)
        {
        }



        private IEnumerable<string> GetAllImageUrl(HtmlDocument html)
        {
            var isPicture = new Regex("([^\\s]+(\\.(?i)(jpg|gif|tiff))$)");
            var nodes = html.DocumentNode.SelectNodes("//img[@src]") ?? Enumerable.Empty<HtmlNode>();
            return nodes.Select(x => x.Attributes["src"].Value)
                .Where(x => isPicture.IsMatch(x));
        }

        protected override void SaveFile(string content)
        {
            var html = new HtmlDocument();
            Console.WriteLine("Le header de la requete pour le site " + Uri + " est : ");
            Console.WriteLine(Header+"\n");
            html.LoadHtml(content);
            html.Save("page.html");
            foreach (var imageUrl in GetAllImageUrl(html))
            {
                var url = new Uri(imageUrl.StartsWith("http") ? imageUrl : "http://" + Uri.Host + imageUrl);
                var image = new ImageFromInternet(url);
                image.Download();
            }
        }

        private string NewContent(string content, int length)
        {
            if (length > content.Length)
            {
                return content + Encoding.Default.GetString(DownloadMissingContent(length - content.Length));
            }
            else
            {
                return String.Concat(content.SkipWhile(Char.IsLetterOrDigit).SkipWhile(Char.IsWhiteSpace));
            }
        }

        protected override string ExtractContent(string content)
        {
            int contentLength = ContentLength();
            if (contentLength != 0)
            {
                return NewContent(content, contentLength);
            }
            else
            {
                int length = Convert.ToInt32(String.Concat(content.TakeWhile(Char.IsLetterOrDigit)), 16);
                return NewContent(content,length);
            }
        }
    }
}
