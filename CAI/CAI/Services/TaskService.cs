using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CAI.Services
{
    /// <summary>
    /// Executes OS-level commands and application launches.
    /// Processes tasks received from the server.
    /// </summary>
    public class TaskService
    {
        /// <summary>
        /// Execute a task command based on its type.
        /// </summary>
        public async Task<(bool Success, string Result)> ExecuteAsync(
            string command, string commandType)
        {
            try
            {
                return commandType switch
                {
                    "app" => await LaunchApplicationAsync(command),
                    "file" => await OpenFileAsync(command),
                    "system" => await RunSystemCommandAsync(command),
                    "settings" => await OpenSettingsAsync(command),
                    _ => await RunSystemCommandAsync(command),
                };
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>Launch an application by name or path.</summary>
        public Task<(bool, string)> LaunchApplicationAsync(string appNameOrPath)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = appNameOrPath,
                    UseShellExecute = true,
                };
                Process.Start(psi);
                return Task.FromResult((true, $"Launched: {appNameOrPath}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, $"Failed to launch {appNameOrPath}: {ex.Message}"));
            }
        }

        /// <summary>Open a file with the default associated application.</summary>
        public Task<(bool, string)> OpenFileAsync(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                });
                return Task.FromResult((true, $"Opened: {filePath}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, $"Failed to open {filePath}: {ex.Message}"));
            }
        }

        /// <summary>Run a system command via cmd.exe.</summary>
        public async Task<(bool, string)> RunSystemCommandAsync(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(psi);
                if (process == null) return (false, "Failed to start process");

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                    return (true, output.Trim());
                else
                    return (false, string.IsNullOrEmpty(error) ? output : error);
            }
            catch (Exception ex)
            {
                return (false, $"Command failed: {ex.Message}");
            }
        }

        /// <summary>Open Windows Settings to a specific page.</summary>
        public Task<(bool, string)> OpenSettingsAsync(string settingsUri)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"ms-settings:{settingsUri}",
                    UseShellExecute = true,
                });
                return Task.FromResult((true, $"Opened settings: {settingsUri}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, $"Failed: {ex.Message}"));
            }
        }
    }
}
