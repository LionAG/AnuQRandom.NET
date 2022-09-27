# AnuQRandom.Net
A .NET 6 library for accessing the ANU quantum random number generator API located at: https://qrng.anu.edu.au

How to use:

```C#
var client = new AnuQRandom.NewClient("[API KEY]");
var data = await client.RequestAsync();

if (data != null && data.Success)
{
	foreach (var number in data.Data)
	{
		Console.WriteLine(number);
	}
}

Console.WriteLine("Finished!");

```
