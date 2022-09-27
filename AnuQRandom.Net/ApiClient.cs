using AnuQRandom.Entities;
using AnuQRandom.Model;
using Newtonsoft.Json;

namespace AnuQRandom
{
    public enum RequestedDataType
    {
        uint8,
        uint16,
        hex16
    }

    public abstract class ApiClient
    {
        protected readonly HttpClient ReqClient;

        protected abstract string ApiEndpoint { get; }

        /// <summary>
        /// Data type, the data type must be ‘uint8’ (returns integers between 0–255), ‘uint16’ (returns integers between 0–65535) or ‘hex16’ (returns hexadecimal characters between 00–ff).
        /// </summary>
        public RequestedDataType DataType { get; set; } = RequestedDataType.uint8;

        public int arrayLength = 0xA;
        public int blockSize = 0xA;

        public abstract int ArrayLength { get; set; }
        public abstract int BlockSize { get; set; }

        protected string GetRequestUrl(RequestEntity requestEntity)
        {
            var typeName = Enum.GetName(DataType);

            switch (DataType)
            {
                case RequestedDataType.uint8: return $"{ApiEndpoint}?length={arrayLength}&type={typeName}";
                case RequestedDataType.uint16: return $"{ApiEndpoint}?length={ArrayLength}&type={typeName}";
                case RequestedDataType.hex16: return $"{ApiEndpoint}?length={ArrayLength}&type={typeName}&size={blockSize}"; ;
                default: return "";
            }
        }

        public virtual async Task<RequestedData?> RequestAsync(RequestEntity? requestEntity = null)
        {
            var jsonData = await ReqClient.GetStringAsync(GetRequestUrl(requestEntity ?? new()
            {
                ArrayLength = this.ArrayLength,
                BlockSize = this.BlockSize,
                DataType = this.DataType
            }));

            if (string.IsNullOrEmpty(jsonData))
            {
                throw new Exception("Cannot download data!");
            }

            return JsonConvert.DeserializeObject<RequestedData>(jsonData);
        }

        public virtual RequestedData? Request()
        {
            return RequestAsync().Result;
        }

        public ApiClient()
        {
            ReqClient = new();
        }
        ~ApiClient()
        {
            ReqClient.Dispose();
        }
    }

    /// <summary>
    /// Use this class to access the old QRNG API located at https://qrng.anu.edu.au
    /// </summary>
    public class OldApiClient : ApiClient
    {
        protected override string ApiEndpoint => "https://qrng.anu.edu.au/API/jsonI.php";

        /// <summary>
        /// Array length, the length of the array to return. Must be between 1–1024.
        /// </summary>
        public override int ArrayLength
        {
            get => arrayLength;

            set
            {
                if (value < 1 || value > 0x400)
                    throw new ArgumentException($"{nameof(ArrayLength)} must be between 1 and 1024!");

                arrayLength = value;
            }
        }

        /// <summary>
        /// Block size, only needed for ‘hex16’ data type. Sets the length of each block. Must be between 1–1024.
        /// </summary>
        public override int BlockSize
        {
            get
            {
                return blockSize;
            }

            set
            {
                if (value < 1 || value > 0x400)
                    throw new ArgumentException($"{nameof(BlockSize)} must be between 1 and 1024!");

                blockSize = value;
            }
        }
    }

    /// <summary>
    /// Use this class to access the new QRNG API located at https://api.quantumnumbers.anu.edu.au
    /// </summary>
    public class NewApiClient : ApiClient
    {
        private string ApiKey { get; init; }
        protected override string ApiEndpoint => "https://api.quantumnumbers.anu.edu.au";

        /// <summary>
        /// Array length, the length of the array to return. Must be between 1-1024.
        /// </summary>
        public override int ArrayLength
        {
            get => arrayLength;
            set
            {
                if (value < 1 || value > 0x400)
                    throw new ArgumentException($"{nameof(ArrayLength)} must be between 1 and 1024!");

                arrayLength = value;
            }
        }

        /// <summary>
        /// Block size, only needed for 'hex8' and 'hex16' data types. Sets the length of each block. Must be between 1-10.
        /// </summary>
        public override int BlockSize
        {
            get => blockSize;

            set
            {
                if (value < 1 || value > 0xA)
                    throw new ArgumentException($"{nameof(BlockSize)} must be between 1 and 10!");

                blockSize = value;
            }
        }

        public override async Task<RequestedData?> RequestAsync(RequestEntity? requestEntity = null)
        {
            var reqMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(GetRequestUrl(requestEntity ?? new()
                {
                    ArrayLength = this.ArrayLength,
                    DataType = this.DataType,
                    BlockSize = this.BlockSize
                })),

                Method = HttpMethod.Get,
            };

            reqMessage.Headers.Add("x-api-key", ApiKey);

            var response = await ReqClient.SendAsync(reqMessage);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RequestedData>(content);
        }

        public NewApiClient(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}