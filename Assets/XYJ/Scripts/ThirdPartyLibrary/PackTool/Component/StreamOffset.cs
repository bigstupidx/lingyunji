using System;
using System.IO;

namespace PackTool
{
    public class StreamOffset : Stream
    {
        //object lock_ = new object();

        public StreamOffset(Stream stream, long start, long length)
        {
            baseStream_ = stream;

            Reset(start, length);
        }

        public override int ReadByte()
        {
            if (readPos_ >= end_)
            {
                // -1 is the correct value at end of stream.
                return -1;
            }

            readPos_++;
            return baseStream_.ReadByte();
        }

        public override void Close()
        {
            // Do nothing at all!
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > end_ - readPos_)
            {
                count = (int)(end_ - readPos_);
                if (count == 0)
                {
                    return 0;
                }
            }

            int readCount = baseStream_.Read(buffer, offset, count);
            if (readCount > 0)
            {
                readPos_ += readCount;
            }

            return readCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {            
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos = readPos_;

            switch (origin)
            {
            case SeekOrigin.Begin:
                newPos = start_ + offset;
                break;

            case SeekOrigin.Current:
                newPos = readPos_ + offset;
                break;

            case SeekOrigin.End:
                newPos = end_ + offset;
                break;
            }

            if (newPos < start_)
            {
                throw new ArgumentException("Negative position is invalid");
            }

            if (newPos >= end_)
            {
                throw new IOException("Cannot seek past end");
            }

            readPos_ = newPos;
            baseStream_.Seek(readPos_, SeekOrigin.Begin);
            return readPos_;
        }

        public override void Flush()
        {
            // Nothing to do.
        }

        public override long Position
        {
            get { return readPos_ - start_; }

            set
            {
                long newPos = start_ + value;

                if (newPos < start_)
                {
                    throw new ArgumentException("Negative position is invalid");
                }

                if (newPos >= end_)
                {
                    throw new InvalidOperationException("Cannot seek past end");
                }
                readPos_ = newPos;
                baseStream_.Seek(readPos_, SeekOrigin.Begin);
            }
        }

        public override long Length { get { return length_; } }

        public void Reset(long start, long length)
        {
            int start_stream = (int)BaseStream.Position;
            start_ = start + start_stream;
            length_ = length;

            readPos_ = start_stream + start;
            end_ = start_ + length;
        }

        public override bool CanWrite { get { return false; } }

        public override bool CanSeek { get { return true; } }

        public override bool CanRead { get { return true; } }

        public override bool CanTimeout { get { return baseStream_.CanTimeout; } }

        public Stream BaseStream { get { return baseStream_; } }

        public long StartPosition { get { return start_; } }

        Stream baseStream_;

        long start_;
        long length_;
        long readPos_;
        long end_;
    }
}