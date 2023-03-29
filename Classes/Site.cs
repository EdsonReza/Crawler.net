using Crawler.Classes.Automata;

namespace Crawler.Classes;

public class Site
{
    public string Url { get; set; }
    private string Text { get; set; } = "";
    private int Depth { get; set; }

    private List<string> Links { get; set; } = new();
    private List<string> Mails { get; set; } = new();
    
    public Site(string url, int depth = 3)
    {
        Url = url;
        Depth = depth;
        if (depth >= 1)
        {
            Text = WebDownload.Download(url);
            Links = new();
            Mails = new();
            GetLinks();
        }
    }

    private void GetLinks()
    {
        SearchMatches();
        Matches.Links.AddRange(Links);
        Matches.Mails.AddRange(Mails);
        foreach (var x in Links) 
        {
            Site st = new(x,Depth - 1);
        }
    }

    private void SearchMatches()
    {
        UrlAutomata urlrg = new(Text);
        MailAutomata mailrg = new(Text);
        Mails.AddRange(mailrg.Matches.Distinct().Except(Matches.Mails));
        Links.AddRange(urlrg.Matches.Distinct().Except(Matches.Links));
    }   
}
