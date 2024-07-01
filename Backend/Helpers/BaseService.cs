using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace Backend.Services;

public class BaseService
{
  internal readonly HttpClient _httpClient;

  internal readonly string _retailEndpoint = "https://api.retailbank.projects.bbdgrad.com";
  internal readonly string _commercialEndpoint = "https://api.commercialbank.projects.bbdgrad.com";
  internal readonly string _exchangeEndpoint = "https://mese.projects.bbdgrad.com";
  internal readonly string _taxEndpoint = "https://api.mers.projects.bbdgrad.com";

  public BaseService()
  {
    _httpClient = SetupClient();
  }

  internal static HttpClient SetupClient()
  {
    var cert = Environment.GetEnvironmentVariable("MTLS_CERT_PFX") ?? throw new Exception("Could not load MTLS_CERT_PFX");
    X509Certificate clientCert = new X509Certificate2(Convert.FromBase64String(cert));

    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(clientCert);

    var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
    return client;
  }

  internal Task<HttpResponseMessage> PerformCall(string url, object body, HttpMethod method)
  {
    var jsonBody = JsonConvert.SerializeObject(body);
    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var request = new HttpRequestMessage(method, url)
    {
      Content = content
    };

    return _httpClient.SendAsync(request);
  }
}