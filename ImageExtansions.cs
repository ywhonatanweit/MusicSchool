using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace NoaMedia
{
    public static class ImageExtensions
    {
        public static BitmapImage? ByteToImage(this byte[]? array)
        {
            if (array == null || array.Length == 0)
                return null;

            try
            {
                BitmapImage image = new BitmapImage();

                using (MemoryStream ms = new MemoryStream(array))
                {
                    ms.Position = 0;

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }

                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public static BitmapImage? ToBitmapImage(string? imageValue, string? fallbackPath = null)
        {
            BitmapImage? fromMainValue = TryLoadFromBase64OrPath(imageValue);

            if (fromMainValue != null)
                return fromMainValue;

            return TryLoadFromBase64OrPath(fallbackPath);
        }

        private static BitmapImage? TryLoadFromBase64OrPath(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string cleanValue = value.Trim();

            int commaIndex = cleanValue.IndexOf(',');

            if (cleanValue.StartsWith("data:image", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            {
                cleanValue = cleanValue.Substring(commaIndex + 1);
            }

            BitmapImage? fromBase64 = TryLoadBase64(cleanValue);

            if (fromBase64 != null)
                return fromBase64;

            string? fullPath = FindExistingPath(value.Trim());

            if (fullPath == null)
                return null;

            return TryLoadFile(fullPath);
        }

        private static BitmapImage? TryLoadBase64(string value)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(value);
                return ByteToImage(bytes);
            }
            catch
            {
                return null;
            }
        }

        private static string? FindExistingPath(string path)
        {
            string normalized = path.Replace('/', Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(normalized) && File.Exists(normalized))
                return normalized;

            string fileName = Path.GetFileName(normalized);
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] candidates =
            {
                Path.Combine(baseDir, normalized),
                Path.Combine(baseDir, "IMAGES", normalized),
                Path.Combine(baseDir, "IMAGES", "song_pics", fileName),
                Path.Combine(baseDir, "IMAGES", "chords", fileName),

                Path.Combine(baseDir, "..", "..", "..", normalized),
                Path.Combine(baseDir, "..", "..", "..", "IMAGES", normalized),
                Path.Combine(baseDir, "..", "..", "..", "IMAGES", "song_pics", fileName),
                Path.Combine(baseDir, "..", "..", "..", "IMAGES", "chords", fileName),

                Path.Combine(baseDir, "..", "..", "..", "..", "MusicSchool-project-master", "ViewModel", "pictures", fileName),
                Path.Combine(baseDir, "..", "..", "..", "..", "MusicSchool-project", "ViewModel", "pictures", fileName)
            };

            return candidates
                .Select(Path.GetFullPath)
                .FirstOrDefault(File.Exists);
        }

        private static BitmapImage? TryLoadFile(string fullPath)
        {
            try
            {
                BitmapImage image = new BitmapImage();

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fullPath, UriKind.Absolute);
                image.EndInit();

                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }
    }
}