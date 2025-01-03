using System.Reflection;
using ATL;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sandreas.AudioMetadata;

public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
{
    private readonly Dictionary<Type, HashSet<string>> _ignores;

    public PropertyRenameAndIgnoreSerializerContractResolver()
    {
        _ignores = new Dictionary<Type, HashSet<string>>();
    }

    public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
    {
        if (!_ignores.ContainsKey(type))
            _ignores[type] = new HashSet<string>();

        foreach (var prop in jsonPropertyNames)
            _ignores[type].Add(prop);
    }


    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (IsIgnored(property.DeclaringType, property.PropertyName))
        {
            property.ShouldSerialize = _ => false;
            property.Ignored = true;
        }

        return property;
    }

    private bool IsIgnored(Type? type, string? jsonPropertyName)
    {
        return type == null || jsonPropertyName == null ||
               _ignores.ContainsKey(type) && _ignores[type].Contains(jsonPropertyName);
    }
}

public static class MetadataExtensions
{
    // skip movement in favor of virtual property `Part`
    public static MetadataProperty[] MetadataProperties => Enum.GetValues<MetadataProperty>()
        .ToArray();

    public static Type? GetMetadataPropertyType(this IMetadata metadata, MetadataProperty property) => property switch
    {
        MetadataProperty.Album => typeof(string),
        MetadataProperty.AlbumArtist => typeof(string),
        MetadataProperty.Artist => typeof(string),
        MetadataProperty.Bpm => typeof(int),
        MetadataProperty.ChaptersTableDescription => typeof(string),
        MetadataProperty.Composer => typeof(string),
        MetadataProperty.Comment => typeof(string),
        MetadataProperty.Conductor => typeof(string),
        MetadataProperty.Copyright => typeof(string),
        MetadataProperty.Description => typeof(string),
        MetadataProperty.DiscNumber => typeof(int),
        MetadataProperty.DiscTotal => typeof(int),
        MetadataProperty.EncodedBy => typeof(string),
        MetadataProperty.EncoderSettings => typeof(string),
        MetadataProperty.EncodingTool => typeof(string),
        MetadataProperty.Genre => typeof(string),
        MetadataProperty.Group => typeof(string),
        MetadataProperty.ItunesCompilation => typeof(ItunesCompilation),
        MetadataProperty.ItunesMediaType => typeof(ItunesMediaType),
        MetadataProperty.ItunesPlayGap => typeof(ItunesPlayGap),
        MetadataProperty.LongDescription => typeof(string),
        MetadataProperty.Lyrics => typeof(LyricsInfo),
        MetadataProperty.Part => typeof(string),
        MetadataProperty.Movement => typeof(string),
        MetadataProperty.MovementName => typeof(string),
        MetadataProperty.Narrator => typeof(string),
        MetadataProperty.OriginalAlbum => typeof(string),
        MetadataProperty.OriginalArtist => typeof(string),
        MetadataProperty.Popularity => typeof(int),
        MetadataProperty.Publisher => typeof(string),
        MetadataProperty.PublishingDate => typeof(DateTime),
        MetadataProperty.PurchaseDate => typeof(DateTime),
        MetadataProperty.RecordingDate => typeof(DateTime),
        MetadataProperty.SortTitle => typeof(string),
        MetadataProperty.SortAlbum => typeof(string),
        MetadataProperty.SortArtist => typeof(string),
        MetadataProperty.SortAlbumArtist => typeof(string),
        MetadataProperty.SortComposer => typeof(string),
        MetadataProperty.Subtitle => typeof(string),
        MetadataProperty.Title => typeof(string),
        MetadataProperty.TrackNumber => typeof(int),
        MetadataProperty.TrackTotal => typeof(int),
        MetadataProperty.Chapters => typeof(IList<ChapterInfo>),
        MetadataProperty.EmbeddedPictures => typeof(IList<PictureInfo>),
        MetadataProperty.AdditionalFields => typeof(IDictionary<string, string>),
        _ => null
    };

    public static void RemoveMetadataPropertyValue(this IMetadata metadata, MetadataProperty property) {
        switch(property) {
            case MetadataProperty.Movement:
                metadata.Movement = "";
                break;
            case MetadataProperty.Part:
                metadata.Part = "";
                break;
            default:
                metadata.SetMetadataPropertyValue(property, metadata.GetPropertyValueThatLeadsToRemoval(property));
                break;
        }
    }
    public static object? GetMetadataPropertyValue(this IMetadata metadata, MetadataProperty property) =>
        property switch
        {
            MetadataProperty.Album => metadata.Album,
            MetadataProperty.AlbumArtist => metadata.AlbumArtist,
            MetadataProperty.Artist => metadata.Artist,
            MetadataProperty.Bpm => metadata.Bpm,
            MetadataProperty.ChaptersTableDescription => metadata.ChaptersTableDescription,
            MetadataProperty.Composer => metadata.Composer,
            MetadataProperty.Comment => metadata.Comment,
            MetadataProperty.Conductor => metadata.Conductor,
            MetadataProperty.Copyright => metadata.Copyright,
            MetadataProperty.Description => metadata.Description,
            MetadataProperty.DiscNumber => metadata.DiscNumber,
            MetadataProperty.DiscTotal => metadata.DiscTotal,
            MetadataProperty.EncodedBy => metadata.EncodedBy,
            MetadataProperty.EncoderSettings => metadata.EncoderSettings,
            MetadataProperty.EncodingTool => metadata.EncodingTool,
            MetadataProperty.Genre => metadata.Genre,
            MetadataProperty.Group => metadata.Group,
            MetadataProperty.ItunesCompilation => metadata.ItunesCompilation,
            MetadataProperty.ItunesMediaType => metadata.ItunesMediaType,
            MetadataProperty.ItunesPlayGap => metadata.ItunesPlayGap,
            MetadataProperty.LongDescription => metadata.LongDescription,
            MetadataProperty.Lyrics => metadata.Lyrics,
            MetadataProperty.Part => metadata.Part,
            MetadataProperty.Movement => metadata.Movement,
            MetadataProperty.MovementName => metadata.MovementName,
            MetadataProperty.Narrator => metadata.Narrator,
            MetadataProperty.OriginalAlbum => metadata.OriginalAlbum,
            MetadataProperty.OriginalArtist => metadata.OriginalArtist,
            MetadataProperty.Popularity => metadata.Popularity,
            MetadataProperty.Publisher => metadata.Publisher,
            MetadataProperty.PublishingDate => metadata.PublishingDate,
            MetadataProperty.PurchaseDate => metadata.PurchaseDate,
            MetadataProperty.RecordingDate => metadata.RecordingDate,
            MetadataProperty.SortTitle => metadata.SortTitle,
            MetadataProperty.SortAlbum => metadata.SortAlbum,
            MetadataProperty.SortArtist => metadata.SortArtist,
            MetadataProperty.SortAlbumArtist => metadata.SortAlbumArtist,
            MetadataProperty.SortComposer => metadata.SortComposer,
            MetadataProperty.Subtitle => metadata.Subtitle,
            MetadataProperty.Title => metadata.Title,
            MetadataProperty.TrackNumber => metadata.TrackNumber,
            MetadataProperty.TrackTotal => metadata.TrackTotal,
            MetadataProperty.Chapters => metadata.Chapters,
            MetadataProperty.EmbeddedPictures => metadata.EmbeddedPictures,
            MetadataProperty.AdditionalFields => metadata.AdditionalFields,
            _ => null
        };

    public static void SetMetadataPropertyValue(this IMetadata metadata, MetadataProperty property, string? value,
        Type type)
    {
        var valueToSet = value == null
            ? metadata.GetPropertyValueThatLeadsToRemoval(property)
            : ConvertStringToType(value, type);
        metadata.SetMetadataPropertyValue(property, valueToSet);
    }


    /// <summary>
    /// The curent atldotnet API in some cases  does not allow to remove a value by setting it to null
    /// This method determines the required value to remove it, in the future this method may be removed by just using
    /// null as value
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private static object? GetPropertyValueThatLeadsToRemoval(this IMetadata metadata, MetadataProperty property)
    {
        if (metadata.GetMetadataPropertyType(property) == typeof(string))
        {
            return "";
        }

        if (metadata.GetMetadataPropertyType(property) == typeof(int))
        {
            return 0;
        }

        if (metadata.GetMetadataPropertyType(property) == typeof(DateTime))
        {
            return DateTime.MinValue;
        }

        if (metadata.GetMetadataPropertyType(property) == typeof(IList<ChapterInfo>))
        {
            return new List<ChapterInfo>();
        }

        if (metadata.GetMetadataPropertyType(property) == typeof(IList<PictureInfo>))
        {
            return new List<PictureInfo>();
        }

        if (metadata.GetMetadataPropertyType(property) == typeof(IDictionary<string, string>))
        {
            return new Dictionary<string, string>();
        }

        return null;
    }

    private static object? ConvertStringToType(string value, Type type) => type switch
    {
        _ when type == typeof(string) => value,
        _ when type == typeof(DateTime) => TryParseDateTime(value, out var dateTime) ? dateTime : null,
        _ when type == typeof(int) => int.TryParse(value, out var i) ? i : null,
        _ when type == typeof(ItunesCompilation) => Enum.TryParse(value, out ItunesCompilation i) ? i : null,
        _ when type == typeof(ItunesMediaType) => Enum.TryParse(value, out ItunesMediaType i) ? i : null,
        _ when type == typeof(ItunesPlayGap) => Enum.TryParse(value, out ItunesPlayGap i) ? i : null,
        _ when type == typeof(LyricsInfo) => string.IsNullOrWhiteSpace(value)
            ? null
            : new LyricsInfo { ContentType = LyricsInfo.LyricsType.LYRICS, UnsynchronizedLyrics = value },
        _ when type == typeof(IList<ChapterInfo>) => null,
        _ when type == typeof(IList<PictureInfo>) => null,
        _ when type == typeof(IDictionary<string, string>) => null,
        _ => null
    };

    private static bool TryParseDateTime(string dateTimeAsString, out DateTime dateTime)
    {
        if (dateTimeAsString.Length == 4 && dateTimeAsString.All(char.IsDigit))
        {
            dateTimeAsString += "-01-01";
        }

        return DateTime.TryParse(dateTimeAsString, out dateTime);
    }

    public static bool UpdateMetadataPropertyValue(this IMetadata metadata, MetadataProperty property, object? value)
    {
        if (EqualityComparer<object?>.Default.Equals(metadata.GetMetadataPropertyValue(property), value))
        {
            return false;
        }

        metadata.SetMetadataPropertyValue(property, value);
        return true;
    }

    public static void SetMetadataPropertyValue(this IMetadata metadata, MetadataProperty property, object? value)
    {
        switch (property)
        {
            case MetadataProperty.Album:
                metadata.Album = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.AlbumArtist:
                metadata.AlbumArtist = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Artist:
                metadata.Artist = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Bpm:
                metadata.Bpm = ObjectAsType<int?>(value);
                break;
            case MetadataProperty.ChaptersTableDescription:
                metadata.ChaptersTableDescription = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Composer:
                metadata.Composer = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Comment:
                metadata.Comment = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Conductor:
                metadata.Conductor = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Copyright:
                metadata.Copyright = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Description:
                metadata.Description = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.DiscNumber:
                metadata.DiscNumber = ObjectAsType<int?>(value);
                break;
            case MetadataProperty.DiscTotal:
                metadata.DiscTotal = ObjectAsType<int?>(value);
                break;
            case MetadataProperty.EncodedBy:
                metadata.EncodedBy = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.EncoderSettings:
                metadata.EncoderSettings = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.EncodingTool:
                metadata.EncodingTool = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Genre:
                metadata.Genre = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Group:
                metadata.Group = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.ItunesCompilation:
                metadata.ItunesCompilation = ObjectAsType<ItunesCompilation?>(value);
                break;
            case MetadataProperty.ItunesMediaType:
                metadata.ItunesMediaType = ObjectAsType<ItunesMediaType?>(value);
                break;
            case MetadataProperty.ItunesPlayGap:
                metadata.ItunesPlayGap = ObjectAsType<ItunesPlayGap?>(value);
                break;
            case MetadataProperty.LongDescription:
                metadata.LongDescription = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Lyrics:
                metadata.Lyrics = ObjectAsType<LyricsInfo>(value);
                break;
            case MetadataProperty.Part:
                metadata.Part = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Movement:
                metadata.Movement = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.MovementName:
                metadata.MovementName = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Narrator:
                metadata.Narrator = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.OriginalAlbum:
                metadata.OriginalAlbum = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.OriginalArtist:
                metadata.OriginalArtist = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Popularity:
                metadata.Popularity = ObjectAsType<float?>(value);
                break;
            case MetadataProperty.Publisher:
                metadata.Publisher = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.PublishingDate:
                metadata.PublishingDate = ObjectAsType<DateTime?>(value);
                break;
            case MetadataProperty.PurchaseDate:
                metadata.PurchaseDate = ObjectAsType<DateTime?>(value);
                break;
            case MetadataProperty.RecordingDate:
                metadata.RecordingDate = ObjectAsType<DateTime?>(value);
                break;
            case MetadataProperty.SortTitle:
                metadata.SortTitle = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.SortAlbum:
                metadata.SortAlbum = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.SortArtist:
                metadata.SortArtist = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.SortAlbumArtist:
                metadata.SortAlbumArtist = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.SortComposer:
                metadata.SortComposer = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Subtitle:
                metadata.Subtitle = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.Title:
                metadata.Title = ObjectAsType<string?>(value);
                break;
            case MetadataProperty.TrackNumber:
                metadata.TrackNumber = ObjectAsType<int?>(value);
                break;
            case MetadataProperty.TrackTotal:
                metadata.TrackTotal = ObjectAsType<int?>(value);
                break;
            case MetadataProperty.Chapters:
                metadata.Chapters = ObjectAsType<IList<ChapterInfo>?>(value) ?? new List<ChapterInfo>();
                break;
            case MetadataProperty.EmbeddedPictures:
                // the simpler line above that works with chapters does not work for pictures, so this cant be replaced
                var pictures = ObjectAsType<IList<PictureInfo>?>(value) ?? new List<PictureInfo>();
                metadata.EmbeddedPictures.Clear();
                foreach (var picture in pictures)
                {
                    metadata.EmbeddedPictures.Add(picture);
                }

                break;
            case MetadataProperty.AdditionalFields:
                var additionalFields = ObjectAsType<IDictionary<string, string>?>(value) ??
                                       new Dictionary<string, string>();
                foreach (var kvp in metadata.MappedAdditionalFields)
                {
                    additionalFields[kvp.Key] = kvp.Value;
                }

                metadata.AdditionalFields.Clear();
                foreach (var kvp in additionalFields)
                {
                    metadata.AdditionalFields.Add(kvp);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(property), property, null);
        }
    }


    private static T? ObjectAsType<T>(object? value)
    {
        if (value is T v)
        {
            return v;
        }

        return default;
    }

    public static void MergeProperties(this IMetadata metadata, IMetadata source)
    {
        foreach (var property in MetadataProperties)
        {
            var destinationPropertyValue = metadata.GetMetadataPropertyValue(property);
            if (IsEmpty(destinationPropertyValue))
            {
                metadata.SetMetadataPropertyValue(property, source.GetMetadataPropertyValue(property));
            }
        }
    }

    public static void OverwritePropertiesWhenNotEmpty(this IMetadata metadata, IMetadata source)
    {
        foreach (var property in MetadataProperties)
        {
            var newValue = source.GetMetadataPropertyValue(property);
            if (IsEmpty(newValue))
            {
                continue;
            }

            metadata.SetMetadataPropertyValue(property, newValue);
        }
    }

    public static void OverwriteProperties(this IMetadata metadata, IMetadata source,
        IEnumerable<string>? removeAdditionalKeys = null)
    {
        foreach (var property in MetadataProperties)
        {
            metadata.SetMetadataPropertyValue(property, source.GetMetadataPropertyValue(property));
        }

        if (removeAdditionalKeys == null) return;
        foreach (var key in removeAdditionalKeys)
        {
            metadata.AdditionalFields.Remove(key);
        }
    }


    public static bool IsEmpty(object? value, MetadataEmptyFlags flags = MetadataEmptyFlags.All) => value switch
    {
        null => flags.HasFlag(MetadataEmptyFlags.Null),
        int i => i == 0 && flags.HasFlag(MetadataEmptyFlags.Int),
        float f => f == 0.0f && flags.HasFlag(MetadataEmptyFlags.Float),
        double d => d == 0.0d && flags.HasFlag(MetadataEmptyFlags.Double),
        string s => string.IsNullOrEmpty(s) && flags.HasFlag(MetadataEmptyFlags.String),
        DateTime d => d == DateTime.MinValue && flags.HasFlag(MetadataEmptyFlags.DateTime),
        LyricsInfo l => (string.IsNullOrEmpty(l.UnsynchronizedLyrics) || l.SynchronizedLyrics.Count == 0) &&
                        flags.HasFlag(MetadataEmptyFlags.Lyrics),
        IList<ChapterInfo> { Count: 0 }
            or IList<PictureInfo> { Count: 0 }
            or IDictionary<string, string> { Count: 0 } => flags.HasFlag(MetadataEmptyFlags.Enumerable),
        _ => false
    };

    public static void ClearProperties(this IMetadata metadata, IEnumerable<MetadataProperty>? propertiesToKeep = null)
    {
        var propertiesToKeepArray = propertiesToKeep?.ToArray() ?? Array.Empty<MetadataProperty>();

        // part is a virtual property that wraps movement in case of non int values
        // so this has to be handled separately
        var keepPart = propertiesToKeepArray.Contains(MetadataProperty.Part) ||
                       propertiesToKeepArray.Contains(MetadataProperty.Movement);
        var part = keepPart
            ? metadata.GetMetadataPropertyValue(MetadataProperty.Part)
            : null;

        foreach (var property in MetadataProperties)
        {
            if (propertiesToKeepArray.Length == 0 || !propertiesToKeepArray.Contains(property))
            {
                metadata.SetMetadataPropertyValue(property, null);
            }
        }

        metadata.SetMetadataPropertyValue(MetadataProperty.Part, part);
    }

    public static List<(MetadataProperty Property, object? CurrentValue, object? NewValue)> Diff(
        this IMetadata currentTrack, IMetadata newTrack)
    {
        var diff = new List<(MetadataProperty Property, object? CurrentValue, object? NewValue)>();
        var properties = Enum.GetValues<MetadataProperty>();
        foreach (var property in properties)
        {
            var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(PictureInfo), nameof(PictureInfo.PictureData));
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var oldPropertyValue = NormalizePropertyValue(currentTrack.GetMetadataPropertyValue(property),
                serializerSettings, currentTrack.MappedAdditionalFields);
            var newPropertyValue = NormalizePropertyValue(newTrack.GetMetadataPropertyValue(property),
                serializerSettings, currentTrack.MappedAdditionalFields);

            if (oldPropertyValue == null && newPropertyValue == null ||
                EqualityComparer<object?>.Default.Equals(oldPropertyValue, newPropertyValue))
            {
                continue;
            }


            diff.Add((property, oldPropertyValue, newPropertyValue));
        }

        return diff;
    }

    private static object? NormalizePropertyValue(object? getMetadataPropertyValue,
        JsonSerializerSettings jsonSerializerSettings,
        IDictionary<string, string> currentTrackMappedAdditionalFields) => getMetadataPropertyValue switch
    {
        LyricsInfo l => JsonConvert.SerializeObject(l, jsonSerializerSettings),
        IList<ChapterInfo> c => JsonConvert.SerializeObject(c, jsonSerializerSettings),
        IList<PictureInfo> p => JsonConvert.SerializeObject(p, jsonSerializerSettings),
        IDictionary<string, string> a => NormalizeAdditionalProperties(a, jsonSerializerSettings,
            currentTrackMappedAdditionalFields),
        _ => getMetadataPropertyValue
    };

    private static string NormalizeAdditionalProperties(IDictionary<string, string> additionalFields,
        JsonSerializerSettings jsonSerializerSettings, IDictionary<string, string> mappedFields)
    {
        var result = additionalFields.Where(kvp => !mappedFields.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return JsonConvert.SerializeObject(result, jsonSerializerSettings);
    }
}