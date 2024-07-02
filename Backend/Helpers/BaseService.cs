using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
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
    var certString = Environment.GetEnvironmentVariable("MTLS_CERT") ?? throw new Exception("Could not load MTLS_CERT");
    var passString = Environment.GetEnvironmentVariable("MTLS_KEY")?.Trim() ?? throw new Exception("Could not load MTLS_KEY");

    var cert = Convert.FromBase64String(certString);
    var clientCert = new X509Certificate2(cert, passString);

    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(clientCert);

    var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("origin-service", "ShortTermInsurance");
    return client;
  }

  internal async Task<HttpResponseMessage> PerformCall(string service, string url, object body, HttpMethod method)
  {
    var jsonBody = JsonConvert.SerializeObject(body);
    var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
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

  internal static string RemoveBeginEndCertificate(string cert)
  {
    const string beginCert = "-----BEGIN CERTIFICATE-----";
    const string endCert = "-----END CERTIFICATE-----";

    cert = cert.Replace(beginCert, string.Empty, StringComparison.OrdinalIgnoreCase);
    cert = cert.Replace(endCert, string.Empty, StringComparison.OrdinalIgnoreCase);
    cert = cert.Replace("\n", string.Empty).Replace("\r", string.Empty); 
    return cert.Trim();
  }

  //public static void SaveCertToEnv()
  //{
  //  var certPath = @"C:\Users\bbdnet2824\Downloads\short_term_insurance.pfx";
  //  // Read the PFX certificate file as bytes
  //  byte[] certBytes = File.ReadAllBytes(certPath);

  //  // Convert the bytes to base64 string
  //  string base64Cert = Convert.ToBase64String(certBytes);

  //  // Set the environment variable
  //  Environment.SetEnvironmentVariable("MTLS_CERT", base64Cert, EnvironmentVariableTarget.User);
  //}
}