namespace poc_dotnetcore_rest_client.gateways;

public interface IHttpClient
{
  T Get<T>(string url, string? accessToken = null);
  T Post<T>(string url, object body, string? accessToken = null);
  T Put<T>(string url, object body, string? accessToken = null);
  T Delete<T>(string url, string? accessToken = null);
}