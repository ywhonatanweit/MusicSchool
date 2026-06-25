using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicSchoolWpf
{
    public static class ChordSoundPlayer
    {
        // האינדקס הוא ה-ID של האקורד.
        // אינדקס 0 נשאר ריק בכוונה.
        private static readonly double[][] chordSounds = new double[36][];

        static ChordSoundPlayer()
        {
            chordSounds[0] = Array.Empty<double>();

            chordSounds[1] = new double[] { 220.00, 277.18, 329.63 };          // A
            chordSounds[2] = new double[] { 220.00, 277.18, 329.63, 392.00 };  // A7
            chordSounds[3] = new double[] { 220.00, 261.63, 329.63 };          // Am
            chordSounds[4] = new double[] { 220.00, 261.63, 329.63, 392.00 };  // Am7
            chordSounds[5] = new double[] { 220.00, 277.18, 329.63, 415.30 };  // Amaj7

            chordSounds[6] = new double[] { 246.94, 311.13, 369.99 };          // B
            chordSounds[7] = new double[] { 246.94, 311.13, 369.99, 440.00 };  // B7
            chordSounds[8] = new double[] { 246.94, 293.66, 369.99 };          // Bm
            chordSounds[9] = new double[] { 246.94, 293.66, 369.99, 440.00 };  // Bm7
            chordSounds[10] = new double[] { 246.94, 311.13, 369.99, 466.16 }; // Bmaj7

            chordSounds[11] = new double[] { 261.63, 329.63, 392.00 };         // C
            chordSounds[12] = new double[] { 261.63, 329.63, 392.00, 466.16 }; // C7
            chordSounds[13] = new double[] { 261.63, 311.13, 392.00 };         // Cm
            chordSounds[14] = new double[] { 261.63, 311.13, 392.00, 466.16 }; // Cm7
            chordSounds[15] = new double[] { 261.63, 329.63, 392.00, 493.88 }; // Cmaj7

            chordSounds[16] = new double[] { 293.66, 369.99, 440.00 };         // D
            chordSounds[17] = new double[] { 293.66, 369.99, 440.00, 523.25 }; // D7
            chordSounds[18] = new double[] { 293.66, 349.23, 440.00 };         // Dm
            chordSounds[19] = new double[] { 293.66, 349.23, 440.00, 523.25 }; // Dm7
            chordSounds[20] = new double[] { 293.66, 369.99, 440.00, 554.37 }; // Dmaj7

            chordSounds[21] = new double[] { 329.63, 415.30, 493.88 };         // E
            chordSounds[22] = new double[] { 329.63, 415.30, 493.88, 587.33 }; // E7
            chordSounds[23] = new double[] { 329.63, 392.00, 493.88 };         // Em
            chordSounds[24] = new double[] { 329.63, 392.00, 493.88, 587.33 }; // Em7
            chordSounds[25] = new double[] { 329.63, 415.30, 493.88, 622.25 }; // Emaj7

            chordSounds[26] = new double[] { 349.23, 440.00, 523.25 };         // F
            chordSounds[27] = new double[] { 349.23, 440.00, 523.25, 622.25 }; // F7
            chordSounds[28] = new double[] { 349.23, 415.30, 523.25 };         // Fm
            chordSounds[29] = new double[] { 349.23, 415.30, 523.25, 622.25 }; // Fm7
            chordSounds[30] = new double[] { 349.23, 440.00, 523.25, 659.25 }; // Fmaj7

            chordSounds[31] = new double[] { 196.00, 246.94, 293.66 };         // G
            chordSounds[32] = new double[] { 196.00, 246.94, 293.66, 349.23 }; // G7
            chordSounds[33] = new double[] { 196.00, 233.08, 293.66 };         // Gm
            chordSounds[34] = new double[] { 196.00, 233.08, 293.66, 349.23 }; // Gm7
            chordSounds[35] = new double[] { 196.00, 246.94, 293.66, 369.99 }; // Gmaj7
        }

        public static void PlayChordById(int chordId)
        {
            if (chordId <= 0 || chordId >= chordSounds.Length)
                return;

            double[] notes = chordSounds[chordId];

            if (notes == null || notes.Length == 0)
                return;

            PlayNotes(notes, 650, 2);
        }

        private static void PlayNotes(double[] frequencies, int durationMs, double volume)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WaveOutEvent output = new WaveOutEvent())
                    {
                        ChordProvider provider = new ChordProvider(frequencies, durationMs, volume);

                        output.Init(provider);
                        output.Play();

                        Thread.Sleep(durationMs + 80);

                        output.Stop();
                    }
                }
                catch
                {
                }
            });
        }

        private class ChordProvider : ISampleProvider
        {
            private const int SampleRate = 44100;

            private readonly double[] frequencies;
            private readonly int totalSamples;
            private readonly double volume;
            private int sampleIndex = 0;

            public WaveFormat WaveFormat { get; }

            public ChordProvider(double[] frequencies, int durationMs, double volume)
            {
                this.frequencies = frequencies;
                this.volume = volume;
                totalSamples = SampleRate * durationMs / 1000;
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                int samplesWritten = 0;

                for (int i = 0; i < count; i++)
                {
                    if (sampleIndex >= totalSamples)
                        break;

                    double time = (double)sampleIndex / SampleRate;
                    double sample = 0;

                    foreach (double freq in frequencies)
                    {
                        sample += Math.Sin(2 * Math.PI * freq * time);
                    }

                    sample /= frequencies.Length;

                    double progress = (double)sampleIndex / totalSamples;

                    double fadeIn = Math.Min(1.0, progress / 0.05);
                    double fadeOut = 1.0 - progress;
                    double envelope = fadeIn * fadeOut;

                    buffer[offset + i] = (float)(sample * volume * envelope);

                    sampleIndex++;
                    samplesWritten++;
                }

                return samplesWritten;
            }
        }
    }
}