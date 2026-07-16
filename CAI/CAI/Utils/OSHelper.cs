using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CAI.Utils
{
    /// <summary>
    /// OS-level helper for launching apps, opening files, and accessing system settings.
    /// </summary>
    public static class OSHelper
    {
        /// <summary>Launch an application by executable name.</summary>
        public static bool LaunchApp(string executableName)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = executableName,
                    UseShellExecute = true,
                });
                return true;
            }
            catch { return false; }
        }

        /// <summary>Open a file or URL with the default handler.</summary>
        public static bool OpenFile(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                });
                return true;
            }
            catch { return false; }
        }

        /// <summary>Open Windows Settings to a specific page.</summary>
        public static bool OpenSettings(string settingsPage = "")
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"ms-settings:{settingsPage}",
                    UseShellExecute = true,
                });
                return true;
            }
            catch { return false; }
        }

        /// <summary>Get the current machine name.</summary>
        public static string GetMachineName() => Environment.MachineName;

        /// <summary>Get the OS version string.</summary>
        public static string GetOSVersion() => Environment.OSVersion.ToString();

        /// <summary>Get the current username.</summary>
        public static string GetUserName() => Environment.UserName;
    }
}
