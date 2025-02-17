using FluentAssertions;

namespace Sandreas.AudioMetadata.Tests.AudioMetadata;

public class MetadataTrackHolderTest
{
    private readonly MetadataTrackHolder _sut;

    public MetadataTrackHolderTest()
    {
        _sut = new MetadataTrackHolder();
    }
    
        
    [Fact]
    public void Bpm_ShouldWork()
    {
        _sut.Bpm.Should().Be(null);
        _sut.Bpm = 100;
        _sut.Bpm.Should().Be(100);
    }    
    
    [Fact]
    public void Part_ShouldAlsoSetMovement_WhenIntegerStringIsUsed()
    {
        _sut.Part = "1";
        _sut.Part.Should().Be("1");
        _sut.Movement.Should().Be("1");
    }    
    
    [Fact]
    public void Part_ShouldNotSetMovement_WhenDecimalStringIsUsed()
    {
        _sut.Part = "2.1";
        _sut.Part.Should().Be("2.1");
        _sut.Movement.Should().BeNull();
    }  

    [Fact]
    public void Part_ShouldRemovePartAndMovement_WhenRemoved()
    {
        _sut.Part = "1";
        _sut.Part.Should().Be("1");
        _sut.RemoveMetadataPropertyValue(MetadataProperty.Part);
        _sut.Movement.Should().BeEmpty();
        _sut.Part.Should().BeEmpty();
    }    
    
    [Fact]
    public void Movement_ShouldSetAndRemoveMovement_WhenIntegerStringIsUsed()
    {
        _sut.Movement = "1";
        _sut.Movement.Should().Be("1");
        _sut.RemoveMetadataPropertyValue(MetadataProperty.Movement);
        _sut.Movement.Should().BeEmpty();
    }
    
    [Fact]
    public void RecordingDate_ShouldBeMinValue_WhenRemoved()
    {
        _sut.RecordingDate = DateTime.Today;
        _sut.RemoveMetadataPropertyValue(MetadataProperty.RecordingDate);
        _sut.RecordingDate.Should().BeSameDateAs(DateTime.MinValue);
    }
}