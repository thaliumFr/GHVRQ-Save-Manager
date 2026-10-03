using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GHVRQ_Save_Manager.Systems
{
    internal static class SaveFileLoader
    {
        private static readonly string[] SaveFolders = ["Story", "Survival", "SOA", "Challenge", "COOP_Story", "COOP_Survival", "COOP_SOA"];

        public static List<SaveFile> Files = [];

        public static event EventHandler<Result[]> OnSaveFilesExtracted;
        public static event EventHandler OnSaveFilesPushed;
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
                if (Tools.IsDebug) FolderPath = Path.Combine(Tools.DebugPath, Dir);
                Debug.WriteLine(FolderPath);
                if (!Directory.Exists(FolderPath)) continue;
                Directory.GetFiles(FolderPath)?.ToList()?.ForEach(file =>
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

            OnSaveFilesExtracted?.Invoke(null, results);
        }

        public static void PushSaveFiles(out Result[] results)
        {
            string outputDirectory = Path.Combine(Tools.appdata, "GHVRSaveFiles");
            Debug.WriteLine("Attempting saving files to device");

            results = new Result[SaveFolders.Length*3];

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
                Debug.WriteLine(folder);
                /*
                // DELETE FOLDER
                ADB.ExecuteAdbCommand($"shell rm -r /sdcard/Android/data/com.Incuvo.GreenHellVR/files/{folder}", out string DeleteOutput, out string DeleteError);
                results[index] = new Result
                {
                    Folder = folder,
                    Output = DeleteOutput,
                    Error = DeleteError
                };
                index++;*/

                //CREATE FOLDER
                ADB.ExecuteAdbCommand($"shell mkdir /sdcard/Android/data/com.Incuvo.GreenHellVR/files/{folder}", out string CreateOutput, out string CreateError);
                results[index] = new Result
                {
                    Folder = folder,
                    Output = CreateOutput,
                    Error = CreateError
                };
                index++;

                // PUSH
                startInfo.Arguments = $"/C {Tools.adbExecutablePath} push {outputDirectory}/{folder} /sdcard/Android/data/com.Incuvo.GreenHellVR/files/";
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.Close();

                if(error.Contains("error"))
                {
                    
                }
                else
                {
                    // Correct ADB's behaviour to put everything as Errors
                    output = error;
                    error = "";
                }

                results[index] = new Result
                {
                    Folder = folder,
                    Output = output,
                    Error = error
                };
                index++;
            }

            var errors = results.Where((result) => !string.IsNullOrEmpty(result.Error));
            if (errors.Count() > 0)
            {
                Debug.WriteLine("Error(s) occurred while pushing save files to device.");
                Debug.WriteLine(string.Join(Environment.NewLine, errors.Select(e => $"Folder: {e.Folder}, Error: {e.Error}")));
            }
            else
            {
                Debug.WriteLine("Save files pushed to device successfully.");
            }

            OnSaveFilesPushed?.Invoke(null, EventArgs.Empty);
        }

        public static void ClearSaveFilesFolder()
        {
            string outputDirectory = Path.Combine(Tools.appdata, "GHVRSaveFiles");
            if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, true);
        }

        public static string NameToPath(string name, out string fileName, out string filePath)
        {
            var parts = name.Split(" ");
            string category = parts[1].Replace("(", " ").Replace(")", " ").Trim();
            Debug.WriteLine($"Searching for file: {parts[0]} in category: {category}");
            SaveFile saveFile = Files.Find(file => file.FileName == parts[0] && file.Type.ToString() == category);


            fileName = parts[0];
            filePath = saveFile.FilePath;
            return filePath;
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
