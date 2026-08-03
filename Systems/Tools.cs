using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GHVRQ_Save_Manager.Systems
{
    internal class Tools
    {
        public static readonly string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GHVRQSaveManager");
        public static readonly string adbPath = Path.Combine(appdata, "platform-tools");
        public static readonly string adbExecutablePath = Path.Combine(adbPath, "adb.exe");

        public static string GetEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetEntryAssembly();

            foreach (var item in assembly.GetManifestResourceNames())
            {
                Debug.WriteLine($"Resource found: {item}");
            }

            var resourceStream = assembly?.GetManifestResourceStream($"GHVRQ_Save_Manager.{resourceName}");
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        
    }
}
