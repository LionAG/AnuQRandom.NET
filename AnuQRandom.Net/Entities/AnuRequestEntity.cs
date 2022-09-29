using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnuQRandom.Entities
{
    public class AnuRequestEntity
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
    
        public AnuRequestEntity WithDataType(RequestedDataType requestedDataType)
        {
            DataType = requestedDataType;
            return this;
        }

        public AnuRequestEntity WithArrayLength(int arrayLength)
        {
            ArrayLength = arrayLength;
            return this;
        }

        public AnuRequestEntity WithBlockSize(int blockSize)
        {
            BlockSize = blockSize;
            return this;
        }
    }
}
