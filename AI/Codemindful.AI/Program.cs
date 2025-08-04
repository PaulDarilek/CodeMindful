using Microsoft.Extensions.AI;

namespace Codemindful.AI;

internal class Program
{


    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Func<Uri, string, Task> chat = ChatWithHistoryAsync;
        //Func<Uri, string, Task> chat = ChatAsync;

        var uri = new Uri("http://localhost:11434/");
        var model = AIModels.Phi3Mini;

        chat.Invoke(uri, model).GetAwaiter().GetResult();
    }

    static async Task ChatAsync(Uri uri, string model)
    {
        IChatClient client = new OllamaChatClient(uri, model);

        var response = await client.CompleteAsync("What is AI?");

        Console.WriteLine(response.Message);
    }

    static async Task ChatWithHistoryAsync(Uri uri, string model)
    {
        IChatClient client = new OllamaChatClient(uri, model);

        var chatHistory = new List<ChatMessage>()
        {
            new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
            new ChatMessage(ChatRole.User, "What is AI?"),
        };

        var chatOptions = new ChatOptions
        {
            MaxOutputTokens = 1000,
            Seed = 2024,
        };

        ChatCompletion completion = await client.CompleteAsync(chatHistory, options: chatOptions);
        foreach (ChatMessage chat in completion.Choices) {
            chatHistory.Add(chat);
        }

        Console.WriteLine(completion.ToString());
    }

}


public static class AIModels
{
    public const string Phi3Mini = "phi3:mini";
    public const string Llama_3_1 = "llama3.1";
}