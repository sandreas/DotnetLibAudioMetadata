using FluentAssertions;

namespace Sandreas.AudioMetadata.Tests.AudioMetadata;

public class MetadataTrackTest
{
    [Fact]
    public void Narrator_ShouldContainCorrectId3v2_WhenValueIsSet()
    {
        var sut = new MetadataTrack(MetadataSpecification.Id3V24);
        sut.Narrator.Should().Be(null);
        sut.Narrator = "Jim Narrator";
        sut.Narrator.Should().Be("Jim Narrator");
        sut.AdditionalFields.Should().ContainKey("TXXX:NARRATOR");
    }
    
        
    [Fact]
    public void Narrator_ShouldContainCorrectMp4Atom_WhenValueIsSet()
    {
        var sut = new MetadataTrack(MetadataSpecification.Mp4);
        sut.Narrator.Should().Be(null);
        sut.Narrator = "Jim Narrator";
        sut.Narrator.Should().Be("Jim Narrator");
        sut.AdditionalFields.Should().ContainKey("©nrt");
    }

    
}