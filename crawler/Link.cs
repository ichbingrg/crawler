using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;

namespace crawler
{
    public class Link
    {
        private string Url;
        private string Title;

        public Link(string Url)
        {
            this.Url = GetFullURL(Url);
            this.Title = this.GetPageTitle();
        }

        public string GetUrl()
        {
            return this.Url;
        }
        public string GetTitle()
        {
            return this.Title;
        }

        private string GetPageTitle()
        {

            HtmlWeb hw = new HtmlWeb();
            string url = this.Url;
            try
            {
                HtmlDocument doc = hw.Load(url);
                if (!(doc is null))
                {
                    var title = doc.DocumentNode.SelectSingleNode("html/head/title").InnerText;
                    if (title is null)
                    {
                        return " title could not be determined ";
                    }
                    return title.TrimStart().TrimEnd();
                }
                else
                {
                    return "empty page";
                }
            }
            catch (Exception e)
            {
                return "Title could not be determined because of Exception: " + e.Message;
            }
        }

        private string GetFullURL(string url)
        {
            if (!(url.StartsWith("www") || url.StartsWith("http")))
            {
                url = "www." + url;
            }
            if (!(url.StartsWith("http")))
            {
                url = "https://" + url;
            }
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            return url;
        }

        public void savePage()
        {
            Console.WriteLine("Saving html file : " + this.GetTitle() + ".html");

            using (StreamWriter htmlFile = new StreamWriter(this.GetTitle() + ".html", false))
            {
                using (var client = new WebClient())

                using (var stream = client.OpenRead(this.GetUrl()))

                using (var textReader = new StreamReader(stream, Encoding.UTF8, true))

                {

                    htmlFile.Write(textReader.ReadToEnd());

                }
            }
        }

    }
}
