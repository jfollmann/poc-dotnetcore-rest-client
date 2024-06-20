namespace poc_dotnetcore_rest_client.gateways;

public class RestSharpClient : IHttpClient
{
  private readonly RestClient _client;

  public RestSharpClient(string baseUrl)
  {
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
      throw new ArgumentException("Base URL must be provided.", nameof(baseUrl));
    }
    _client = new RestClient(baseUrl);
  }

  private void AddAuthorizationHeader(RestRequest request, string? accessToken)
  {
    if (!string.IsNullOrWhiteSpace(accessToken))
      request.AddHeader("Authorization", $"Bearer {accessToken}");
  }

  private void AddJsonBody(RestRequest request, object body)
  {
    if (body != null)
      request.AddJsonBody(body);
  }

  private void WriteLog(Guid requestId, RestRequest request, RestResponse? response, long durationMs)
  {
    var requestLog = new
    {
      requestId,
      resource = request.Resource,
      parameters = request.Parameters.Select(parameter => new
      {
        name = parameter.Name,
        value = parameter.Value,
        type = parameter.Type.ToString()
      }),
      method = request.Method.ToString().ToUpper(),
      uri = _client.BuildUri(request),
    };
    var responseLog = new
    {
      requestId,
      statusCode = response?.StatusCode,
      content = JsonConvert.DeserializeObject(response?.Content ?? ""),
      // headers = response?.Headers,
      responseUri = response?.ResponseUri,
      errorMessage = response?.ErrorMessage,
      durationMs
    };
    Console.WriteLine($"Request {requestId} completed in {durationMs} ms");
    Console.WriteLine(JsonConvert.SerializeObject(requestLog, Formatting.Indented));
    Console.WriteLine(JsonConvert.SerializeObject(responseLog, Formatting.Indented));
  }

  private T ExecuteRequest<T>(RestRequest request)
  {
    var requestId = Guid.NewGuid();
    RestResponse<T>? response = null;
    var stopWatch = new Stopwatch();
    try
    {
      stopWatch.Start();
      response = _client.Execute<T>(request);
      stopWatch.Stop();
      if (response.ErrorException != null) throw new Exception($"Error executing request: {response.ErrorException.Message}", response.ErrorException);
      if (!response.IsSuccessful) throw new Exception($"Request failed with status code {response.StatusCode}: {response.Content}");
      return response.Data!;
    }
    catch
    {
      WriteLog(requestId, request, response, stopWatch.ElapsedMilliseconds);
      throw;
    }
    finally
    {
      WriteLog(requestId, request, response, stopWatch.ElapsedMilliseconds);
    }
  }

  public T Get<T>(string url, string? accessToken)
  {
    var request = new RestRequest(url, Method.Get);
    AddAuthorizationHeader(request, accessToken);
    return ExecuteRequest<T>(request);
  }

  public T Post<T>(string url, object body, string? accessToken)
  {
    var request = new RestRequest(url, Method.Post);
    AddAuthorizationHeader(request, accessToken);
    AddJsonBody(request, body);
    return ExecuteRequest<T>(request);
  }

  public T Put<T>(string url, object body, string? accessToken)
  {
    var request = new RestRequest(url, Method.Put);
    AddAuthorizationHeader(request, accessToken);
    AddJsonBody(request, body);
    return ExecuteRequest<T>(request);
  }
  public T Delete<T>(string url, string? accessToken)
  {
    var request = new RestRequest(url, Method.Delete);
    AddAuthorizationHeader(request, accessToken);
    return ExecuteRequest<T>(request);
  }
}
