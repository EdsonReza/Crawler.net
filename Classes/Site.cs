using Crawler.Classes.Automata;

namespace Crawler.Classes;

public class Site
{
    public string Url { get; set; }
    private string Text { get; set; }
    private int Depth { get; set; }

    public List<string> Links { get; set; }
    public List<string> Mails { get; set; }
    
    public Site(string url, int depth = 3)
    {
        Url = url;
        Depth = depth;
        Text = WebDownload.Download(url);
        Links = new();
        Mails = new();
        GetLinks();
    }

    private void GetLinks()
    {
        if (Depth < 1) return;
        SearchMatches();
    }

    private void SearchMatches()
    {
        UrlAutomata urlrg = new(Text);
        MailAutomata mailrg = new(Text);
        Mails.AddRange(mailrg.Matches.Distinct());
        Links.AddRange(urlrg.Matches.Distinct());
    }   
}
