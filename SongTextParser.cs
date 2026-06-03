using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MusicSchoolWpf
{
    public class ParsedLyricLine
    {
        public int Placement { get; set; }
        public string ChordName { get; set; }
        public string Text { get; set; }
    }

    public static class SongTextParser
    {
        public static List<ParsedLyricLine> Parse(string rawText)
        {
            List<ParsedLyricLine> result = new List<ParsedLyricLine>();

            if (string.IsNullOrWhiteSpace(rawText))
                return result;

            string[] lines = rawText
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n');

            int placement = 1;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                List<ParsedLyricLine> parsedFromLine = ParseSingleLine(line);

                foreach (ParsedLyricLine item in parsedFromLine)
                {
                    item.Placement = placement++;
                    result.Add(item);
                }
            }

            return result;
        }

        private static List<ParsedLyricLine> ParseSingleLine(string line)
        {
            List<ParsedLyricLine> result = new List<ParsedLyricLine>();

            MatchCollection matches = Regex.Matches(line, @"\[(?<chord>[^\]]+)\]");

            if (matches.Count == 0)
            {
                result.Add(new ParsedLyricLine
                {
                    ChordName = "",
                    Text = line
                });

                return result;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Match currentMatch = matches[i];

                string chordName = currentMatch.Groups["chord"].Value.Trim();

                int textStart = currentMatch.Index + currentMatch.Length;
                int textEnd = (i + 1 < matches.Count)
                    ? matches[i + 1].Index
                    : line.Length;

                string text = line.Substring(textStart, textEnd - textStart).Trim();

                if (string.IsNullOrWhiteSpace(text))
                    text = " ";

                result.Add(new ParsedLyricLine
                {
                    ChordName = chordName,
                    Text = text
                });
            }

            return result;
        }
    }
}