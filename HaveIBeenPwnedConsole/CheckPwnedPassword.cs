
namespace HaveIBeenPwnedConsole
{
    public class CheckPwnedPassword
    {
        PwnedPasswordModel pwnedPasswordModel = new PwnedPasswordModel();
        public async Task<PwnedPasswordModel> HasPasswordBeenPwned(string password, CancellationToken cancellationToken = default)
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
                    pwnedPasswordModel.frequency= await Contains(response.Content, sha1Suffix);
                    pwnedPasswordModel.isPwned = (pwnedPasswordModel.frequency >= MinimumFrequencyToConsiderPwned);

                    return pwnedPasswordModel;
                }
                Console.WriteLine("Unexpected response from API: {StatusCode}", response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return pwnedPasswordModel;
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
    }
}
