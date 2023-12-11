using DiscUtils.Iso9660;

namespace MakeIso;

public class IsoBuilder
{
    public static int BuildIso(DirectoryInfo sourceDirectory, string targetFile)
    {
        var builder = new CDBuilder();
        var resultList = new Dictionary<string, string>();
        try
        {
            GetFileList(sourceDirectory, sourceDirectory).ToList().ForEach(file => resultList.Add(file.Key, file.Value));
            foreach (var (key, value) in resultList.ToList())
                builder.AddFile(key, value);
            builder.Build(targetFile);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error Writing ISO. Check Permissions and Files. {e.Message}");
            return 1;
        }
        return 0;
    }

    private static Dictionary<string, string> GetFileList(DirectoryInfo folder, FileSystemInfo home)
    {
        var filesDictionary = new Dictionary<string, string>();
        var files = folder.GetFiles();
        foreach (var file in files)
				filesDictionary.Add(file.FullName.Split("\\")[^1], file.FullName);
        foreach (var directory in folder.GetDirectories())
				GetFileList(directory, home).ToList().ForEach(file => filesDictionary.Add(file.Key, file.Value));
        return filesDictionary;
    }
}