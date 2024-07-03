using Newtonsoft.Json;

using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Backend.Services;

public class BaseService(ILogger<BaseService> logger)
{
  internal readonly HttpClient _httpClient = SetupClient();
  internal readonly ILogger<BaseService> _logger = logger;

  internal readonly string _retailEndpoint = "https://api.retailbank.projects.bbdgrad.com";
  internal readonly string _commercialEndpoint = "https://api.commercialbank.projects.bbdgrad.com";
  internal readonly string _exchangeEndpoint = "https://api.mese.projects.bbdgrad.com";
  internal readonly string _taxEndpoint = "https://api.mers.projects.bbdgrad.com";

  internal static HttpClient SetupClient()
  {
    var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("X-Origin", "short_term_insurance");

    return client;
  }

  internal async Task<HttpResponseMessage> PerformCall(string service, string url, object body, HttpMethod method)
  {
    try
    {
      var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
      content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      var request = new HttpRequestMessage(method, url)
      {
        Content = content
      };

      _logger.LogInformation("Creating {method} call to {service}! {url}", method.ToString(), service, url);

      var response = await _httpClient.SendAsync(request);
      var responseContent = await response.Content.ReadAsStringAsync();
      _logger.LogInformation("Call Result: {StatusCode}, {responseContent}", response.StatusCode, responseContent);

      return response;
    }
    catch (HttpRequestException ex)
    {
      // Log network-related errors
      _logger.LogError(ex, "HTTP request failed: {service}, {url}", service, url);
      return new HttpResponseMessage(HttpStatusCode.InternalServerError);
    }
    catch (TaskCanceledException ex)
    {
      // Log timeout errors
      _logger.LogError(ex, "HTTP request timed out: {service}, {url}", service, url);
      return new HttpResponseMessage(HttpStatusCode.InternalServerError);
    }
    catch (Exception ex)
    {
      // Handle any other unexpected exceptions
      _logger.LogError(ex, "Unexpected error occurred during HTTP request: {service}, {url}", service, url);
      return new HttpResponseMessage(HttpStatusCode.InternalServerError);
    }
  }
}