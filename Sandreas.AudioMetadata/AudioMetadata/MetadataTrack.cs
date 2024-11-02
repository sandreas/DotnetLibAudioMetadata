using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using ATL;
using ATL.AudioData;

namespace Sandreas.AudioMetadata;

public class MetadataTrack : Track, IMetadata
{
    // meet IMetadata interface requirements
    public new string? Path => base.Path;
    public string? BasePath { get; set; }
    public List<MetaDataIOFactory.TagType> SupportedMetas { get; set; } = new();

    private readonly MetadataSpecification _manualMetadataSpecification = MetadataSpecification.Undefined;
    public MetadataSpecification[] MetadataSpecifications => GetMetadataSpecifications();

    private MetadataSpecification[] GetMetadataSpecifications()
    {
        var metadataFormats = MetadataFormats?.Where(f => f.ID != -1).ToList() ?? new List<Format>();
        if(metadataFormats.Any())
        {
            return metadataFormats
                .Select(AtlFileFormatToMetadataFormat)
                .Where(tagType => tagType != MetadataSpecification.Undefined)
                .ToArray();
        }

        if (_manualMetadataSpecification != MetadataSpecification.Undefined)
        {
            return new[] { _manualMetadataSpecification };
        }

        if (SupportedMetas.Any())
        {
            
            var meta = AudioFormat?.ID switch
            {
                AudioDataIOFactory.CID_MP4 => MetadataSpecification.Mp4,
                AudioDataIOFactory.CID_AIFF => MetadataSpecification.Aiff,
                AudioDataIOFactory.CID_FLAC => MetadataSpecification.Vorbis,
                AudioDataIOFactory.CID_OGG => MetadataSpecification.Vorbis,
                AudioDataIOFactory.CID_WMA => MetadataSpecification.WindowsMediaAsf,
                _ => GetFirstSupportedMeta()
            };

            if (meta != MetadataSpecification.Undefined)
            {
                return new[] { meta };
            }
        }

        return Array.Empty<MetadataSpecification>();
    }

    private MetadataSpecification GetFirstSupportedMeta()
    {
        var path = Path ?? string.Empty;
        if (path.EndsWith(".ape") && SupportedMetas.Contains(MetaDataIOFactory.TagType.APE))
        {
            return MetadataSpecification.Ape;
        }

        if (SupportedMetas.Contains(MetaDataIOFactory.TagType.ID3V2))
        {
            return Settings.ID3v2_tagSubVersion == 3  ? MetadataSpecification.Id3V23 : MetadataSpecification.Id3V24;
        }
        
        if (SupportedMetas.Contains(MetaDataIOFactory.TagType.ID3V1))
        {
            return MetadataSpecification.Id3V1;
        }

        if (SupportedMetas.Contains(MetaDataIOFactory.TagType.APE))
        {
            return MetadataSpecification.Ape;
        }

        return MetadataSpecification.Undefined;
    }

    public DateTime? RecordingDate
    {
        get => Date;
        set => Date = value;
    }

    // be able to set totalDuration, if it has not been detected or its a dummy track
    private TimeSpan? _totalDuration;

    public TimeSpan TotalDuration
    {
        get => _totalDuration ?? TimeSpan.FromMilliseconds(DurationMs);
        set => _totalDuration = value;
    }

    public IDictionary<string, string> MappedAdditionalFields => AdditionalFields
        .Where(kvp =>
        {
            var (key, _) = kvp;
            return MetadataSpecifications.Any(spec => spec switch
            {
                MetadataSpecification.Id3V23 => TagMapping.Any(t => t.Value.ID3v23 == key),
                MetadataSpecification.Id3V24 => TagMapping.Any(t => t.Value.ID3v24 == key),
                MetadataSpecification.Mp4 => TagMapping.Any(t => t.Value.Mp4 == key),
                MetadataSpecification.Matroska => TagMapping.Any(t => t.Value.Matroska == key),
                MetadataSpecification.WindowsMediaAsf => TagMapping.Any(t => t.Value.WindowsMediaAsf == key),
                MetadataSpecification.Ape => TagMapping.Any(t => t.Value.Ape == key),
                MetadataSpecification.Vorbis => TagMapping.Any(t => t.Value.Vorbis == key),
                _ => false
            });
        })
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static Dictionary<string, (string ID3v23, string ID3v24, string Mp4, string Matroska, string WindowsMediaAsf, string Ape, string Vorbis, string RiffInfo)> TagMapping { get; } = new()
    {
        // ("ID3v2.3","ID3v2.4","MP4","Matroska","ASF/Windows Media", "APE" "RIFF INFO")
        { nameof(Bpm), ("TBPM", "TBPM", "tmpo", "T=30", "WM/BeatsPerMinute", "BPM", "BPM", "") },
        // { nameof(EncodedBy), ("TENC", "TENC", "", "T=30 ENCODED_BY", "WM/EncodedBy", "EncodedBy", "ENCODED-BY", "") },
        { nameof(EncoderSettings), ("TSSE", "TSSE", "©enc", "T=30", "WM/EncodingSettings", "", "ENCODER SETTINGS", "") },
        { nameof(EncodingTool), ("", "", "©too", "", "WM/ToolName", "", "ENCODER", "") },
        { nameof(ItunesCompilation), ("TCMP", "TCMP", "cpil", "T=30", "", "Compilation", "COMPILATION", "") },
        { nameof(ItunesMediaType), ("", "", "stik", "", "", "", "", "") },
        { nameof(ItunesPlayGap), ("", "", "pgap", "", "", "", "", "") },
        { nameof(Part), ("TXXX:PART", "TXXX:PART", "----:com.pilabor.tone:PART", "T=20 PART_NUMBER", "", "", "PARTNUMBER", "") },
        // {nameof(MovementTotal), ("MVIN","MVIN","©mvc","T=30","","")}, // special case: MVIN has to be appended, not replaced
        { nameof(Narrator), ("", "", "©nrt", "T=30", "", "", "", "") },
        { nameof(PurchaseDate), ("", "", "purd", "", "", "", "", "") },
        { nameof(SortComposer), ("TSOC", "TSOC", "soco", "T=30", "", "", "", "") },
        { nameof(Subtitle), ("TIT3", "TIT3", "----:com.apple.iTunes:SUBTITLE", "T=30", "WM/SubTitle", "Subtitle", "SUBTITLE", "") },
        /*mp4 => ©st3 ? */
    };

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


    public MetadataTrack(MetadataSpecification type = MetadataSpecification.Undefined)
    {
        _manualMetadataSpecification = type;
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

    public MetadataTrack(string path, bool load = true)
        : base(path, load)
    {
        InitMetadataTrack();
    }

    public MetadataTrack(IFileSystemInfo fileInfo, bool load = true)
        : base(fileInfo.FullName, load)
    {
        InitMetadataTrack();
    }

    private void InitMetadataTrack()
    {
        Chapters ??= new List<ChapterInfo>();
        AdditionalFields ??= new Dictionary<string, string>();
        
        // if the file contains NO metadata, we need to determine the supportedMetas before the first write
        // this piece of code may be removed after solving: https://github.com/Zeugma440/atldotnet/issues/286
        if (!string.IsNullOrEmpty(Path) && !MetadataSpecifications.Any())
        {
            try
            {
                using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read, Settings.FileBufferSize, FileOptions.RandomAccess);
                var audioData = AudioDataIOFactory.GetInstance().GetFromStream(fs);
                SupportedMetas = audioData.GetSupportedMetas();
            }
            catch (Exception)
            {
                // ignored
            }
        }
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

    private string GetFirstValuedKey(string key = "")
    {
        foreach (var spec in MetadataSpecifications)
        {
            var mappedKey = MapAdditionalFieldKey(spec, key);
            if (mappedKey != "" && AdditionalFields.ContainsKey(mappedKey) && AdditionalFields[mappedKey] != null)
            {
                return mappedKey;
            }
        }
        return "";
    }
    
    private T? GetAdditionalField<T>(Func<string?, T?> converter, [CallerMemberName] string key = "")
    {
        var mappedKey = GetFirstValuedKey(key);
        return mappedKey == "" ? default : converter(AdditionalFields[mappedKey]);
    }

    private void SetAdditionalField<T>(T? value, [CallerMemberName] string key = "")
    {
        foreach (var spec in MetadataSpecifications)
        {
            var mappedKey = MapAdditionalFieldKey(spec, key);
            if (mappedKey == "")
            {
                continue;
            }

            if (value == null)
            {
                if (AdditionalFields.ContainsKey(mappedKey))
                {
                    AdditionalFields.Remove(mappedKey);
                }
                continue;
            }
            

            AdditionalFields[mappedKey] = value switch
            {
                DateTime d => FormatDate(spec, d),
                Enum e => ((int)(object)e).ToString(), // cast enum to int
                _ => value.ToString() ?? ""
            };
        }
    }

    private static string FormatDate(MetadataSpecification spec, DateTime date) => spec switch
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
    
    private static MetadataSpecification AtlFileFormatToMetadataFormat(Format format) => format.Name switch
    {
        "Native / MPEG-4" => MetadataSpecification.Mp4,
        "Native / AIFF" => MetadataSpecification.Aiff,
        "Native / Vorbis (OGG)" => MetadataSpecification.Vorbis,
        "ID3v1.1" => MetadataSpecification.Id3V1,
        "ID3v2.3" => MetadataSpecification.Id3V23,
        "ID3v2.4" => MetadataSpecification.Id3V24,
        "APEtag v2" => MetadataSpecification.Ape,
        _ => MetadataSpecification.Undefined
    };
    
    private static string MapAdditionalFieldKey(MetadataSpecification format, string key) => format switch
    {
        // ignored atm: MetadataSpecification.Id3v1 => TagMapping.ContainsKey(key) ? TagMapping[key].Id3v1 : "",
        MetadataSpecification.Id3V23 => TagMapping.ContainsKey(key) ? TagMapping[key].ID3v23 : "",
        MetadataSpecification.Id3V24 => TagMapping.ContainsKey(key) ? TagMapping[key].ID3v24 : "",
        MetadataSpecification.Mp4 => TagMapping.ContainsKey(key) ? TagMapping[key].Mp4 : "",
        MetadataSpecification.WindowsMediaAsf => TagMapping.ContainsKey(key) ? TagMapping[key].WindowsMediaAsf : "",
        MetadataSpecification.Matroska => TagMapping.ContainsKey(key) ? TagMapping[key].Matroska : "",
        MetadataSpecification.Ape => TagMapping.ContainsKey(key) ? TagMapping[key].Ape : "",
        _ => ""
    };
}