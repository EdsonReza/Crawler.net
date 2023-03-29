using System;
using System.Net.Http;

namespace Crawler.Classes;

public static class WebDownload
{
    public static string Download(string url)
    {
        var client = new HttpClient();
        try {
            HttpResponseMessage response = client.GetAsync(url).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return content;
        }
        catch(Exception)
        {
            return "N/A";
        }
    }
}