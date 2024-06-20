using RestSharp.Interceptors;

namespace poc_dotnetcore_rest_client.gateways;

public class LogInterceptor : Interceptor
{
  private Guid RequestId { get; set; }
  private Stopwatch Stopwatch { get; set; }

  public LogInterceptor()
  {
    RequestId = Guid.NewGuid();
    Stopwatch = new Stopwatch();
  }

  public override ValueTask BeforeHttpRequest(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
  {
    Stopwatch.Start();
    return ValueTask.CompletedTask;
  }

  public override ValueTask BeforeRequest(RestRequest request, CancellationToken cancellationToken)
  {
    var requestLog = new
    {
      Tyoe = "Http Request",
      RequestId,
      resource = request.Resource,
      parameters = request.Parameters.Select(parameter => new
      {
        name = parameter.Name,
        value = parameter.Value,
        type = parameter.Type.ToString()
      }),
      method = request.Method.ToString().ToUpper(),
      // uri = _client.BuildUri(request),
    };
    Console.WriteLine(JsonConvert.SerializeObject(requestLog, Formatting.Indented));
    return ValueTask.CompletedTask;
  }

  public override ValueTask AfterHttpRequest(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
  {
    Stopwatch.Stop();
    return ValueTask.CompletedTask;
  }

  public override ValueTask BeforeDeserialization(RestResponse response, CancellationToken cancellationToken)
  {
    var durationMs = Stopwatch.ElapsedMilliseconds;
    var responseLog = new
    {
      Tyoe = "Http Response",
      RequestId,
      statusCode = response?.StatusCode,
      content = JsonConvert.DeserializeObject(response?.Content ?? ""),
      // headers = response?.Headers,
      responseUri = response?.ResponseUri,
      errorMessage = response?.ErrorMessage,
      durationMs
    };
    Console.WriteLine($"Request {RequestId} completed in {durationMs} ms");
    Console.WriteLine(JsonConvert.SerializeObject(responseLog, Formatting.Indented));
    return ValueTask.CompletedTask;
  }
}
