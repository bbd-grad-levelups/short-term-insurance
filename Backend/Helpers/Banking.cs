using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Backend.Helpers;

record DebitOrderBody(string PersonaAccount, string CommercialAccount, double Amount);

public static class Banking
{
  internal static string RetailEndpoint = "https://api.commercialbank.projects.bbdgrad.com";
  internal static string CommercialEndpoint = "https://api.commercialbank.projects.bbdgrad.com";
  public static string RetailKey = "";
  public static string CommercialKey = "";
  public static string CompanyAccount = "";

  public async static void UpdateRetailDebitOrder(string personaAccount, double amount)
  {
    HttpClient client = new();
    client.DefaultRequestHeaders.Authorization = new("Bearer", RetailKey);
    // API endpoint URL
    string apiUrl = RetailEndpoint + "/debit";

    var json = JsonConvert.SerializeObject(new DebitOrderBody(personaAccount, CompanyAccount, amount));

    // Set Content-Type header to application/json
    var content = new StringContent(json);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    // Create HTTP POST request
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await client.SendAsync(request);
    Console.WriteLine(response.ToString() + "/n" + response.Content.ToString());
  }

  public static async void MakeCommercialPayment(string account, double amount)
  {
    HttpClient client = new();
    client.DefaultRequestHeaders.Authorization = new("Bearer", CommercialKey);

    // API endpoint URL
    string apiUrl = CommercialEndpoint + "/pay";

    var json = JsonConvert.SerializeObject(new DebitOrderBody(account, CompanyAccount, amount));

    // Set Content-Type header to application/json
    var content = new StringContent(json);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    // Create HTTP POST request
    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
    {
      Content = content
    };

    var response = await client.SendAsync(request);
    Console.WriteLine(response.ToString() + "/n" + response.Content.ToString());
  }
}