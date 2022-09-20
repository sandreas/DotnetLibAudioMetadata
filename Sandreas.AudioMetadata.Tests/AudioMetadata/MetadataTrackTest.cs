namespace Sandreas.AudioMetadata.Tests.AudioMetadata;

public class MetadataTrackTest
{
    [Fact]
    public void TestMappedProperties()
    {
        var exception = Record.Exception(() =>
        {
            var track = new MetadataTrack
            {
                EncodingTool = "testing"
            };
        });
        Assert.Null(exception);
    }
}