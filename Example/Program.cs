using AnuQRandom;
using AnuQRandom.Entities;

namespace Example
{
    public static class Program
    {
        public static async Task GetNumbers_NewApi()
        {
            var client = new NewAnuClient("[API KEY]");
            var data = await client.RequestAsync();

            if (data.Success)
            {
                foreach (var number in data.Data)
                    Console.WriteLine(number);
            }
        }

        public static async Task GetNumbers_OldApi()
        {
            var client = new OldAnuClient()
            {
                arrayLength = 0x32 // Get 50 numbers
            };

            var data = await client.RequestAsync();

            if (data.Success)
            {
                foreach (var number in data.DataNumbers)
                    Console.WriteLine(number);
            }
        }

        public static async Task GetNumbers_FluentStyleConfiguration()
        {
            var client = new OldAnuClient();

            var data = await client.RequestAsync(new AnuRequestEntity().WithDataType(RequestedDataType.hex16)
                                                                    .WithBlockSize(0xA)
                                                                    .WithArrayLength(0x32));

            if (data.Success)
            {
                foreach (var number in data.Data)
                    Console.WriteLine(number);
            }
        }

        public static async Task DirtyClientExample()
        {
            var client = new DirtyAnuClient();

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


