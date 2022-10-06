// See https://aka.ms/new-console-template for more information
using PwnedPasswords;

async Task<bool> HasPasswordBeenPwned(string password, CancellationToken cancellationToken = default)
{
    var sha1Password = SHA1Util.SHA1HashStringForUTF8String(password);
    var sha1Prefix = sha1Password.Substring(0, 5);
    var sha1Suffix = sha1Password.Substring(5);
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
    int MinimumFrequencyToConsiderPwned = 1;

    try
    {
        var response = await client.GetAsync("range/" + sha1Prefix, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var frequency = await Contains(response.Content, sha1Suffix);
            var isPwned = (frequency >= MinimumFrequencyToConsiderPwned);
            if (isPwned)
            {
                Console.WriteLine($"HaveIBeenPwned API indicates the password has been pwned { frequency } times");
                Console.WriteLine("Enter another password to check or 0 to quit ");
            }
            else
            {
                Console.WriteLine("HaveIBeenPwned API indicates the password has not been pwned");
            }
            return isPwned;
        }
        Console.WriteLine("Unexpected response from API: {StatusCode}", response.StatusCode);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
    return false;
}

static async Task<long> Contains(HttpContent content, string sha1Suffix)
{
    using (var streamReader = new StreamReader(await content.ReadAsStreamAsync()))
    {
        while (!streamReader.EndOfStream)
        {
            var line = await streamReader.ReadLineAsync();
            var segments = line.Split(':');
            if (segments.Length == 2
                && string.Equals(segments[0], sha1Suffix, StringComparison.OrdinalIgnoreCase)
                && long.TryParse(segments[1], out var count))
            {
                return count;
            }
        }
    }

    return 0;
}

Console.WriteLine("Please enter the password to check..");
string? password = Console.ReadLine();
while (password != "0")
{
    Task<bool> isPwnd = HasPasswordBeenPwned(password);
    password = Console.ReadLine();
}