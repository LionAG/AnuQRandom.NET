using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnuQRandom.Entities
{
    public class RequestEntity
    {
        public RequestedDataType DataType { get; set; } = RequestedDataType.uint8;
        
        /// <summary>
        /// Range limits <1, 1024>
        /// </summary>
        public int ArrayLength { get; set; } = 0xA;

        /// <summary>
        /// Range limits - old API: <1, 1024> ; new API: <1, 10>
        /// </summary>
        public int BlockSize { get; set; } = 0xA;
    
        public RequestEntity WithDataType(RequestedDataType requestedDataType)
        {
            DataType = requestedDataType;
            return this;
        }

        public RequestEntity WithArrayLength(int arrayLength)
        {
            ArrayLength = arrayLength;
            return this;
        }

        public RequestEntity WithBlockSize(int blockSize)
        {
            BlockSize = blockSize;
            return this;
        }
    }
}
