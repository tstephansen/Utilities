using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
			{
				Console.WriteLine("Please specify the top level directory.");
				return;
			}
			var dir = args[0];
            var cleaner = new Cleaner(dir);
            var canClean = cleaner.CalculateDirectoriesToClean();
			if (!canClean)
			{
				Console.WriteLine("No empty directories found.");
				return;
			}
			var choice = string.Empty;
			while(choice != "q")
			{
				Console.WriteLine("\n\nEmpty Directories Found!\n\nPlease choose an option:\n\n1 - List Directories To Remove\n2 - Remove Directories\nQ - Exit");
				choice = Console.ReadLine();
				Console.WriteLine("\n");
				switch(choice)
				{
					case "1":
						cleaner.ListDirectories();
						break;
					case "2":
						Console.WriteLine("\nRemoving directories...\n\n");
						System.Threading.Thread.Sleep(1000);
						cleaner.RemoveDirectories();
						if (cleaner.Exceptions.Count > 0)
						{
							Console.WriteLine("There was a problem removing one or more directories. They are listed below. All other empty directories have been removed.\n\n");
							foreach(var item in cleaner.Exceptions)
								Console.WriteLine(item);
						}
						else
						{
							Console.WriteLine("\n\nAll empty directories have been removed!");
						}
						choice = "q";
						break;
					case "q":
					case "Q":
						break;
					default:
						Console.WriteLine("You have entered an invalid selection.");
						break;
				}
			}			
        }
    }

    public class Cleaner
    {
        public Cleaner(string dir)
        {
            _baseDirectory = dir;            
        }
		
		public bool CalculateDirectoriesToClean()
		{
			var baseDirInfo = new DirectoryInfo(_baseDirectory);
			CheckIfDirectoryShouldBeCleaned(baseDirInfo);
			return _directoriesToRemove.Any();
		}

        private void CheckIfDirectoryShouldBeCleaned(DirectoryInfo dirInfo)
        {
            var directories = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            var files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach(var directory in directories)
            {
                var subDirInfo = new DirectoryInfo(directory.FullName);
                CheckIfDirectoryShouldBeCleaned(subDirInfo);
            }
            if (directories.Length == 0 && files.Length == 0)
                _directoriesToRemove.Add(dirInfo.FullName);
        }        
		
		public void ListDirectories()
		{
			foreach(var item in _directoriesToRemove)
				Console.WriteLine(item);
		}
		
		public void RemoveDirectories()
        {
			foreach(var directory in _directoriesToRemove)
			{
				try
				{
					var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
					if (files.Any())
					{
						Console.WriteLine($"{directory} is not empty so it is not being removed.");
					}
					//FileSystem.DeleteDirectory(directory, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
					Directory.Delete(directory);
					Console.WriteLine(directory);
				}
				catch (Exception ex)
				{
                    Exceptions.Add($"Unable to remove directory: {directory}\n\n {ex.Message.Trim()}");
				}
			}            
        }

        private readonly string _baseDirectory;
        private readonly List<string> _directoriesToRemove = new();
		public List<string> Exceptions { get; } = new();
    }
}
