namespace Sandreas.AudioMetadata.Tests.AudioMetadata;

public class MetadataTrackTest
{
    [Fact]
    public void TestRemoveMovement()
    {
        var t = new MetadataTrack
        {
            Movement = "1"
        };
        t.RemoveMetadataPropertyValue(MetadataProperty.Movement);
        Assert.Null(t.Movement);
    }
    
    [Fact]
    public void TestRecordingDate()
    {
        var t = new MetadataTrack
        {
            RecordingDate = DateTime.Today
        };
        t.RemoveMetadataPropertyValue(MetadataProperty.RecordingDate);
        Assert.True(t.RecordingDate <= DateTime.MinValue);
    }
}