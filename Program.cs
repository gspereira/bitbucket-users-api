using System.Net;

static async Task<HttpResponseMessage> GetUserData(string user)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    HttpResponseMessage response = await client.GetAsync($"https://api.bitbucket.org/2.0/users/{user}");
    return response;
}

// Ask for the file path
Console.WriteLine("Inform the file path:");

// Read the path for the file
string path = Console.ReadLine();

if (path != null)
{
    //Get the file using the informed path
    string[] users = System.IO.File.ReadAllLines(path);

    foreach (string user in users)
    {
        var data = await GetUserData(user);
        if (data.IsSuccessStatusCode)
        {
            
        } else Console.WriteLine($"{user} : {data.ReasonPhrase}");
        await Task.Delay(5000);
    }
}
else Console.WriteLine("Invalid Path");
