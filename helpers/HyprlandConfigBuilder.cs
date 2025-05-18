using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VtHyprset.Helpers
{
    public static class HyprlandConfigBuilder
    {
        private static readonly string HyprlandConfigRoot =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "hypr");

        private static readonly string HyprlandConfigPath =
            Path.Combine(HyprlandConfigRoot, "hyprland.conf");

        public static string BuildFullConfig()
        {
            return BuildCombinedConfig(HyprlandConfigPath);
        }

        private static string BuildCombinedConfig(string configPath)
        {
            var visited = new HashSet<string>();
            var sb = new StringBuilder();

            void ParseFile(string path)
            {
                var fullPath = Path.GetFullPath(path);
                Console.WriteLine($"[DEBUG] Parsing: {fullPath}");

                if (visited.Contains(fullPath))
                {
                    Console.WriteLine($"[DEBUG] Skipping already visited: {fullPath}");
                    return;
                }

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[WARNING] File not found: {fullPath}");
                    return;
                }

                visited.Add(fullPath);
                var baseDir = Path.GetDirectoryName(fullPath)!;

                foreach (var line in File.ReadLines(fullPath))
                {
                    Console.WriteLine($"[DEBUG] Line: {line}");

                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("source ="))
                    {
                        var includePath = trimmed.Substring("source =".Length).Trim();

                        // Expand ~ and resolve full path
                        if (includePath.StartsWith("~"))
                        {
                            includePath = includePath.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                        }

                        string resolvedPath = Path.IsPathRooted(includePath)
                            ? Path.GetFullPath(includePath)
                            : Path.GetFullPath(Path.Combine(baseDir, includePath));

                        Console.WriteLine($"[DEBUG] Found include: {includePath} → {resolvedPath}");

                        ParseFile(resolvedPath);
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            ParseFile(configPath);
            return sb.ToString();
        }
    }
}
