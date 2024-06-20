using poc_dotnetcore_rest_client.domain;
using poc_dotnetcore_rest_client.gateways;

namespace poc_dotnetcore_rest_client;

class Program
{

  static void WriteItem(Todo item) => Console.WriteLine($"{item.Id} | {item.Title} | {item.Completed}");

  static void Main(string[] args)
  {
    JsonConvert.DeserializeObject("");

    IHttpClient httpClient = new RestSharpClient("https://jsonplaceholder.typicode.com");

    httpClient.Get<List<Todo>>("/todos");
    httpClient.Post<Todo>("/todos", new Todo { UserId = 90, Title = "Sample 999", Completed = true });
    httpClient.Put<Todo>("/todos/1", new { Id = 1, Title = "Updated", Completed = true });
    httpClient.Delete<Todo>("/todos/201");

    // Console.WriteLine("------ Main Logs ------");
    // Console.WriteLine("[GET Result]");
    // foreach (var item in resultGet) 
    //   WriteItem(item);

    // Console.WriteLine("[POST Result]");
    // WriteItem(resultPost);

    // Console.WriteLine("[PUT Result]");
    // WriteItem(resultPut);

    // Console.WriteLine("[DELETE Result]");
    // WriteItem(resultDelete);
  }
}
