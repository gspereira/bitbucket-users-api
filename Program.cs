using Newtonsoft.Json;

static string GetPathInput()
{
    // Ask for the file path
    Console.WriteLine("Inform the file path:");

    string path = Console.ReadLine();

    // Repeat the method until path is not null
    if (path == null)
    {
        Console.WriteLine("Invalid Path");
        return GetPathInput();
    }
    else return path;
}

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
static void SaveFile(string fileContent, string fullPath)
{
    //Delete old instance of log file
    if (File.Exists(fullPath))
        File.Delete(fullPath);
    else
        File.Create(fullPath);

    using StreamWriter wr = new StreamWriter(fullPath);
    wr.Write($"{DateTime.Now}{Environment.NewLine}{Environment.NewLine}");
    wr.Write(fileContent);
    wr.Close();
}

//Check if code has not run in the last 60 seconds
static Boolean CheckTimeSinceLastRun(string fullPath)
{
    //Read first line of file, where the last run dateTime is stored
    //than, compares it to the current dateTime.
    if (File.Exists(fullPath))
    {
        DateTime lastRun = DateTime.Parse(File.ReadLines(fullPath).First());
        DateTime now = DateTime.Now;
        var diffDates = now - lastRun;
        return diffDates.TotalSeconds > 60 ? true : false;
    }
    else
    {
        return true;
    }
}

//Log file path
string relativePath = Directory.GetCurrentDirectory();
string fileName = "users.log";
string fullPath = Path.Combine(relativePath, fileName);

//API BaseUrl
string baseUrl = "https://api.bitbucket.org/2.0/users/";

if (CheckTimeSinceLastRun(fullPath))
{
    // Read the path for the file
    string path = GetPathInput();

    //Get the file using the informed path
    string[] users = System.IO.File.ReadAllLines(path);
    string fileContent = "";

    Console.WriteLine("Reading: ");
    //Search for each of the users on the list, if the is user is not found, logs error.
    foreach (string user in users)
    {
        Console.WriteLine($"{user} - {baseUrl}{user}");
        var data = await GetUserData(baseUrl, user);

        if (data.IsSuccessStatusCode)
            fileContent = AddToFile(fileContent, user, JsonConvert.SerializeObject(data.Content.ReadAsStringAsync()));
        else
            fileContent = AddToFile(fileContent, user, data.ReasonPhrase);

        await Task.Delay(5000);
    }

    SaveFile(fileContent, fullPath);

    //5 seconds countdown
    Console.WriteLine("Ending in: ");
    for (int i = 5; i > 0; i--)
    {
        Console.WriteLine(i);
        await Task.Delay(1000);
    }
}
else Console.WriteLine("Already ran in the last 60 seconds");








