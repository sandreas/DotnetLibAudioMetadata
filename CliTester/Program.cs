// See https://aka.ms/new-console-template for more information

using ATL;
using Newtonsoft.Json;
using Sandreas.AudioMetadata;

var file = "/home/andreas/projects/tone/tone/var/issues/76/input.m4b";
var t = new MetadataTrack(file);
var x = t.MetadataSpecifications;


/*
var src = "/home/andreas/projects/DotnetLibAudioMetadata/CliTester/samples/sample.mp3";
var dst = "/home/andreas/projects/DotnetLibAudioMetadata/CliTester/tmp/sample.m4a";

var track = new Track(dst);
track.AdditionalFields["©nrt"] = "Narrator Name";
*/
/*
var track = new MetadataTrack(dst);
track.Narrator = "Narrator Name";
*/

// await track.SaveAsync();
return await Task.FromResult(0);
/*
var chaptersJson = """
                   [{"StartTime":0,"EndTime":158220,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"001","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":158220,"EndTime":1194248,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"002","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":1194248,"EndTime":2264363,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"003","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":2264363,"EndTime":3590501,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"004","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":3590501,"EndTime":5053776,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"005","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":5053776,"EndTime":6364868,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"006","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":6364868,"EndTime":6960552,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"007","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":6960552,"EndTime":7869614,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"008","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":7869614,"EndTime":8391645,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"009","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":8391645,"EndTime":8794650,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"010","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":8794650,"EndTime":9919657,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"011","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":9919657,"EndTime":10306083,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"012","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":10306083,"EndTime":10843625,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"013","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":10843625,"EndTime":11589311,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"014","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":11589311,"EndTime":12154903,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"015","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":12154903,"EndTime":12734659,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"016","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":12734659,"EndTime":14135193,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"017","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":14135193,"EndTime":14863325,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"018","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":14863325,"EndTime":15600187,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"019","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":15600187,"EndTime":16303751,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"020","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":16303751,"EndTime":17764333,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"021","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":17764333,"EndTime":18350730,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"022","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null},{"StartTime":18350730,"EndTime":19305210,"StartOffset":0,"EndOffset":0,"UseOffset":false,"Title":"023","UniqueID":"","UniqueNumericID":0,"Subtitle":"","Url":null,"Picture":null}]
                   """;


var chaps = JsonConvert.DeserializeObject<List<JsonSerializableChapterInfo>>(chaptersJson);

var originalFile = "file.m4b";
var fileToEdit = "file_to_edit.m4b";
if (File.Exists(fileToEdit))
{
    File.Delete(fileToEdit);
}
File.Copy(originalFile, fileToEdit);
var track = new Track(fileToEdit);
track.Chapters.Clear();

foreach (var chapter in chaps)
{
    track.Chapters.Add(chapter);
}

await track.SaveAsync();

return await Task.FromResult(0);

class JsonSerializableChapterInfo : ChapterInfo
{

}
*/

/*
var x = Settings.DefaultTagsWhenNoMetadata;



var track = new Track(dst);
track.Description = "description test";
Console.WriteLine(track.Description);
await track.SaveAsync();

var y = "y";
*/
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
/*
var value = "10";
if (int.TryParse(value, out var intValue))
{
    Console.WriteLine(intValue);
}
else
{
    Console.WriteLine("failed");
}

Settings.UseFileNameWhenNoTitle = false;
var files = new List<string>(["sample.flac", "sample.m4a", "sample.mp3"]);
foreach (var f in files)
{
    var full = "/home/andreas/projects/DotnetLibAudioMetadata/CliTester/tmp/" + f;
    var track = new Track(full);
    Console.WriteLine(track.Title);
    if (!string.IsNullOrEmpty(track.Title))
    {
        Console.WriteLine($"{f}: track.Title is not empty, although I removed it");
    }
    track.Title = "";
    await track.SaveAsync();
}
*/

/*
switch (track.MetadataFormats.FirstOrDefault()?.Name)
{
    case "Native / MPEG-4":
        track.AdditionalFields["----:com.pilabor.tone:PART"] = "2.1";
        break;
    case "ID3v2.3":
    case "ID3v2.4":
        track.AdditionalFields["TXXX:PART"] = "2.1";
        break;
    default:
        // ????
        break;
}
*/