using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    class WebPage
    {
        private string Html;

        public WebPage(byte[] response)
            : this(Encoding.Default.GetString(response))
        {
            
        }

        public WebPage(string response)
        {
            RemoveHeader(response);
        }

        private void RemoveHeader(string response)
        {
            var sccc = response.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).SkipWhile(x => x != "").Skip(1);
            Html = sccc.Skip(1).First();
        }

        public IEnumerable<string> GetAllImageUrl()
        {
            var isPicture = new Regex("([^\\s]+(\\.(?i)(jpg|gif|tiff))$)");
            var htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(Html);
            return htmlSnippet.DocumentNode.SelectNodes("//img[@src]").Select(x => x.Attributes["src"].Value).Where(x => isPicture.IsMatch(x));
        }
    }
}
