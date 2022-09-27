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
