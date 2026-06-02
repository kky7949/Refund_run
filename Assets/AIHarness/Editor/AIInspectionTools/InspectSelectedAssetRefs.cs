using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;

namespace AIInspectionTools
{
    [UnityCliTool(
        Name = "inspect_selected_asset_refs",
        Description = "Read-only dependency and reverse-reference summary for selected Project assets.",
        Group = "inspection")]
    public static class InspectSelectedAssetRefs
    {
        public class Parameters
        {
            [ToolParameter("Expand selected folders to files. Default false.")]
            public bool ExpandFolders { get; set; }

            [ToolParameter("Maximum selected assets to inspect. Default 20.")]
            public int Limit { get; set; }

            [ToolParameter("Maximum project assets scanned for reverse refs. Default 3000.")]
            public int ScanLimit { get; set; }
        }

        public static object HandleCommand(JObject parameters)
        {
            var p = new ToolParams(parameters);
            bool expandFolders = p.GetBool("expand_folders", false);
            int limit = p.GetInt("limit", 20) ?? 20;
            int scanLimit = p.GetInt("scan_limit", 3000) ?? 3000;

            List<string> selectedPaths = InspectionUtility.GetSelectedAssetPaths(expandFolders);
            if (selectedPaths.Count == 0)
            {
                return new ErrorResponse("No selected Project assets.");
            }

            var selectedSet = new HashSet<string>(selectedPaths);
            var inspected = new List<object>();

            int count = 0;
            foreach (string path in selectedPaths)
            {
                if (count >= limit)
                {
                    break;
                }

                inspected.Add(new
                {
                    path,
                    guid = AssetDatabase.AssetPathToGUID(path),
                    dependencies = AssetDatabase.GetDependencies(path, true)
                });
                count++;
            }

            var referrers = new Dictionary<string, List<string>>();
            foreach (string path in selectedPaths)
            {
                referrers[path] = new List<string>();
            }

            string[] allGuids = AssetDatabase.FindAssets("");
            int scanned = 0;
            foreach (string guid in allGuids)
            {
                if (scanned >= scanLimit)
                {
                    break;
                }

                string candidate = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(candidate)
                    || AssetDatabase.IsValidFolder(candidate)
                    || selectedSet.Contains(candidate)
                    || IsGeneratedOrPackagePath(candidate))
                {
                    continue;
                }

                scanned++;
                string[] dependencies = AssetDatabase.GetDependencies(candidate, true);
                foreach (string selected in selectedPaths)
                {
                    if (System.Array.IndexOf(dependencies, selected) >= 0)
                    {
                        referrers[selected].Add(candidate);
                    }
                }
            }

            return new SuccessResponse("Selected asset refs inspected", new
            {
                selected_count = selectedPaths.Count,
                inspected_count = inspected.Count,
                truncated_selected_count = System.Math.Max(0, selectedPaths.Count - inspected.Count),
                reverse_scan_count = scanned,
                reverse_scan_limit = scanLimit,
                selected_assets = inspected,
                referrers
            });
        }

        private static bool IsGeneratedOrPackagePath(string path)
        {
            string normalized = path.Replace("\\", "/");
            return normalized.StartsWith("Packages/")
                || normalized.StartsWith("Library/")
                || normalized.StartsWith("ProjectSettings/")
                || Path.GetExtension(normalized) == ".cs";
        }
    }
}

