#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
#pragma warning disable 649,414

namespace NAudio.Wave.WaveFormats
{
    /// <summary>
    /// The WMA wave format. 
    /// May not be much use because WMA codec is a DirectShow DMO not an ACM
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    class WmaWaveFormat : WaveFormat
    {
        short wValidBitsPerSample; // bits of precision 
        int dwChannelMask;       // which channels are present in stream 
        int dwReserved1;
        int dwReserved2;
        short wEncodeOptions;
        short wReserved3;

        public WmaWaveFormat(int sampleRate, int bitsPerSample, int channels)
            : base(sampleRate,bitsPerSample,channels)
        {
            wValidBitsPerSample = (short) bitsPerSample;
            if (channels == 1)
                dwChannelMask = 1;
            else if (channels == 2)
                dwChannelMask = 3;

            // WMAUDIO3 is Pro
            this.waveFormatTag = WaveFormatEncoding.WindowsMediaAudio;
        }
    }
}

#endif