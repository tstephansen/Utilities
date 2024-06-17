namespace Truster;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Apple Quarantine Remover");
        if (args.Length == 0)
        {
            Console.WriteLine("A directory must be specified.");
            return;
        }
        if (!Directory.Exists(args[0]))
        {
            Console.WriteLine($"Directory not found! {args[0]}");
            return;
        }
        var directory = new DirectoryInfo(args[0]);
        var files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach(var file in files)
        {
            RemoveQuatantine(file.FullName);
        }
    }

    private static void RemoveQuatantine(string filePath)
    {
        Console.WriteLine($"Removing Quarantine from file {filePath}");
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \" xattr -d com.apple.quarantine {filePath}\" ",
                CreateNoWindow = true
            } 
        };
        process.ErrorDataReceived += (o, e) => Console.WriteLine(e.Data);
        process.Start();
        process.WaitForExit();
    }
}