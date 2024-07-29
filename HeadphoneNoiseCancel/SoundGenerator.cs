using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeadphoneNoiseCancel
{
    internal class SoundGenerator
    {
        private int sampleRate;
        private static string audioDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio");
        private static string filePath = Path.Combine(audioDirectory, "sound.wav");

        public SoundGenerator(int sampleRate = 44100)
        {
            this.sampleRate = sampleRate;
        }

        public bool FileExist() {

            if (File.Exists(filePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GenerateAndPlaySound(double frequency, int duration)
        {
            byte[] waveData = GenerateSineWave(frequency, duration, sampleRate);

            
            if (!Directory.Exists(audioDirectory))
            {
                Directory.CreateDirectory(audioDirectory);
            }

            
            File.WriteAllBytes(filePath, waveData);

            using (SoundPlayer player = new SoundPlayer(filePath))
            {
                player.Play();
            }

            Console.WriteLine($"A hangfájl létrejött és lejátszásra került: {filePath}");
        }
                
        

        private byte[] GenerateSineWave(double frequency, int duration, int sampleRate)
        {
            int samples = duration * sampleRate;
            int bytesPerSample = 2;
            int byteRate = sampleRate * bytesPerSample;
            int dataSize = samples * bytesPerSample;
            int fileSize = 44 + dataSize;

            byte[] waveData = new byte[fileSize];

            Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, waveData, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(fileSize - 8), 0, waveData, 4, 4);
            Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, waveData, 8, 4);

            Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, waveData, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(16), 0, waveData, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((short)1), 0, waveData, 20, 2);
            Buffer.BlockCopy(BitConverter.GetBytes((short)1), 0, waveData, 22, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(sampleRate), 0, waveData, 24, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(byteRate), 0, waveData, 28, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((short)bytesPerSample), 0, waveData, 32, 2);
            Buffer.BlockCopy(BitConverter.GetBytes((short)16), 0, waveData, 34, 2);

            Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes("data"), 0, waveData, 36, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(dataSize), 0, waveData, 40, 4);

            double amplitude = 32760;
            double twoPiF = 2 * Math.PI * frequency;
            for (int i = 0; i < samples; i++)
            {
                short sample = (short)(amplitude * Math.Sin((twoPiF * i) / sampleRate));
                byte[] byteSample = BitConverter.GetBytes(sample);
                waveData[44 + i * 2] = byteSample[0];
                waveData[44 + i * 2 + 1] = byteSample[1];
            }

            return waveData;
        }
    }
}