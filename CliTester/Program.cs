// See https://aka.ms/new-console-template for more information

using ATL;
using Sandreas.AudioMetadata;


var src = "/home/andreas/projects/DotnetLibAudioMetadata/CliTester/samples/sample.mp3";
var dst = "/home/andreas/projects/DotnetLibAudioMetadata/CliTester/tmp/sample.mp3";
/*
if (File.Exists(dst))
{
    File.Delete(dst);
}

File.Copy(
    src,
    dst
    );
*/
/*
var track = new Track(dst);
track.AdditionalFields.Clear();
track.SeriesPart = null;
await track.SaveAsync();
return await Task.FromResult(0);
*/

var track = new MetadataTrack(dst);

track.RemoveMetadataPropertyValue(MetadataProperty.Part);
await track.SaveAsync();
return await Task.FromResult(0);
