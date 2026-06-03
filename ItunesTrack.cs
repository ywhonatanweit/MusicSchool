using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MusicSchoolWpf
{
    public class ItunesSearchResponse
    {
        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("results")]
        public List<ItunesTrack> Results { get; set; } = new List<ItunesTrack>();
    }

    public class ItunesTrack
    {
        [JsonPropertyName("trackName")]
        public string? TrackName { get; set; }

        [JsonPropertyName("artistName")]
        public string? ArtistName { get; set; }

        [JsonPropertyName("collectionName")]
        public string? CollectionName { get; set; }

        [JsonPropertyName("primaryGenreName")]
        public string? PrimaryGenreName { get; set; }

        [JsonPropertyName("artworkUrl100")]
        public string? ArtworkUrl100 { get; set; }

        [JsonIgnore]
        public string BestArtworkUrl
        {
            get
            {
                return ArtworkUrl100 ?? "";
            }
        }

        [JsonIgnore]
        public string DisplayTitle
        {
            get
            {
                return $"{TrackName} - {ArtistName}";
            }
        }
    }
}