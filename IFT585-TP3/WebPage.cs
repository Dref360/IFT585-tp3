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
            return html.DocumentNode.SelectNodes("//img[@src]")
                .Select(x => x.Attributes["src"].Value)
                .Where(x => isPicture.IsMatch(x));
        }

        protected override void SaveFile(string content)
        {
            var html = new HtmlDocument();
            Console.WriteLine("Le header de la requete pour le site : " + Uri);
            Console.WriteLine(Header);
            html.LoadHtml(content);
            html.Save("page.html");
            foreach (var imageUrl in GetAllImageUrl(html))
            {
                var url = new Uri(imageUrl.StartsWith("http") ? imageUrl : "http://" + Uri.Host + imageUrl);
                var image = new ImageFromInternet(url);
                image.Download();
            }
        }
        protected override string ExtractContent(string content)
        {
            int length = Convert.ToInt32(String.Concat(content.TakeWhile(Char.IsLetterOrDigit)),16);
            Debug.Assert(length < content.Length);
            return String.Concat(content.SkipWhile(Char.IsLetterOrDigit).SkipWhile(Char.IsWhiteSpace)).Substring(0, length);
        }
    }
}
