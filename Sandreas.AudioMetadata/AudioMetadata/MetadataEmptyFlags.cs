namespace Sandreas.AudioMetadata;

public enum MetadataEmptyFlags
{
    None = 1 << 0,
    Null = 1 << 1,
    Int = 1 << 2,
    Float = 1 << 3,
    Double = 1 << 4,
    String = 1 << 5,
    DateTime = 1 << 6,
    Lyrics = 1 << 7,
    Enumerable = 1 << 8,
    All = int.MaxValue
}