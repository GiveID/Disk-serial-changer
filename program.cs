using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.IO.Compression;
using System.Threading;
using System.Security.Principal;

class DiskSerialChanger
{
    static Random random = new Random();
    static string volumeIdExe = "volumeid.exe";
    static string downloadUrl = "https://download.sysinternals.com/files/VolumeId.zip";
    static string zipFileName = "VolumeId.zip";

    static void Main()
    {
        // Check for Admin rights
        if (!IsUserAdministrator())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ ERROR: This program requires Administrator privileges.");
            Console.ResetColor();
            RestartAsAdministrator();
            return;
        }

        Console.Title = "🖥️ Disk Serial Changer";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=====================================");
        Console.WriteLine("  🛠️ Hard Disk Serial Number Changer");
        Console.WriteLine("=====================================\n");
        Console.ResetColor();

        // Simulate a startup scan
        LoadingEffect("🔍 Scanning system...", 5);

        // Ensure volumeid.exe is available
        string volumeIdPath = EnsureVolumeIdExe();
        if (string.IsNullOrEmpty(volumeIdPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Failed to locate or download volumeid.exe!");
            Console.ResetColor();
            PauseBeforeExit();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Found volumeid.exe at: {volumeIdPath}\n");
        Console.ResetColor();

        // List available drives
        var drives = DriveInfo.GetDrives()
            .Where(d => d.DriveType == DriveType.Fixed)
            .ToList();

        if (drives.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No fixed drives found.");
            Console.ResetColor();
            PauseBeforeExit();
            return;
        }

        Console.WriteLine("📂 Available Drives:");
        for (int i = 0; i < drives.Count; i++)
        {
            Console.WriteLine($"  [{i + 1}] {drives[i].Name}");
        }

        // Select a drive
        Console.Write("\n🎯 Select a drive (1-{0}): ", drives.Count);
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > drives.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Invalid selection.");
            Console.ResetColor();
            PauseBeforeExit();
            return;
        }

        string selectedDrive = drives[choice - 1].Name.TrimEnd('\\');

        // Generate random serial number
        LoadingEffect("🔢 Generating new serial number...", 3);
        string newSerial = GenerateRandomSerial();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n🔹 New Serial Number: {newSerial}");
        Console.ResetColor();

        // Simulating system preparation
        LoadingEffect("⚙️ Preparing system for changes...", 4);

        try
        {
            // Execute volumeid.exe with parameters
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n⚡ Changing Serial Number...");
            Console.ResetColor();

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C \"{volumeIdPath}\" {selectedDrive} {newSerial} && pause";
            process.StartInfo.Verb = "runas";  // Request admin privileges
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal; // Show CMD window
            process.Start();
            process.WaitForExit();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Serial number changed successfully to {newSerial}.");
            Console.WriteLine("🔄 Please reboot your system for changes to take effect.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.ResetColor();
        }

        // Prevent the console from closing immediately
        PauseBeforeExit();
    }

    // Ensures volumeid.exe is available by downloading if necessary
    static string EnsureVolumeIdExe()
    {
        string volumeIdPath = FindVolumeIdExe();

        if (!string.IsNullOrEmpty(volumeIdPath))
        {
            return volumeIdPath;
        }

        Console.WriteLine("\n📥 Downloading VolumeId from Sysinternals...");
        LoadingEffect("⏳ Connecting to server...", 5);

        try
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(downloadUrl, zipFileName);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Download complete. Extracting...");
            Console.ResetColor();

            ZipFile.ExtractToDirectory(zipFileName, Directory.GetCurrentDirectory());
            File.Delete(zipFileName); // Clean up the zip file

            if (File.Exists(volumeIdExe))
            {
                return Path.Combine(Directory.GetCurrentDirectory(), volumeIdExe);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error downloading/extracting VolumeId: {ex.Message}");
            Console.ResetColor();
        }

        return null;
    }

    // Searches for volumeid.exe in common locations
    static string FindVolumeIdExe()
    {
        string[] commonPaths =
        {
            Directory.GetCurrentDirectory(),
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            Environment.GetEnvironmentVariable("SystemRoot") + @"\System32",
            Environment.GetEnvironmentVariable("SystemRoot"),
            Environment.GetEnvironmentVariable("ProgramFiles"),
            Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
        };

        foreach (var path in commonPaths)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fullPath = Path.Combine(path, "volumeid.exe");
                if (File.Exists(fullPath))
                    return fullPath;
            }
        }

        return null;
    }

    // Check if the program is running as Administrator
    static bool IsUserAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    // Restart the program with Administrator privileges
    static void RestartAsAdministrator()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Could not restart with Administrator privileges: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Generates a random serial number in XXXX-XXXX format (HEX)
    static string GenerateRandomSerial()
    {
        return $"{RandomHex(4)}-{RandomHex(4)}";
    }

    // Generates a random HEX string of given length
    static string RandomHex(int length)
    {
        const string hexDigits = "0123456789ABCDEF";
        char[] hexChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            hexChars[i] = hexDigits[random.Next(hexDigits.Length)];
        }
        return new string(hexChars);
    }

    // Loading animation
    static void LoadingEffect(string message, int dots)
    {
        Console.Write(message);
        for (int i = 0; i < dots; i++)
        {
            Thread.Sleep(500);
            Console.Write(".");
        }
        Console.WriteLine(" ✅");
        Thread.Sleep(500);
    }

    // Prevents the console from closing immediately
    static void PauseBeforeExit()
    {
        Console.WriteLine("\n🔵 Press Enter to exit...");
        Console.ReadLine();
    }
}
