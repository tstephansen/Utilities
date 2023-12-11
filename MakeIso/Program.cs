﻿using System.Text;

namespace MakeIso;

public class Program
{
    private static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Invalid arguments. Please specify the folder and target file.\nExample:\n");
            Console.WriteLine("MakeIso.exe \"C:/Users/Username/Desktop/FolderToMakeIso\" \"C:/Users/Desktop/MyNewIsoFile.iso\"");
            Console.WriteLine("MakeIso.exe \"C:/Users/Username/Desktop/FolderToMakeIso\" \"MyNewIsoFile.iso\"");
            return;
        }
        if (!Directory.Exists(args[0]))
        {
            Console.WriteLine("The specified directory doesn't exist. Please check the path and run the command again.");
            return;
        }
        var directory = new DirectoryInfo(args[0]);
        var filePath = GetFilePathFromFileName(directory, args.Length == 1 ? string.Empty : args[1]);
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("The path to the created ISO file is not valid. Please enter a valid file path.");
            return;
        }
        if (!filePath.EndsWith(".iso"))
            filePath = $"{filePath}.iso";
        var result = IsoBuilder.BuildIso(directory, filePath);
        Console.WriteLine(result != 0
            ? "An error occured while creating the ISO file."
            : $"ISO file created successfully and is located at {filePath}.");
    }

    private static string GetFilePathFromFileName(DirectoryInfo directory, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return $"{Path.Combine(directory.FullName, directory.Name)}.iso";
        string[] split;
        if (fileName.Contains('\\'))
            split = fileName.Split("\\");
        else if (fileName.Contains('/'))
            split = fileName.Split("/");
        else
        {
            return directory.Parent != null
                ? Path.Combine(directory.Parent.FullName, fileName)
                : string.Empty;
        }
        var sb = new StringBuilder();
        for (var i = 0; i < split.Length - 1; i++)
        {
            sb.Append($"{split[i]}\\");
        }
        var filePath = sb.ToString();
        return !Directory.Exists(filePath)
            ? $"{Path.Combine(directory.FullName, directory.Name)}.iso"
            : fileName;
    }
}