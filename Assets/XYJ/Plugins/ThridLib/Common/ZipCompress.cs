using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib;

public class ZipCompress
{
    static Deflater deflater_ = null;
    static byte[] buffer_ = null;

    static public int compress(byte[] data, int offset, int count, Network.BitStream stream)
    {
        if (deflater_ == null)
        {
            deflater_ = new Deflater();
            buffer_ = new byte[512];
        }

        deflater_.Reset();
        deflater_.SetInput(data, offset, count);

        int deflateCount = 0;
        int size = 0;
        while (!deflater_.IsNeedingInput)
        {
            deflateCount = deflater_.Deflate(buffer_, 0, buffer_.Length);
            if (deflateCount <= 0)
            {
                break;
            }

            size += deflateCount;
            stream.Write(buffer_, 0, deflateCount);
        }

        if (!deflater_.IsNeedingInput)
        {
            throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
        }

        deflater_.Finish();
        deflateCount = 0;
        while (!deflater_.IsFinished)
        {
            deflateCount = deflater_.Deflate(buffer_, 0, buffer_.Length);
            if (deflateCount <= 0)
                break;

            size += deflateCount;
            stream.Write(buffer_, 0, deflateCount);
        }

        if (!deflater_.IsFinished)
        {
            throw new SharpZipBaseException("Can't deflate all input?");
        }

        XYJLogger.LogDebug("compress src:{0} compress:{1}", count, size);
        return size;
    }

    // 压缩
    static public byte[] compress(byte[] data, int offset, int count)
    {
        MemoryStream stream = new MemoryStream();
        DeflaterOutputStream outStream = new DeflaterOutputStream(stream);
        outStream.Write(data, offset, count);
        outStream.Finish();
        outStream.Close();
        return stream.ToArray();
    }

    // 解压缩
    static public byte[] decompress(byte[] data, int offset, int count)
    {
        Network.BitStream stream = decompress_stream(data, offset, count);
        byte[] buffer = new byte[stream.WritePos];
        System.Array.Copy(stream.Buffer, 0, buffer, 0, stream.WritePos);
        return buffer;
    }

    // 解压缩
    static public Network.BitStream decompress_stream(byte[] data, int offset, int count)
    {
        Inflater inflater = new Inflater();
        inflater.SetInput(data, offset, count);

        int read = 0;
        Network.BitStream stream = new Network.BitStream(count);
        while ((read = inflater.Inflate(stream.Buffer, stream.WritePos, stream.WriteRemain)) != 0)
        {
            stream.WritePos += read;
            if (inflater.IsFinished)
                break;
            stream.ensureCapacity(count);
        }

        return stream;
    }

    // 解压缩
    static public void decompress_stream(byte[] data, int offset, int count, Network.BitStream stream)
    {
        Inflater inflater = new Inflater();
        inflater.SetInput(data, offset, count);

        int read = 0;
        while ((read = inflater.Inflate(stream.Buffer, stream.WritePos, stream.WriteRemain)) != 0)
        {
            stream.WritePos += read;
            if (inflater.IsFinished)
                break;
            stream.ensureCapacity(count);
        }
    }
}