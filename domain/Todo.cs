namespace poc_dotnetcore_rest_client.domain
{
  public class Todo
  {
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("completed")]
    public bool Completed { get; set; }

    public Todo()
    {
      Title = string.Empty;
    }
  }
}