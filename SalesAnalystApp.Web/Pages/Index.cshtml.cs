using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesAnalystApp.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;

    public List<SalesRecord> Sales { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Country { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Segment { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Descending { get; set; }

    public IndexModel(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ApiClient");
    }

    public async Task OnGetAsync([FromQuery] int page, [FromQuery] string country, [FromQuery] string segment, [FromQuery] string sortBy, [FromQuery] bool descending)
    {
        Page = page == 0 ? 1 : page; // fallback to 1
        Country = country;
        Segment = segment;
        SortBy = sortBy;
        Descending = descending;

        var url = $"api/v1/sales?page={Page}&pageSize={PageSize}";

        if (!string.IsNullOrWhiteSpace(Country)) url += $"&country={Country}";
        if (!string.IsNullOrWhiteSpace(Segment)) url += $"&segment={Segment}";
        if (!string.IsNullOrWhiteSpace(SortBy)) url += $"&sortBy={SortBy}&descending={Descending.ToString().ToLower()}";

        Console.WriteLine("Built API URL: " + url);

        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json).RootElement;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Sales = JsonSerializer.Deserialize<List<SalesRecord>>(root.GetProperty("data").ToString(), options);
            TotalCount = root.GetProperty("total").GetInt32();
        }
        else
        {
            Console.WriteLine($"API Error: {response.StatusCode}");
        }
    }

}
