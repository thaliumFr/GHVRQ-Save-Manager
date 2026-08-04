using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GHVRQ_Save_Manager.Systems
{
    internal static class SaveFileLoader
    {
        private static readonly string[] SaveFolders = ["Story", "Survival", "SOA", "Challenge", "COOP_Story", "COOP_Survival", "COOP_SOA", "Settings"];

        public static List<SaveFile> Files = [];

        public static event EventHandler<Result[]> OnSaveFileExtracted;
        public static event EventHandler<List<SaveFile>> OnSaveFileLoaded;

        public static void LoadSaveFiles()
        {
            string outputDirectory = Tools.appdata;

            Files = [];
            if (!Directory.Exists(outputDirectory))
            {
                Debug.WriteLine($"{outputDirectory} isn't a thing");
                return;
            }

            foreach (var Dir in SaveFolders)
            {
                string FolderPath = Path.Combine(outputDirectory, "GHVRSaveFiles", Dir );
                Debug.WriteLine(FolderPath);
                Directory.GetFiles(FolderPath).ToList().ForEach(file =>
                {
                    var fileInfo = new FileInfo(file);
                    var saveFile = new SaveFile
                    {
                        FilePath = file,
                        FileName = fileInfo.Name,
                        AppVersion = "Unknown",
                        FileSaveDate = fileInfo.LastWriteTime.ToString(),
                        Type = (SaveFile.SaveFileType)Enum.Parse(typeof(SaveFile.SaveFileType), Dir),
                        Mode = SaveFile.SaveFileMode.ManualSave
                    };
                    Files.Add(saveFile);
                });
            }

            OnSaveFileLoaded?.Invoke(null, Files);
            Debug.WriteLine(Files.Count);
        }
    

        public static void ExtractSaveFiles(out Result[] results) {
            string outputDirectory = Path.Combine(Tools.appdata, "GHVRSaveFiles");

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
                startInfo.Arguments = $"/C {Tools.adbExecutablePath} pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/{folder}/ {outputDirectory}";
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.Close();

                results[index] = new Result
                {
                    Folder = folder,
                    Output = output,
                    Error = error
                };
                index++;
            }

            OnSaveFileExtracted?.Invoke(null, results);
        }

        public static void ClearSaveFilesFolder()
        {
            string outputDirectory = Path.Combine(Tools.appdata, "GHVRSaveFiles");
            Directory.Delete(outputDirectory, true);
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
            SOA,
            Challenge,
            COOP_Story,
            COOP_Survival,
            COOP_SOA,
            Settings,
        }
        
    }
}
