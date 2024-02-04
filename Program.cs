using System.Diagnostics;
using System.Net;

var socketsHandler = new SocketsHttpHandler
{
  // Set this to false to make it send all requests using only 1 connection
  EnableMultipleHttp2Connections = false
};

HttpClient myHttpClient = new HttpClient(socketsHandler)
{
  DefaultRequestVersion = HttpVersion.Version20,
  DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
};

string requestUrl = "https://httpbin.org/get";

var stopwatch = Stopwatch.StartNew();

var tasks = Enumerable.Range(1, 10).Select(i => Task.Run(async () =>
{
  try
  {
    Console.WriteLine($"* GET {requestUrl}. Iteration: {i}");
    
    myHttpClient.DefaultRequestHeaders.Date = DateTime.Now;
    HttpResponseMessage response = await myHttpClient.GetAsync(requestUrl);
    response.EnsureSuccessStatusCode();

    Console.WriteLine($"[+] Request Time At Iteration {i}: {myHttpClient.DefaultRequestHeaders.Date}\n Server Time At Iteration {i}: {response.Headers.Date}\n Response HttpVersion for iteration {i}: {response.Version}\n Elapsed Time At Iteration {i}: {stopwatch.ElapsedMilliseconds}\n\tTime Diff: {response.Headers.Date - myHttpClient.DefaultRequestHeaders.Date}");
    
    string responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"- Response Body Length for iteration {i} is: {responseBody.Length}");
  }
  catch (HttpRequestException e)
  {
    Console.WriteLine($"[-] HttpRequestException : {e.Message}");
  }
}));

try { await Task.WhenAll(tasks); } catch { /* pass! */ }