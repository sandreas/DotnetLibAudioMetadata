using System.IO.Abstractions;
using ATL;
using ATL.AudioData;

namespace Sandreas.AudioMetadata;

public class MetadataTrack : MetadataTrackHolder
{
    public MetadataTrack(MetadataSpecification type = MetadataSpecification.Undefined): base(type)
    {
    }
    
    public MetadataTrack(string path, bool load = true)
        : base(path, load)
    {
    }

    public MetadataTrack(IFileSystemInfo fileInfo, bool load = true)
        : base(fileInfo, load)
    {
    }
    public MetadataSpecification[] MetadataSpecifications => GetMetadataSpecifications();

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
        { nameof(Narrator), ("TXXX:NARRATOR", "TXXX:NARRATOR", "©nrt", "T=30", "", "", "", "") },
        { nameof(PurchaseDate), ("", "", "purd", "", "", "", "", "") },
        { nameof(SortComposer), ("TSOC", "TSOC", "soco", "T=30", "", "", "", "") },
        { nameof(Subtitle), ("TIT3", "TIT3", "----:com.apple.iTunes:SUBTITLE", "T=30", "WM/SubTitle", "Subtitle", "SUBTITLE", "") },
        /*mp4 => ©st3 ? */
    };
    protected override IDictionary<string, string> MapAdditionalFields()
    {
        return AdditionalFields
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
    }
    private MetadataSpecification[] GetMetadataSpecifications()
    {
        var metadataFormats = MetadataFormats?.Where(f => f.ID > 0).ToList() ?? new List<Format>();
        if(metadataFormats.Any())
        {
            return metadataFormats
                .Select(AtlFileFormatToMetadataFormat)
                .Where(tagType => tagType != MetadataSpecification.Undefined)
                .ToArray();
        }

        if (ManualMetadataSpecification != MetadataSpecification.Undefined)
        {
            return new[] { ManualMetadataSpecification };
        }

        if (SupportedMetadataFormats is not null && SupportedMetadataFormats.Any())
        {
            var meta = ContainerIdToMetadataSpecification(AudioFormat?.ContainerId ?? 0);
            if (meta != MetadataSpecification.Undefined)
            {
                return new[] { meta };
            }
        }

        return Array.Empty<MetadataSpecification>();
    }

    private MetadataSpecification ContainerIdToMetadataSpecification(int containerId) => containerId switch
    {
        AudioDataIOFactory.CID_MP4 => MetadataSpecification.Mp4,
        AudioDataIOFactory.CID_AIFF => MetadataSpecification.Aiff,
        AudioDataIOFactory.CID_FLAC => MetadataSpecification.Vorbis,
        AudioDataIOFactory.CID_OGG => MetadataSpecification.Vorbis,
        AudioDataIOFactory.CID_WMA => MetadataSpecification.WindowsMediaAsf,
        _ => GetFirstSupportedMeta()
    };

    private MetadataSpecification GetFirstSupportedMeta()
    {
        var path = Path ?? string.Empty;
        var hasApeSupport = SupportedMetadataFormats.Any(f => f.ID == (int)MetaDataIOFactory.TagType.APE);
        if (path.EndsWith(".ape") && hasApeSupport)
        {
            return MetadataSpecification.Ape;
        }

        if (SupportedMetadataFormats.Any(f => f.ID == (int)MetaDataIOFactory.TagType.ID3V2))
        {
            return Settings.ID3v2_tagSubVersion == 3  ? MetadataSpecification.Id3V23 : MetadataSpecification.Id3V24;
        }
        
        if (SupportedMetadataFormats.Any(f => f.ID == (int)MetaDataIOFactory.TagType.ID3V1))
        {
            return MetadataSpecification.Id3V1;
        }

        if (hasApeSupport)
        {
            return MetadataSpecification.Ape;
        }

        return MetadataSpecification.Undefined;
    }

    
    protected override string GetFirstValuedKey(string key = "")
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
    
    private MetadataSpecification AtlFileFormatToMetadataFormat(Format format)
    {
        // native formats
        if (format.ShortName == "Native")
        {
            return ContainerIdToMetadataSpecification(AudioFormat.ContainerId);
        }
        
        return format.Name switch
        {
            "Native tagging / MPEG-4" => MetadataSpecification.Mp4,
            "Native tagging / AIFF" => MetadataSpecification.Aiff,
            "Native tagging / Vorbis (OGG)" => MetadataSpecification.Vorbis,
            "ID3v1.1" => MetadataSpecification.Id3V1,
            "ID3v2.3" => MetadataSpecification.Id3V23,
            "ID3v2.4" => MetadataSpecification.Id3V24,
            "APEtag v2" => MetadataSpecification.Ape,
            _ => MetadataSpecification.Undefined
        };
    }
    
    protected static string MapAdditionalFieldKey(MetadataSpecification format, string key) => format switch
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

    public override void SetAdditionalField<T>(T? value, string key = "") where T : default
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
            
            base.SetAdditionalField(value, mappedKey);
        }
    }
}