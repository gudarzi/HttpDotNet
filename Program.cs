using System.Net;

var socketsHandler = new SocketsHttpHandler
{
  EnableMultipleHttp2Connections = false // Setting this to false to make it send all requests using only 1 connection
};

HttpClient myHttpClient = new HttpClient(socketsHandler)
{
  DefaultRequestVersion = HttpVersion.Version20,
  DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
};

string requestUrl = "https://WebsiteThatSupportsHTTPv2.com";

var tasks = Enumerable.Range(1, 100).Select(i => Task.Run(async () =>
{
  try
  {
    Console.WriteLine($"GET {requestUrl}. Iteration: {i}");
    HttpResponseMessage response = await myHttpClient.GetAsync(requestUrl);
    response.EnsureSuccessStatusCode();
    Console.WriteLine($"Response HttpVersion for iteration {i}: {response.Version}");
    string responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"Response Body Length for iteration {i} is: {responseBody.Length}");
  }
  catch (HttpRequestException e)
  {
    Console.WriteLine($"HttpRequestException : {e.Message}");
  }
}));

await Task.WhenAll(tasks);