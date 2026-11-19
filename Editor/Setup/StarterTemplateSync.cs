using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Libraries.DNV.Core;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Host project only. The starter is edited in Assets, where Unity can open it, but the package holds the only tracked
    /// copy (Samples~ is invisible to Unity). Sync mirrors the Assets folders listed in <see cref="StarterLayout.MirroredFolders"/>
    /// into Samples~/Starter, <see cref="FindDrift"/> tells whether the two have diverged.
    /// </summary>
    internal static class StarterTemplateSync
    {
        private const string SyncMenuItem = MenuPath.Tools + "Sync Starter to Package";
        private const int BinarySniffLength = 8000;
        private const byte CarriageReturn = (byte)'\r';

        public enum DriftKind
        {
            Added,
            Changed,
            Removed,
        }

        public readonly struct Drift
        {
            public Drift(string relativePath, DriftKind kind)
            {
                RelativePath = relativePath;
                Kind = kind;
            }

            public string RelativePath { get; }
            public DriftKind Kind { get; }

            public override string ToString() => $"{Kind}: {RelativePath}";
        }

        [MenuItem(SyncMenuItem)]
        private static void SyncFromMenu()
        {
            Sync();
        }

        [MenuItem(SyncMenuItem, true)]
        private static bool CanSyncFromMenu()
        {
            return StarterPackage.IsEmbedded();
        }

        public static List<Drift> FindDrift()
        {
            var samples = StarterPackage.FindSamplesFolder();
            var drift = new List<Drift>();

            if(samples == null)
            {
                return drift;
            }

            var assets = Collect(StarterLayout.AssetsRoot, ExistingMirroredFolders());
            var package = Collect(samples, ExistingMirroredFolders());

            foreach (var pair in assets)
            {
                if(!package.TryGetValue(pair.Key, out var packagePath))
                {
                    drift.Add(new Drift(pair.Key, DriftKind.Added));
                }
                else if(!SameContent(pair.Value, packagePath))
                {
                    drift.Add(new Drift(pair.Key, DriftKind.Changed));
                }
            }

            drift.AddRange(package.Keys.Where(key => !assets.ContainsKey(key)).Select(key => new Drift(key, DriftKind.Removed)));
            return drift;
        }

        public static void Sync()
        {
            var samples = StarterPackage.FindSamplesFolder();

            if(samples == null || !StarterPackage.IsEmbedded())
            {
                Log.Warning(LogCategory.Editor, "Sync Starter: the package is not embedded in this project, nothing to sync.");
                return;
            }

            if(ExistingMirroredFolders().Length != StarterLayout.MirroredFolders.Length)
            {
                Log.Error(LogCategory.Editor, "Sync Starter: a starter folder is missing in Assets. Run Template > Tools > Setup Project first.");
                return;
            }

            if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            AssetDatabase.SaveAssets();

            var drift = FindDrift();

            foreach (var item in drift)
            {
                Apply(item, samples);
            }

            RemoveEmptyFolders(samples);
            Log.Info(LogCategory.Editor, drift.Count == 0
                ? "Sync Starter: the package already matches Assets."
                : $"Sync Starter: {drift.Count} file(s) updated in the package. Commit Samples~/Starter.\n" + string.Join("\n", drift));
        }

        private static void Apply(Drift drift, string samples)
        {
            var target = Path.Combine(samples, drift.RelativePath);

            if(drift.Kind == DriftKind.Removed)
            {
                File.Delete(target);
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(Path.Combine(StarterLayout.AssetsRoot, drift.RelativePath), target, true);
        }

        private static string[] ExistingMirroredFolders()
        {
            return StarterLayout.MirroredFolders
                .Where(folder => Directory.Exists(Path.Combine(StarterLayout.AssetsRoot, folder)))
                .ToArray();
        }

        private static Dictionary<string, string> Collect(string root, string[] folders)
        {
            var files = new Dictionary<string, string>();

            foreach (var folder in folders)
            {
                AddIfExists(files, root, folder + StarterLayout.MetaExtension);

                foreach (var file in Directory.GetFiles(Path.Combine(root, folder), "*", SearchOption.AllDirectories))
                {
                    AddIfExists(files, root, Path.GetRelativePath(root, file));
                }
            }

            return files;
        }

        private static void AddIfExists(Dictionary<string, string> files, string root, string relative)
        {
            var full = Path.Combine(root, relative);

            if(File.Exists(full))
            {
                files[relative.Replace('\\', '/')] = full;
            }
        }

        private static bool SameContent(string first, string second)
        {
            var firstBytes = File.ReadAllBytes(first);
            var secondBytes = File.ReadAllBytes(second);

            if(IsText(firstBytes) && IsText(secondBytes))
            {
                return WithoutCarriageReturns(firstBytes).AsSpan().SequenceEqual(WithoutCarriageReturns(secondBytes));
            }

            return firstBytes.AsSpan().SequenceEqual(secondBytes);
        }

        private static bool IsText(byte[] bytes)
        {
            var length = Math.Min(bytes.Length, BinarySniffLength);

            for (var i = 0; i < length; i++)
            {
                if(bytes[i] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] WithoutCarriageReturns(byte[] bytes)
        {
            return bytes.Where(value => value != CarriageReturn).ToArray();
        }

        private static void RemoveEmptyFolders(string samples)
        {
            foreach (var folder in StarterLayout.MirroredFolders)
            {
                var path = Path.Combine(samples, folder);

                if(!Directory.Exists(path))
                {
                    continue;
                }

                foreach (var directory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                             .OrderByDescending(directory => directory.Length))
                {
                    if(!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory);
                    }
                }
            }
        }
    }
}
