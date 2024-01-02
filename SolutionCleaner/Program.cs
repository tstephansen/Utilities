using FolderCleaner;

namespace SolutionCleaner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("*****VS Solution Cleaner*****");
        var filePath = string.Empty;
        if (args.Any(c=> !c.StartsWith("-")))
        {
            foreach(var arg in args.Where(c=> !c.StartsWith("-")))
            {
                if (arg.StartsWith("~"))
                    filePath = arg.Replace("~", $"/Users/{Environment.UserName}");
                else
                    filePath = arg;
            }
        }
        if (string.IsNullOrEmpty(filePath))
            filePath = System.Environment.CurrentDirectory;
        Console.WriteLine($"\nThe folder path you provided is: {filePath}\n");
        var confirmationRequired = !args.Any(c => c is "-y" or "-confirm");
        if (!Directory.Exists(filePath))
        {
            Console.WriteLine("An invalid path was specified. Please check the path and try again.");
            return;
        }
        var objFolders = new List<string>();
        var binFolders = new List<DirectoryInfo>();
        var foldersToRemove = new List<string>();
        var solutionDirectory = new DirectoryInfo(filePath);
        var files = solutionDirectory.GetFiles("*.csproj", SearchOption.AllDirectories);
        foreach(var file in files)
        {
            if (file.Directory == null)
                continue;
            var objFolder = Path.Combine(file.Directory.FullName, "obj");
            var binFolder = Path.Combine(file.Directory.FullName, "bin");
            if (Directory.Exists(objFolder))
            {
                objFolders.Add(objFolder);
                foldersToRemove.Add(objFolder);
            }

            if (Directory.Exists(binFolder))
            {
                binFolders.Add(new DirectoryInfo(binFolder));
                foldersToRemove.Add(binFolder);
            }
        }
        Console.WriteLine("\nThe following folders will be removed:\n");
        foreach(var folder in foldersToRemove)
        {
            Console.WriteLine(folder);
        }
        if (confirmationRequired)
        {
            Console.WriteLine("\nAre you sure you want to remove these folders? (y/n)");
            var result = Console.ReadLine();
            if (result?.ToLower() != "y")
            {
                Console.WriteLine("\nExiting...");
                return;
            }
        }
        foreach (var folder in objFolders)
            TryRemoveFolder(folder);
        foreach(var folder in binFolders)
        {
            Console.WriteLine($"Removing folder {folder.FullName}");
            var filesToRemove = folder.GetFiles("*", SearchOption.AllDirectories);
            foreach(var file in filesToRemove)
            {
                if (file.Extension.Contains("key") || file.Extension.Contains("edf"))
                    continue;
                File.Delete(file.FullName);
            }
        }
        var cleaner = new Cleaner(filePath);
        var canClean = cleaner.CalculateDirectoriesToClean();
        if (canClean)
        {
            Console.WriteLine("Removing empty directories...");
            cleaner.RemoveDirectories();
            if (cleaner.Exceptions.Count > 0)
            {
                Console.WriteLine("There was a problem removing one or more directories. They are listed below. All other empty directories have been removed.\n\n");
                foreach(var item in cleaner.Exceptions)
                    Console.WriteLine(item);
            }
        }
        Console.WriteLine("\n\nThe solution has been cleaned.");
    }

    private static bool TryRemoveFolder(string folder)
    {
        try
        {
            Console.WriteLine($"Removing {folder}");
            Directory.Delete(folder, true);
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error removing the folder {folder}.\n{ex.Message.Trim()}");
            return false;
        }
    }
}
