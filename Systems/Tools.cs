using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GHVRQ_Save_Manager.Systems
{
    internal class Tools
    {
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

        public static void CheckForAdb()
        {
            string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GHVRQSaveManager");
            if (!Path.Exists(appdata))
            {
                Directory.CreateDirectory(appdata);
            }

            string adbPath = Path.Combine(appdata, "platform-tools");

            Debug.WriteLine(adbPath);
            if (!Directory.Exists(adbPath))
            {
                Debug.WriteLine($"ADB not found. Installing ...");
                System.IO.Compression.ZipFile.ExtractToDirectory(Assembly.GetEntryAssembly().GetManifestResourceStream($"GHVRQ_Save_Manager.Resources.platform-tools-latest-windows.zip"), appdata);
            }
            else
            {
                Debug.WriteLine("ADB found.");
            }
        }
    }
}
