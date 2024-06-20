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

  private RestRequest CreateRequest(string url, Method method)
  {
    var request = new RestRequest(url, method) {
      Interceptors = [new LogInterceptor()]
    };
    return request;
  }

  private T ExecuteRequest<T>(RestRequest request)
  {
    RestResponse<T> response = _client.Execute<T>(request);
    if (response.ErrorException != null) throw new Exception($"Error executing request: {response.ErrorException.Message}", response.ErrorException);
    if (!response.IsSuccessful) throw new Exception($"Request failed with status code {response.StatusCode}: {response.Content}");
    return response.Data!;
  }

  public T Get<T>(string url, string? accessToken)
  {
    var request = CreateRequest(url, Method.Get);
    AddAuthorizationHeader(request, accessToken);
    return ExecuteRequest<T>(request);
  }

  public T Post<T>(string url, object body, string? accessToken)
  {
    var request = CreateRequest(url, Method.Post);
    AddAuthorizationHeader(request, accessToken);
    AddJsonBody(request, body);
    return ExecuteRequest<T>(request);
  }

  public T Put<T>(string url, object body, string? accessToken)
  {
    var request = CreateRequest(url, Method.Put);
    AddAuthorizationHeader(request, accessToken);
    AddJsonBody(request, body);
    return ExecuteRequest<T>(request);
  }
  public T Delete<T>(string url, string? accessToken)
  {
    var request = CreateRequest(url, Method.Delete);
    AddAuthorizationHeader(request, accessToken);
    return ExecuteRequest<T>(request);
  }
}
