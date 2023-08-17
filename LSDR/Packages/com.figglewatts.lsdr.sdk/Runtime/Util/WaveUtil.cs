using System.IO;
using System.Text;

namespace LSDR.SDK.Util
{
    public class WaveUtil
    {
        public static void WriteWave(string fileName, short[] samples, int channels, int sampleRate)
        {
            using var fs = new FileStream(fileName, FileMode.Create);
            using var bw = new BinaryWriter(fs);

            int fmtChunkSize = 16;
            int dataChunkSize = samples.Length * 2; // 2 bytes per sample
            int riffChunkSize = 4 + 8 + fmtChunkSize + 8 + dataChunkSize;

            // write RIFF header
            bw.Write(Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(riffChunkSize);
            bw.Write(Encoding.ASCII.GetBytes("WAVE"));

            // write fmt chunk
            bw.Write(Encoding.ASCII.GetBytes("fmt "));
            bw.Write(fmtChunkSize);
            bw.Write((short)1);                  // audio format, 1 = PCM
            bw.Write((short)channels);           // channels
            bw.Write(sampleRate);                // sample rate
            bw.Write(sampleRate * channels * 2); // byte rate
            bw.Write((short)(channels * 2));     // block align
            bw.Write((short)16);                 // bits per sample

            // write data chunk
            bw.Write(Encoding.ASCII.GetBytes("data"));
            bw.Write(dataChunkSize);
            foreach (var sample in samples)
            {
                bw.Write(sample);
            }
        }
    }
}
