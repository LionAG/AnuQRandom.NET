using System.Drawing;
using System.Text;

namespace AnuQRandom
{
    /// <summary>
    /// Use this class to access the Blocks, Fun Stuff etc. sections on qrng.anu.edu.au
    /// </summary>
    public class DirtyClient
    {
        public enum BlockType
        {
            Binary,
            Hexadecimal,
            Alphanumeric
        }

        public enum LiveStreamType
        {
            Color,
            Binary,
            Hexadecimal
        }

        public enum SensingRandomnessType
        {
            BWPixels,
            ScatterPlot,
            WhiteNoise,
            BernoulliNoise
        }

        private string PluginBase => "https://qrng.anu.edu.au/wp-content/plugins/colours-plugin/";

        protected readonly HttpClient ReqClient;

        private async Task<bool> DecodeAndSaveFile(byte[] data, string fileName)
        {
            var textData = Encoding.ASCII.GetString(data);
            textData = textData.Remove(0, textData.IndexOf(',') + 1);

            var decodedData = Convert.FromBase64String(textData);

            await File.WriteAllBytesAsync(fileName, decodedData);

            return true;
        }

        public async Task<string> RequestLiveStreamAsync(LiveStreamType liveStreamType)
        {
            switch (liveStreamType)
            {
                case LiveStreamType.Color: return await ReqClient.GetStringAsync($"{PluginBase}get_one_colour.php");
                case LiveStreamType.Hexadecimal: return await ReqClient.GetStringAsync($"{PluginBase}get_one_hex.php");
                case LiveStreamType.Binary: return await ReqClient.GetStringAsync($"{PluginBase}get_one_binary.php");
                default: return "";
            }
        }
        public async Task<byte[]> RequestSensingRandomnessAsync(SensingRandomnessType sensingRandomnessType)
        {
            using var memoryStream = new MemoryStream();

            switch (sensingRandomnessType)
            {
                case SensingRandomnessType.BWPixels:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_image_bw.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
                case SensingRandomnessType.ScatterPlot:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_image_scatter.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
                case SensingRandomnessType.WhiteNoise:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_audio_whiteNoise.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
                case SensingRandomnessType.BernoulliNoise:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_audio_bernoulliNoise.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
            }

            return memoryStream.ToArray();
        }
        public async Task<byte[]> RequestBlockAsync(BlockType blockType)
        {
            using var memoryStream = new MemoryStream();

            switch (blockType)
            {
                case BlockType.Binary:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_block_binary.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
                case BlockType.Hexadecimal:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_block_hex.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
                case BlockType.Alphanumeric:
                    {
                        await ReqClient.GetStreamAsync($"{PluginBase}get_block_alpha.php").ContinueWith(s => s.Result.CopyToAsync(memoryStream));
                        break;
                    }
            }

            return memoryStream.ToArray();
        }

        public byte[] RequestBlock(BlockType blockType)
        {
            return RequestBlockAsync(blockType).Result;
        }
        public byte[] RequestSensingRandomness(SensingRandomnessType sensingRandomnessType)
        {
            return RequestSensingRandomnessAsync(sensingRandomnessType).Result;
        }
        public string RequestLiveStream(LiveStreamType liveStreamType)
        {
            return RequestLiveStreamAsync(liveStreamType).Result;
        }

        /// <summary>
        /// Random colour
        /// </summary>
        /// <returns></returns>
        public async Task<Color> GetColorAsync()
        {
            var colorData = await RequestLiveStreamAsync(LiveStreamType.Color);
            var colorNumber = Convert.ToInt32(colorData, 0x10);

            return Color.FromArgb(colorNumber);
        }

        /// <summary>
        /// Random hex
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHexadecimalAsync()
        {
            return await RequestLiveStreamAsync(LiveStreamType.Hexadecimal);
        }

        /// <summary>
        /// Random binary
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetBinaryAsync()
        {
            return await RequestLiveStreamAsync(LiveStreamType.Binary);
        }

        /// <summary>
        /// Random alphanumeric characters (1024 random alphanumeric (and underscore) characters.)
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAlphanumericBlockAsync()
        {
            var blockData = await RequestBlockAsync(BlockType.Alphanumeric);
            return Encoding.ASCII.GetString(blockData);
        }

        /// <summary>
        /// Random block hex
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHexadecimalBlockAsync()
        {
            var blockData = await RequestBlockAsync(BlockType.Hexadecimal);
            return Encoding.ASCII.GetString(blockData);
        }

        /// <summary>
        /// Random binary bits
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetBinaryBlockAsync()
        {
            var blockData = await RequestBlockAsync(BlockType.Binary);
            return Encoding.ASCII.GetString(blockData);
        }

        /// <summary>
        /// Generate m permutations of N objects.The permutations are obtained using the Fisher–Yates shuffle.
        /// </summary>
        /// <param name="permutationCount">Number of permutations to generate? (1–50)</param>
        /// <param name="objectCount">Number of objects in each permutation? (1–200)</param>
        /// <returns></returns>
        public async Task<List<List<int>>> GetRandomPermutations(int permutationCount, int objectCount)
        {
            if (permutationCount < 1 || permutationCount > 50)
                throw new ArgumentException("Invalid permutation count!");

            if (objectCount < 1 || objectCount > 200)
                throw new ArgumentException("Invalid object count!");

            var formData = new Dictionary<string, string>
            {
                { "set_num", permutationCount.ToString() },
                { "max_num", objectCount.ToString() },
                { "action", "permutation_action" },
                { "permutation_nonce_field", "9cbbdb661e" }
            };

            using HttpContent formContent = new FormUrlEncodedContent(formData);

            var response = await ReqClient.PostAsync("https://qrng.anu.edu.au/wp-admin/admin-ajax.php", formContent);
            var data = await response.Content.ReadAsStringAsync();

            data = data.Replace("\"{\\\"type\\\":\\\"success\\\",\\\"output\\\":[[", String.Empty);
            data = data.Replace("]]}\"", string.Empty);
            data = data.Replace("[", string.Empty);
            data = data.Replace("]", string.Empty);

            List<List<int>> permutations = new();

            int permutationCurrentCount = -1;

            for (int numberCount = 0; numberCount < objectCount * permutationCount; numberCount++)
            {
                if (numberCount % objectCount == 0)
                {
                    permutationCurrentCount++;
                    permutations.Add(new List<int>());
                }

                permutations[permutationCurrentCount].Add(data[numberCount]);
            }

            return permutations;
        }

        /// <summary>
        /// Image composed of 256-by-256 random black and white pixels.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task GetBWPixelsImageAsync(string fileName)
        {
            var data = await RequestSensingRandomnessAsync(SensingRandomnessType.BWPixels);

            if (Path.HasExtension(fileName) == false)
                fileName += ".png";

            await DecodeAndSaveFile(data, fileName);
        }

        /// <summary>
        /// Random 256-by-256 pixel scatter plot.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task GetScatterPlotImageAsync(string fileName)
        {
            var data = await RequestSensingRandomnessAsync(SensingRandomnessType.ScatterPlot);

            if (Path.HasExtension(fileName) == false)
                fileName += ".png";

            await DecodeAndSaveFile(data, fileName);
        }

        /// <summary>
        /// Five seconds of white noise.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task GetWhiteNoiseAsync(string fileName)
        {
            var data = await RequestSensingRandomnessAsync(SensingRandomnessType.WhiteNoise);

            if (Path.HasExtension(fileName) == false)
                fileName += ".wav";

            await DecodeAndSaveFile(data, fileName);
        }

        /// <summary>
        /// Each second is divided into 44100 divisions. The probability of a click in each division is 0.03 percent.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task GetBernoulliNoiseAsync(string fileName)
        {
            var data = await RequestSensingRandomnessAsync(SensingRandomnessType.BernoulliNoise);

            if (Path.HasExtension(fileName) == false)
                fileName += ".wav";

            await DecodeAndSaveFile(data, fileName);
        }

        public DirtyClient()
        {
            ReqClient = new();
        }
        ~DirtyClient()
        {

            ReqClient.Dispose();
        }
    }
}
