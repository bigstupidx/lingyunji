#if !USE_HOT
using System;
using System.Text;

namespace xys.hot.RPC
{
    public static class ComputeSize
    {
        private const int LittleEndian64Size = 8;
        private const int LittleEndian32Size = 4;        

        public static int ComputeDoubleSize(double value)
        {
            return LittleEndian64Size;
        }

        public static int ComputeFloatSize(float value)
        {
            return LittleEndian32Size;
        }

        public static int ComputeUInt64Size(ulong value)
        {
            return wProtobuf.ComputeSize.ComputeUInt64Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// int64 field, including the tag.
        /// </summary>
        public static int ComputeInt64Size(long value)
        {
            return wProtobuf.ComputeSize.ComputeRawVarint64Size((ulong)value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// int32 field, including the tag.
        /// </summary>
        public static int ComputeInt32Size(int value)
        {
            return wProtobuf.ComputeSize.ComputeInt32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// fixed64 field, including the tag.
        /// </summary>
        public static int ComputeFixed64Size(ulong value)
        {
            return LittleEndian64Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// fixed32 field, including the tag.
        /// </summary>
        public static int ComputeFixed32Size(uint value)
        {
            return LittleEndian32Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// bool field, including the tag.
        /// </summary>
        public static int ComputeBoolSize(bool value)
        {
            return 1;
        }

        public static int ComputeStringSize(String value)
        {
            return wProtobuf.ComputeSize.ComputeStringSize(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// embedded message field, including the tag.
        /// </summary>
        public static int ComputeMessageSize(IMessage value)
        {
            int size = value.CalculateSize();
            return ComputeLengthSize(size) + size;
        }

        public static int ComputeMessageSize(wProtobuf.IMessage value)
        {
            return wProtobuf.ComputeSize.ComputeMessageSize(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// bytes field, including the tag.
        /// </summary>
        public static int ComputeBytesSize(wProtobuf.ByteString value)
        {
            return wProtobuf.ComputeSize.ComputeBytesSize(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// uint32 field, including the tag.
        /// </summary>
        public static int ComputeUInt32Size(uint value)
        {
            return wProtobuf.ComputeSize.ComputeUInt32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// enum field, including the tag. The caller is responsible for
        /// converting the enum value to its numeric value.
        /// </summary>
        public static int ComputeEnumSize(int value)
        {
            // Currently just a pass-through, but it's nice to separate it logically.
            return wProtobuf.ComputeSize.ComputeInt32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sfixed32 field, including the tag.
        /// </summary>
        public static int ComputeSFixed32Size(int value)
        {
            return LittleEndian32Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sfixed64 field, including the tag.
        /// </summary>
        public static int ComputeSFixed64Size(long value)
        {
            return LittleEndian64Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sint32 field, including the tag.
        /// </summary>
        public static int ComputeSInt32Size(int value)
        {
            return wProtobuf.ComputeSize.ComputeSInt32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sint64 field, including the tag.
        /// </summary>
        public static int ComputeSInt64Size(long value)
        {
            return wProtobuf.ComputeSize.ComputeSInt64Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a length,
        /// as written by <see cref="WriteLength"/>.
        /// </summary>
        public static int ComputeLengthSize(int length)
        {
            return wProtobuf.ComputeSize.ComputeLengthSize(length);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a tag.
        /// </summary>
        public static int ComputeTagSize(int fieldNumber)
        {
            return wProtobuf.ComputeSize.ComputeTagSize(fieldNumber);
        }
    }
}
#endif