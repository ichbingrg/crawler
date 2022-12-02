using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using crawler;
using HtmlAgilityPack;

class Program
{
    static Hashtable loaded = new Hashtable();
    static int count = 1;
    static string rootDomain;
    static void Main()
    {
        Console.WriteLine("Enter a domain to crawl : ");
        string url = Console.ReadLine();

        string userWish;


        Console.WriteLine();
        Link baseLink = new Link(url);

        int index = baseLink.GetUrl().Substring(8, (baseLink.GetUrl()).Length - 9).IndexOf("/");
        rootDomain = index == -1 ? baseLink.GetUrl() : baseLink.GetUrl().Substring(0, index + 8);
        Console.WriteLine("ROOT DOMAIN      " + rootDomain);

        //baseLink.savePage();

        // stores all the found links
        HashSet<Link> foundLinks = new HashSet<Link>();
        foundLinks.Add(baseLink);

        Console.WriteLine("Do you wish to scrape the whole site or just this one page ? \n" +
                        "Press '0' for only one page and '1' for the entire site");
        userWish = Console.ReadLine();
        Console.WriteLine("Ok, now crawling " + baseLink.GetUrl() + " ---- " + baseLink.GetTitle());

        if (File.Exists("Links.txt"))
        {
            Console.WriteLine("Filename Links.txt exists, Please delete this first");
            return;
        }

        if (userWish == "0")
        {
            foundLinks.UnionWith(ExtractNodes(baseLink.GetUrl()));
        }
        else if (userWish == "1")
        {
            /*RUN FOR LOOP IF YOU WISH TO CRAWL THE WHOLE SITE THROUGH AND THROUGH*/
            // stores {<Link>,<bool>} , So, the sites already visited don't get repeated
            Hashtable seen = new Hashtable();
            // n works as hops counter
            for (int n = 1; n > 0; n--)
            {
                HashSet<Link> Links = new HashSet<Link>(foundLinks);
                foreach (var link in Links)
                {
                    if ((seen[link]) == null)
                    {
                        seen[link] = true;
                        n++;
                        HashSet<Link> newLinks = ExtractNodes(link.GetUrl());
                        if (newLinks != null)
                        {
                            foundLinks.UnionWith(newLinks);
                        }
                    }
                };
            }
        }
        Console.WriteLine("Complete");
        Console.ReadLine();

        return;
    }
    private static HashSet<Link> ExtractNodes(string baseUrl)
    {
        HashSet<Link> Links = new HashSet<Link>();
        HtmlWeb hw = new HtmlWeb();
        try
        {
            HtmlDocument doc = hw.Load(baseUrl);

            if (doc is null)
            {
                Console.WriteLine("Page not found");
                return new HashSet<Link>();
            }

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]").ToArray())
            {
                var linkAsString = link.Attributes["href"].Value.ToString();
                if (linkAsString is "#" || linkAsString is "/")
                {
                    continue;
                }
                var fullLink = checkRelative(linkAsString, baseUrl);
                Link newLink = new Link(fullLink);
                if (loaded[newLink.GetUrl()] == null)
                {
                    loaded[newLink.GetUrl()] = true;
                    string textLine = count + ". " + newLink.GetTitle() + " -- " + newLink.GetUrl();
                    Console.WriteLine(textLine);
                    using (StreamWriter file2 = new StreamWriter("Links.txt", true))
                    {
                        file2.WriteLine(textLine);
                    }

                    // makes sure that before the link is added to the Links which is used to look for more links, the domain is the same
                    // Otherwise it can lead to an external link and the program would never stop
                    if (newLink.GetUrl().StartsWith(rootDomain))
                    {
                        Links.Add(newLink);
                    }
                    count++;
                }
            }
            return Links;
        }
        catch (Exception e)
        {
            Console.WriteLine("Page : " + baseUrl + " could not be read, Exception : " + e.Message);
            return new HashSet<Link>();
        }

    }

    private static string checkRelative(string href, string baseUrl)
    {
        if (href.Substring(0, 1) is "/")
        {
            return baseUrl + href;
        }
        else
        {
            return href;
        }
    }

}