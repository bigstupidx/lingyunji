#if UNITY_EDITOR || USE_RESOURCESEXPORT
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace PackTool
{
    public class StreamDataSource : IStaticDataSource
    {
        private Stream _stream;

        public StreamDataSource(Stream stream)
        {
            _stream = stream;
        }

        ~StreamDataSource()
        {
            if (_stream != null)
                _stream.Close();
        }

        public Stream GetSource()
        {
            return _stream;
        }
    }
}
#endif