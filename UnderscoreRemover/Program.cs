namespace UnderscoreRemover;

class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        if (args.Length == 0)
            args = new[] { "/Users/tim/Git/FlipperAmiibo" };
#endif
        Console.WriteLine("Underscore Remover");
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a path.");
            return;
        }
        var folderPath = args[0];
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("Invalid path provided.");
            return;
        }
        var dir = new DirectoryInfo(folderPath);
        var folders = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
        RenameFolders(folders);
        var files = dir.GetFiles("*", SearchOption.AllDirectories);
        RenameFiles(files);
        Console.WriteLine("Folders and Files Renamed");
    }

    private static void RenameFolders(DirectoryInfo[] folders)
    {
        foreach(var folder in folders)
        {
            var subFolders = folder.GetDirectories("*", SearchOption.TopDirectoryOnly);
            if (subFolders.Length > 0)
                RenameFolders(subFolders);
        }
        foreach(var folder in folders)
        {
            if (!folder.Name.Contains("_"))
                continue;
            var newFolderPath = folder.FullName.Replace(folder.Name, folder.Name.Replace("_", " "));
            Directory.Move(folder.FullName, newFolderPath);
        }
    }

    private static void RenameFiles(FileInfo[] files)
    {
        foreach(var file in files)
        {
            if (!file.Name.Contains("_"))
                continue;
            var newFileName = file.Name.Replace("_", " ");
            File.Move(file.FullName, file.FullName.Replace(file.Name, newFileName));
        }
    }
}
