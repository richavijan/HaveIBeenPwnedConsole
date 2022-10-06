// See https://aka.ms/new-console-template for more information
using System.Security;
using HaveIBeenPwnedConsole;

Console.WriteLine("Please enter the password to check..");
SecureString maskedPassword = MaskInputValue.maskInput();
string password = new System.Net.NetworkCredential(string.Empty, maskedPassword).Password;
Console.WriteLine(""); //Adding extra line

CheckPwnedPassword checkPwnedPassword = new CheckPwnedPassword();
PwnedPasswordModel pwnedPasswordModel = await checkPwnedPassword.HasPasswordBeenPwned(password);

if (pwnedPasswordModel.isPwned)
{
    Console.WriteLine($"HaveIBeenPwned API indicates the password has been pwned { pwnedPasswordModel.frequency } times");
}
else
{
    Console.WriteLine("HaveIBeenPwned API indicates the password has not been pwned");
}