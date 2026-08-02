using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GHVRQ_Save_Manager.Systems
{
    internal static class SaveFileLoader
    {
        public static string SaveDirectory = "/sdcard/Android/data/com.Incuvo.GreenHellVR/files/";
        private static string[] SaveFolders = ["Story", "Survival", "SOA", "Challenge", "COOP_SOA", "COOP_Story", "COOP_Survival", "Settings"];

        public static List<SaveFile> Files = [];

        public static void LoadSaveFiles()
        {
            Files = [];
            if (!File.Exists(SaveDirectory)) return;

            Directory.GetFiles(SaveDirectory+"Story").ToList().ForEach(file =>
            {
                var fileInfo = new FileInfo(file);
                var saveFile = new SaveFile
                {
                    FilePath = file,
                    FileName = fileInfo.Name,
                    AppVersion = "Unknown",
                    FileSaveDate = fileInfo.LastWriteTime.ToString(),
                    Type = SaveFile.SaveFileType.Story,
                    Mode = SaveFile.SaveFileMode.ManualSave
                };
                Files.Add(saveFile);
            });
        }
    

        public static void ExtractSaveFiles(out Result[] results) {
            string file = "Resources.BackupGHVRSaveFiles.bat";
            string outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHVRSaveFiles");

            results = new Result[SaveFolders.Length];

            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = "cmd.exe",
            };

            int index = 0;
            foreach (var folder in SaveFolders)
            {
                startInfo.Arguments = $"/C adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/{folder}/ {outputDirectory}";
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                results[index] = new Result
                {
                    Folder = folder,
                    Output = output,
                    Error = error
                };
                index++;
            }
        }
    }

    public struct Result
    {
        public string Folder { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }

    public class SaveFile{
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public string AppVersion { get; set; }

        public string FileSaveDate { get; set; }

        public SaveFileType Type { get; set; }

        public SaveFileMode Mode { get; set; }

        public enum SaveFileMode
        {
            AutoSave,
            ManualSave
        }

        public enum SaveFileType
        {
            Story,
            Survival,
            Challenge
        }
        
    }
}
