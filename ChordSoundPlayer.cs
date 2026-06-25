using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicSchoolWpf
{
    public static class ChordSoundPlayer
    {
        // כל אינדקס הוא chordId.
        // בכל אקורד יש 6 תדרים, אחד לכל מיתר.
        // 0 אומר: לא לנגן את המיתר הזה.
        private static readonly double[][] chordStrings = new double[36][];

        static ChordSoundPlayer()
        {
            chordStrings[0] = new double[] { 0, 0, 0, 0, 0, 0 };

            // סדר המיתרים:
            // מיתר 6, מיתר 5, מיתר 4, מיתר 3, מיתר 2, מיתר 1
            // כלומר: מלמעלה למטה בגיטרה

            chordStrings[1] = new double[] { 0, 220.00, 329.63, 440.00, 554.37, 659.25 };       // A
            chordStrings[2] = new double[] { 0, 220.00, 329.63, 392.00, 554.37, 659.25 };       // A7
            chordStrings[3] = new double[] { 0, 220.00, 329.63, 440.00, 523.25, 659.25 };       // Am
            chordStrings[4] = new double[] { 0, 220.00, 329.63, 392.00, 523.25, 659.25 };       // Am7
            chordStrings[5] = new double[] { 0, 220.00, 329.63, 415.30, 554.37, 659.25 };       // Amaj7

            chordStrings[6] = new double[] { 0, 246.94, 369.99, 493.88, 622.25, 739.99 };      // B
            chordStrings[7] = new double[] { 0, 246.94, 369.99, 440.00, 622.25, 739.99 };      // B7
            chordStrings[8] = new double[] { 0, 246.94, 369.99, 493.88, 587.33, 739.99 };      // Bm
            chordStrings[9] = new double[] { 0, 246.94, 369.99, 440.00, 587.33, 739.99 };      // Bm7
            chordStrings[10] = new double[] { 0, 246.94, 369.99, 466.16, 622.25, 739.99 };     // Bmaj7

            chordStrings[11] = new double[] { 0, 0, 261.63, 329.63, 392.00, 523.25 };          // C
            chordStrings[12] = new double[] { 0, 0, 261.63, 329.63, 466.16, 523.25 };          // C7
            chordStrings[13] = new double[] { 0, 0, 261.63, 311.13, 392.00, 523.25 };          // Cm
            chordStrings[14] = new double[] { 0, 0, 261.63, 311.13, 466.16, 523.25 };          // Cm7
            chordStrings[15] = new double[] { 0, 0, 261.63, 329.63, 493.88, 523.25 };          // Cmaj7

            chordStrings[16] = new double[] { 0, 0, 293.66, 440.00, 587.33, 739.99 };          // D
            chordStrings[17] = new double[] { 0, 0, 293.66, 440.00, 523.25, 739.99 };          // D7
            chordStrings[18] = new double[] { 0, 0, 293.66, 440.00, 587.33, 698.46 };          // Dm
            chordStrings[19] = new double[] { 0, 0, 293.66, 440.00, 523.25, 698.46 };          // Dm7
            chordStrings[20] = new double[] { 0, 0, 293.66, 440.00, 554.37, 739.99 };          // Dmaj7

            chordStrings[21] = new double[] { 82.41, 123.47, 164.81, 207.65, 246.94, 329.63 }; // E
            chordStrings[22] = new double[] { 82.41, 123.47, 164.81, 196.00, 246.94, 329.63 }; // E7
            chordStrings[23] = new double[] { 82.41, 123.47, 164.81, 196.00, 246.94, 329.63 }; // Em
            chordStrings[24] = new double[] { 82.41, 123.47, 164.81, 196.00, 246.94, 293.66 }; // Em7
            chordStrings[25] = new double[] { 82.41, 123.47, 164.81, 207.65, 246.94, 311.13 }; // Emaj7

            chordStrings[26] = new double[] { 87.31, 130.81, 174.61, 220.00, 261.63, 349.23 }; // F
            chordStrings[27] = new double[] { 87.31, 130.81, 174.61, 207.65, 261.63, 349.23 }; // F7
            chordStrings[28] = new double[] { 87.31, 130.81, 174.61, 207.65, 261.63, 349.23 }; // Fm
            chordStrings[29] = new double[] { 87.31, 130.81, 174.61, 207.65, 261.63, 311.13 }; // Fm7
            chordStrings[30] = new double[] { 87.31, 130.81, 174.61, 220.00, 261.63, 329.63 }; // Fmaj7

            chordStrings[31] = new double[] { 98.00, 123.47, 196.00, 246.94, 293.66, 392.00 }; // G
            chordStrings[32] = new double[] { 98.00, 123.47, 196.00, 246.94, 293.66, 349.23 }; // G7
            chordStrings[33] = new double[] { 98.00, 116.54, 196.00, 233.08, 293.66, 392.00 }; // Gm
            chordStrings[34] = new double[] { 98.00, 116.54, 196.00, 233.08, 293.66, 349.23 }; // Gm7
            chordStrings[35] = new double[] { 98.00, 123.47, 196.00, 246.94, 293.66, 369.99 }; // Gmaj7
        }

        public static void PlayChordById(int chordId)
        {
            if (chordId <= 0 || chordId >= chordStrings.Length)
                return;

            double[] strings = chordStrings[chordId];

            if (strings == null || strings.Length == 0)
                return;

            PlayStrum(strings, 700, 0.55, 35);
        }

        private static void PlayStrum(double[] strings, int durationMs, double volume, int delayBetweenStringsMs)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WaveOutEvent output = new WaveOutEvent())
                    {
                        StrumProvider provider = new StrumProvider(
                            strings,
                            durationMs,
                            volume,
                            delayBetweenStringsMs
                        );

                        output.Init(provider);
                        output.Play();

                        Thread.Sleep(durationMs + delayBetweenStringsMs * 6 + 100);

                        output.Stop();
                    }
                }
                catch
                {
                }
            });
        }

        private class StrumProvider : ISampleProvider
        {
            private const int SampleRate = 44100;

            private readonly double[] strings;
            private readonly int totalSamples;
            private readonly double volume;
            private readonly int delaySamples;
            private int sampleIndex = 0;

            public WaveFormat WaveFormat { get; }

            public StrumProvider(double[] strings, int durationMs, double volume, int delayBetweenStringsMs)
            {
                this.strings = strings;
                this.volume = volume;
                totalSamples = SampleRate * durationMs / 1000;
                delaySamples = SampleRate * delayBetweenStringsMs / 1000;

                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                int samplesWritten = 0;

                for (int i = 0; i < count; i++)
                {
                    if (sampleIndex >= totalSamples)
                        break;

                    double sample = 0;
                    int activeStrings = 0;

                    for (int stringIndex = 0; stringIndex < strings.Length; stringIndex++)
                    {
                        double freq = strings[stringIndex];

                        if (freq <= 0)
                            continue;

                        int stringStartSample = stringIndex * delaySamples;

                        if (sampleIndex < stringStartSample)
                            continue;

                        int localSampleIndex = sampleIndex - stringStartSample;
                        double time = (double)localSampleIndex / SampleRate;

                        double stringProgress = (double)localSampleIndex / totalSamples;

                        if (stringProgress > 1)
                            continue;

                        double fadeIn = Math.Min(1.0, stringProgress / 0.03);
                        double fadeOut = 1.0 - stringProgress;
                        double envelope = fadeIn * fadeOut;

                        sample += Math.Sin(2 * Math.PI * freq * time) * envelope;
                        activeStrings++;
                    }

                    if (activeStrings > 0)
                        sample /= activeStrings;

                    buffer[offset + i] = (float)(sample * volume);

                    sampleIndex++;
                    samplesWritten++;
                }

                return samplesWritten;
            }
        }
    }
}