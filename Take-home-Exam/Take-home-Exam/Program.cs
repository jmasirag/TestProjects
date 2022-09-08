using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

Console.Write("Input Limit Number: ");
var input = Console.ReadLine();

int.TryParse(input.ToString(), out var limit);

if(limit > 0) {
    topArticles(limit);
}


void topArticles(int limit)
{
    Uri baseUrl = new Uri("https://jsonmock.hackerrank.com/api/articles");
    HttpClient client = new HttpClient();
    client.BaseAddress = baseUrl;
    int page = 1;
    string urlParameters = $"?page={page}";
    List<Datum> dataObjects = new List<Datum>();

    while (true)
    {
        HttpResponseMessage response = client.GetAsync(urlParameters).Result;
        if (response.IsSuccessStatusCode)
        {
            var rootObject = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result); 
            dataObjects.AddRange(rootObject.data.Where(a=> !string.IsNullOrEmpty(a.title) || !string.IsNullOrEmpty(a.story_title)));

            if (rootObject.total_pages == page)
                break;
        }
        page++;
        urlParameters = $"?page={page}";
    }

    foreach( (Datum val, int index) in dataObjects.OrderByDescending(o=> string.IsNullOrEmpty(o.title)? o.story_title : o.title)
        .OrderByDescending(d => d.num_comments).Select((s,i)=> (s, i)))
    {
        
        Console.WriteLine( (string.IsNullOrEmpty(val.title) ? val.story_title : val.title));
        if (index >= (limit - 1))
            break;
    }
}

public class Datum
{
    public string title { get; set; }
    public string url { get; set; }
    public string author { get; set; }
    public int? num_comments { get; set; }
    public object story_id { get; set; }
    public string story_title { get; set; }
    public string story_url { get; set; }
    public int? parent_id { get; set; }
    public int created_at { get; set; }
}

public class Root
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<Datum> data { get; set; }
}