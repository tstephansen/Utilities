namespace SolutionCleaner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("*****VS Solution Cleaner*****");
        var filePath = string.Empty;
        if (args.Length == 0)
            filePath = System.Environment.CurrentDirectory;
        else
        {
            if (args[0].StartsWith("~"))
                filePath = args[0].Replace("~", $"/Users/{Environment.UserName}");
            else
                filePath = args[0];
        }
        Console.WriteLine($"\nThe folder path you provided is: {filePath}\n");
        var confirm = true;
        if (args.Any(c=> c == "-y"))
            confirm = false;
        if (!Directory.Exists(filePath))
        {
            Console.WriteLine("An invalid path was specified. Please check the path and try again.");
            return;
        }
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
                foldersToRemove.Add(objFolder);
            if (Directory.Exists(binFolder))
                foldersToRemove.Add(binFolder);
        }
        Console.WriteLine("\n\nThe following folders will be removed:\n");
        foreach(var folder in foldersToRemove)
        {
            Console.WriteLine(folder);
        }
        if (confirm)
        {
            Console.WriteLine("\nAre you sure you want to remove these folders? (y/n)");
            var result = Console.ReadLine();
            if (result?.ToLower() != "y")
            {
                Console.WriteLine("\nExiting...");
                return;
            }
        }
        var results = new List<bool>();
        foreach(var folder in foldersToRemove)
        {
            results.Add(TryRemoveFolder(folder));
        }
        if (results.All(c => c))
            Console.WriteLine("\n\nThe solution has been cleaned successfully.");
        else
            Console.WriteLine("\n\nError were encountered while cleaning the solution. The errors are visible in the terminal above.");
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
