using Spectre.Console;
using Cocona;
using Crawler.Classes;
using System.Text;
using System;

namespace Crawler.Commands;

public class Search
{

    [Command(Description = "Get the URL and crawl it")]
    public static void Crawl([Argument(Description = "Target to be crawled")]string url, [Option('d', Description = "Depth of the crawl")]int depth = 3)
    {
        Console.WriteLine();
        AnsiConsole.Write(new Rule($"[gold1]Crawling[/] [royalblue1]{url}[/]").LeftJustified());
        try {
            Console.WriteLine();
            Site site = new(url, depth);
            StringBuilder mailtext = new();
            site.Mails.ForEach(link => mailtext.AppendLine($"[lightseagreen]{link}[/]"));
            if (mailtext.Length > 0) {
                var panelMails = new Panel(mailtext.ToString());
                panelMails.Border = BoxBorder.Rounded;
                panelMails.Padding = new Padding(1,1);
                panelMails.Header = new PanelHeader("[green]Emails Found[/]");
                AnsiConsole.Write(panelMails);
            }
            else AnsiConsole.MarkupLine("[red]No mail matches found \n[/]");
            StringBuilder urltext = new();
            site.Links.ForEach(link => urltext.AppendLine($"[lightseagreen]{link}[/]"));
            if (urltext.Length > 0)
            {
                var panelLinks = new Panel(urltext.ToString());
                panelLinks.Border = BoxBorder.Rounded;
                panelLinks.Padding = new Padding(1,1);
                panelLinks.Header = new PanelHeader("[green]Links Found[/]");
                AnsiConsole.Write(panelLinks);
            }
            else AnsiConsole.MarkupLine("[red]No URL matches found \n[/]");
        }
        catch(Exception ex)
        {
            AnsiConsole.Markup($"[red]Error: {ex.Message} [/]");
        }
    }

}
