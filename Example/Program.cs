namespace Example
{
	public static class Program
	{
		public static async Task GetNumbers_NewApi()
		{
            var client = new AnuQRandom.NewApiClient("[API KEY]");
            var data = await client.RequestAsync();

            if (data != null && data.Success)
            {
                foreach (var number in data.Data)
                    Console.WriteLine(number);
            }
        }

        public static async Task GetNumbers_OldApi()
        {
            var client = new AnuQRandom.OldApiClient()
            {
                arrayLength = 0x32 // Get 50 numbers
            };

            var data = await client.RequestAsync();

            if (data != null && data.Success)
            {
                foreach (var number in data.Data)
                    Console.WriteLine(number);
            }
        }

        public static async Task DirtyClientExample()
        {
            var client = new AnuQRandom.DirtyClient();

            // Save a random 256 x 256 scatter plot image in the current application working directory.

            await client.GetScatterPlotImageAsync("scatter_plot.png");

            var hexBlock = await client.GetHexadecimalBlockAsync();
            var binBlock = await client.GetBinaryBlockAsync();
            var alpBlock = await client.GetAlphanumericBlockAsync();

            Console.WriteLine("Hexadecimal block:\n\n");
            Console.WriteLine(hexBlock);

            Console.WriteLine("\n\nBinary block:\n\n");
            Console.WriteLine(binBlock);

            Console.WriteLine("\n\nAlphanumeric block:\n\n");
            Console.WriteLine(alpBlock);
        }

        public static void Main() => GetNumbers_OldApi().GetAwaiter().GetResult();
	}
}


