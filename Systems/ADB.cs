using GHVRQ_Save_Manager.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

internal class ADB
{
    public static event EventHandler<DeviceData[]> OnDevicesChanged;
    public static DeviceData[] deviceDatas = [];

    private static bool IsFirstInitialization = true;

    public static void CheckForAdb()
    {

        if (!Path.Exists(Tools.appdata))
        {
            Directory.CreateDirectory(Tools.appdata);
        }

        Debug.WriteLine(Tools.adbPath);
        if (!Directory.Exists(Tools.adbPath))
        {
            Debug.WriteLine($"ADB not found. Installing ...");
            System.IO.Compression.ZipFile.ExtractToDirectory(Assembly.GetEntryAssembly().GetManifestResourceStream($"GHVRQ_Save_Manager.Resources.platform-tools-latest-windows.zip"), Tools.appdata);
        }
        else
        {
            Debug.WriteLine("ADB found.");
        }
    }

    public static DeviceData[] CheckForDevices()
    {
        Debug.WriteLine("Refreshing devices");

        ExecuteAdbCommand("devices -l", out string output, out string error);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.WriteLine($"Error checking for devices: {error}");
            return [];
        }
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var devices = new List<DeviceData>();
        foreach (var line in lines)
        {
            if (line.Contains("device") && !line.Contains("List of devices attached"))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var device = new DeviceData
                {
                    ID = parts[0],
                    Model = parts.Length > 1 ? parts[3].Replace("model:", "") : "Unknown"
                };
                devices.Add(device);
            }
        }

        if (devices.Count != deviceDatas.Length || IsFirstInitialization)
        {
            if (IsFirstInitialization)
            {
                Debug.WriteLine("Was first init of devices");
            }
            Debug.WriteLine($"Devices changed: {devices.Count} devices connected.");
            IsFirstInitialization = false;
            deviceDatas = devices.ToArray();
            OnDevicesChanged?.Invoke(null, deviceDatas);
        }

        return devices.ToArray();
    }

    public static void ExecuteAdbCommand(string arguments, out string output, out string error)
    {
        Process process = new();
        ProcessStartInfo startInfo = new()
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            CreateNoWindow = true,
            FileName = Tools.adbExecutablePath,
            Arguments = arguments
        };
        process.StartInfo = startInfo;
        process.Start();
        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();
        process.WaitForExit();
    }

    public struct DeviceData
    {
        public string ID;
        public string Model;
    }
}

