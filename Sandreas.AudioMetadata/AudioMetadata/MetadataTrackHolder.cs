using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using ATL;

namespace Sandreas.AudioMetadata;

public class MetadataTrackHolder: Track, IMetadata
{
    public new string? Path => base.Path;
    public string? BasePath { get; set; }

    protected readonly MetadataSpecification ManualMetadataSpecification = MetadataSpecification.Undefined;

    
    public DateTime? RecordingDate
    {
        get => Date;
        set => Date = value;
    }

    // be able to set totalDuration, if it has not been detected or it's a dummy track
    private TimeSpan? _totalDuration;

    public TimeSpan TotalDuration
    {
        get => _totalDuration ?? TimeSpan.FromMilliseconds(DurationMs);
        set => _totalDuration = value;
    }

    public IDictionary<string, string> MappedAdditionalFields => MapAdditionalFields();
    protected virtual IDictionary<string, string> MapAdditionalFields() => AdditionalFields;

    public int? Bpm
    {
        get => GetAdditionalField(IntField);
        set => SetAdditionalField(value);
    }

    /*
    public string? EncodedBy
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }
    */

    public string? EncoderSettings
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }

    public string? EncodingTool
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }

    public ItunesCompilation? ItunesCompilation
    {
        get => HasAdditionalField() ? GetAdditionalField(EnumField<ItunesCompilation>) : null;
        set => SetAdditionalField(value);
    }

    public ItunesMediaType? ItunesMediaType
    {
        get => HasAdditionalField() ? GetAdditionalField(EnumField<ItunesMediaType>) : null;
        set => SetAdditionalField(value);
    }

    public ItunesPlayGap? ItunesPlayGap
    {
        get => HasAdditionalField() ? GetAdditionalField(EnumField<ItunesPlayGap>) : null;
        set => SetAdditionalField(value);
    }

    public string? Movement
    {
        get => SeriesPart;
        set {
            // movement MUST contain a valid integer value,
            // values like 1.5 lead to an exception
            // use Part to store a non-integer value, it will automatically handle Movement
            if (string.IsNullOrEmpty(value))
            {
                // SeriesPart only gets removed with an empty string
                // null does not work
                SeriesPart = ""; 
            }
            else if ( int.TryParse(value, out var intValue))
            {
                // only if int.TryParse succeeds, a value will be set
                SeriesPart = intValue.ToString();
            }
            
            // unparsable integer values will be ignored completely
        }
    }

    public string? Part
    {
        get => GetAdditionalField(StringField) ?? Movement;
        set
        {
            Movement = value;
            SetAdditionalField(value);
        }
    }

    public string? MovementName
    {
        get => SeriesTitle;
        set => SeriesTitle = value;
    }

    public string? Narrator
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }

    public DateTime? PurchaseDate
    {
        get => GetAdditionalField(DateTimeField);
        set => SetAdditionalField(value);
    }

    public string? SortComposer
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }

    public string? Subtitle
    {
        get => GetAdditionalField(StringField);
        set => SetAdditionalField(value);
    }


    public MetadataTrackHolder(MetadataSpecification type = MetadataSpecification.Undefined)
    {
        ManualMetadataSpecification = type;
        InitMetadataTrack();
        // https://pastebin.com/DQTZFE6H

        // json spec for tag field mapping
        // Fields => (preferredField,writeTemplate,parseCallback), (alternate1, template, parseCallback), (alternate2,template)

        // possible improvements for the future (unclear/unspecified mapping):
        // ACOUSTID_ID
        // ACOUSTID_FINGERPRINT
        // BARCODE
        // CATALOGNUMBER
        // INITIALKEY
        // INVOLVEDPEOPLE
        // MUSICBRAINZ_*
        // PODCAST*

        // ffmpeg: https://kodi.wiki/view/Video_file_tagging
        // author => ©aut

        // Remove is indeed to remove an entire tag type (e.g. ID3v2 or APE) from a file
        // If you want to remove a field, just assign an empty value "" to it and save
    }

    public MetadataTrackHolder(string path, bool load = true)
        : base(path, load)
    {
        InitMetadataTrack();
    }

    public MetadataTrackHolder(IFileSystemInfo fileInfo, bool load = true)
        : base(fileInfo.FullName, load)
    {
        InitMetadataTrack();
    }

    private void InitMetadataTrack()
    {
        Chapters ??= new List<ChapterInfo>();
        AdditionalFields ??= new Dictionary<string, string>();
    }

    private static string? StringField(string? value) => value;
    private static int? IntField(string? value) => int.TryParse(value, out var result) ? result : null;

    private static DateTime? DateTimeField(string? value) =>
        DateTime.TryParse(value, out var result) ? result : null;

    private static T? EnumField<T>(string? value) where T : Enum?
    {
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        if (int.TryParse(value, out var enumValueInt) && Enum.IsDefined(typeof(T), enumValueInt))
        {
            return (T)Enum.ToObject(typeof(T), enumValueInt);
        }

        if (Enum.TryParse(typeof(T), value, out var enumValue) && enumValue != null)
        {
            return (T)enumValue;
        }

        return default;
    }
    
    private bool HasAdditionalField([CallerMemberName] string key = "")
    {
        return GetFirstValuedKey(key) != "";
    }
    
    protected virtual string GetFirstValuedKey(string key = "") => AdditionalFields.ContainsKey(key) ? key : string.Empty;

    protected  T? GetAdditionalField<T>(Func<string?, T?> converter, [CallerMemberName] string key = "")
    {
        var mappedKey = GetFirstValuedKey(key);
        return mappedKey == "" ? default : converter(AdditionalFields[mappedKey]);
    }

    public virtual void SetAdditionalField<T>(T? value, [CallerMemberName] string key = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        AdditionalFields[key] = value switch
        {
            DateTime d => FormatDate(MetadataSpecification.Undefined, d),
            Enum e => ((int)(object)e).ToString(), // cast enum to int
            _ => value?.ToString() ?? ""
        };
    }
    
    protected static string FormatDate(MetadataSpecification spec, DateTime date) => spec switch
    {
        /*
d.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss").Replace(" 00:00:00", "")
For ID3v2.3 and older, you'd combine the TYER tag (YYYY) with the TDAT tag (MMDD) and TIME tag (HHMM).
For ID3v2.4, you'd use TDRC or TDRA (or any of the other timestamp frames), with any level of accuracy you want, up to: YYYYMMDDTHHMMSS. Include Year, throw in month, throw in day, add the literal T and throw in hour, minute, second.
Vorbis: ISO8601
*/
        MetadataSpecification.Vorbis => date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        _ => date.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss").Replace(" 00:00:00", "")
    };
}