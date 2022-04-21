using Newtonsoft.Json;

//Make the API call to get the user data
static async Task<HttpResponseMessage> GetUserData(string baseUrl, string user)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    HttpResponseMessage response = await client.GetAsync($"{baseUrl}{user}");
    return response;
}

//Add each line to the string
static string AddToFile(string fileContent, string user, string obj)
{
    fileContent += $"{user} : \r\n {obj} \r\n \r\n";
    fileContent = fileContent.Replace("\r\n", Environment.NewLine);
    return fileContent;
}

//Save all content to a file
static void SaveFile(string fileContent)
{
    string relativePath = Directory.GetCurrentDirectory();
    string fileName = "users.log";
    string fullPath = Path.Combine(relativePath, fileName);

    //Delete old instance of log file
    if (File.Exists(fullPath))
        File.Delete(fullPath);
    else
        File.Create(fullPath);

    using StreamWriter wr = new StreamWriter(fullPath);
    wr.Write(fileContent);
    wr.Close();
}

// Ask for the file path
Console.WriteLine("Inform the file path:");

// Read the path for the file
string path = Console.ReadLine();

if (path != null)
{
    //API BaseUrl
    string baseUrl = "https://api.bitbucket.org/2.0/users/";

    //Get the file using the informed path
    string[] users = System.IO.File.ReadAllLines(path);
    string fileContent = "";

    Console.WriteLine("Reading: ");
    //Search for each of the users on the list, if the is user is not found, logs error.
    foreach (string user in users)
    {
        Console.WriteLine($"{user} - {baseUrl}{user}");
        var data = await GetUserData(baseUrl,user);

        if(data.IsSuccessStatusCode)
            fileContent = AddToFile(fileContent, user, JsonConvert.SerializeObject(data.Content.ReadAsStringAsync()));
        else
            fileContent = AddToFile(fileContent, user, data.ReasonPhrase);

        await Task.Delay(5000);
    }

    SaveFile(fileContent);

    //5 seconds countdown
    Console.WriteLine("Ending in: ");
    for (int i = 5; i > 0; i--)
    {
        Console.WriteLine(i);
        await Task.Delay(1000);
    }
}
else Console.WriteLine("Invalid Path");