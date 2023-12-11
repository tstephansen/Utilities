using System.IO.Compression;

namespace Unzipper;

public class Program
{
    private static void Main(string[] args)
    {
		Console.WriteLine("Unzipper\nValid arguments:\nDELETE /d, -d, --delete\nINCLUDE SUBDIRECTORIES /s, -s, --sub\nPARALLEL PROCESSING (use a number after the argument to specify the degree) /p, -p, --parallel");
		Console.WriteLine(@"Example: unzipper C:\Users\USERNAME\Downloads -d -s -p 4\n");
        var folderPath = GetFolderPath(args);
        var maxDegreeOfParallelism = GetMaxDegreeOfParallelism(args);
        FileInfo[] zipFiles;
        var deleteFiles = false;
        if (args.Contains("/d") || args.Contains("-d")|| args.Contains("--delete"))
            deleteFiles = true;
        var includeSubdirectories = false;
        if (args.Contains("/s") || args.Contains("-s") || args.Contains("--sub"))
            includeSubdirectories = true;
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("The specified path does not exist.");
            return;
        }
        var folder = new DirectoryInfo(folderPath);
        if (includeSubdirectories)
            zipFiles = folder.GetFiles("*.zip", SearchOption.AllDirectories);
        else
            zipFiles = folder.GetFiles("*.zip", SearchOption.TopDirectoryOnly);
        Extract(folder, zipFiles, maxDegreeOfParallelism, deleteFiles);
        Console.WriteLine("All zip files have been extracted.");
    }

    private static string GetFolderPath(string[] args)
    {
        var folderPath = Environment.CurrentDirectory;
        var options = new string[] {"/d", "-d", "--delete", "/p", "-p", "--parallel", "/s", "-s", "--sub"};
        if (args.Length > 0)
        {
            foreach(var arg in args)
            {
                if (arg == ".")
                    break;
                if (options.Contains(arg))
                    continue;
                if (int.TryParse(arg, out _))
                    continue;
                folderPath = arg;
                break;
            }
        }
        return folderPath;
    }

    private static int GetMaxDegreeOfParallelism(string[] args)
    {
        var maxDegreeOfParallelism = 0;
        if (args.Contains("/p") || args.Contains("-p") || args.Contains("--parallel"))
        {
            maxDegreeOfParallelism = 4;
            var argsList = new List<string>(args);
            var index = argsList.IndexOf("/p");
            if (index == -1)
                index = argsList.IndexOf("-p");
            if (index == -1)
                index = argsList.IndexOf("--parallel");
            if (argsList.Count - 1 > index && int.TryParse(argsList[index+1], out var maxParallelism))
                maxDegreeOfParallelism = maxParallelism;
        }
        return maxDegreeOfParallelism;
    }

    private static void Extract(DirectoryInfo folder, FileInfo[] zipFiles, int maxDegreeOfParallelism, bool deleteFiles)
    {
        Console.WriteLine($"Extracting {zipFiles.Length} zip files...");
        if (maxDegreeOfParallelism > 0)
        {
            var counter = 0;
            Parallel.ForEach(zipFiles, new ParallelOptions{ MaxDegreeOfParallelism = maxDegreeOfParallelism}, zipFile =>
            {
                var currentCount = Interlocked.Increment(ref counter);
                var fileName = zipFile.Name.Replace(".zip", "");
                var extractPath = Path.Combine(folder.FullName, fileName);
                Console.WriteLine($"({currentCount}/{zipFiles.Length}) Extracting {zipFile.Name} to {extractPath}");
                ZipFile.ExtractToDirectory(zipFile.FullName, extractPath);
                if (deleteFiles)
                    File.Delete(zipFile.FullName);
            });
        }
        else
        {
            var counter = 1;
            foreach(var zipFile in zipFiles)
            {
                var fileName = zipFile.Name.Replace(".zip", "");
                var extractPath = Path.Combine(folder.FullName, fileName);
                Console.WriteLine($"({counter}/{zipFiles.Length}) Extracting {zipFile.Name} to {extractPath}");
                ZipFile.ExtractToDirectory(zipFile.FullName, extractPath);
                if (deleteFiles)
                    File.Delete(zipFile.FullName);
                counter++;
            }
        }
    }
}