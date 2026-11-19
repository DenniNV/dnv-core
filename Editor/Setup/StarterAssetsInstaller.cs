using System.IO;
using System.Linq;
using UnityEditor;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Copies the starter assets from the package into Assets together with their .meta files: GUIDs stay the same,
    /// so the scenes, prefabs and CmsDatabase keep their references to each other. Nothing in Assets is overwritten.
    /// </summary>
    internal sealed class StarterAssetsInstaller
    {
        private const string GuidPrefix = "guid:";

        private readonly SetupReport _report;

        public StarterAssetsInstaller(SetupReport report)
        {
            _report = report;
        }

        public void Install()
        {
            var source = StarterPackage.FindSamplesFolder();

            if(source == null)
            {
                _report.Warning($"Starter assets not found in the package ({StarterLayout.SamplesFolder}).");
                return;
            }

            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith(StarterLayout.MetaExtension));

            foreach (var file in files)
            {
                CopyAsset(source, file);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private void CopyAsset(string sourceRoot, string sourceFile)
        {
            var relative = Path.GetRelativePath(sourceRoot, sourceFile).Replace('\\', '/');
            var target = $"{StarterLayout.AssetsRoot}/{relative}";

            if(File.Exists(target))
            {
                _report.Unchanged($"{target} exists, not overwritten");
                return;
            }

            var guid = ReadGuid(sourceFile + StarterLayout.MetaExtension);
            var occupiedBy = guid == null ? string.Empty : AssetDatabase.GUIDToAssetPath(guid);

            if(!string.IsNullOrEmpty(occupiedBy))
            {
                _report.Warning($"{target} skipped: its GUID is already used by {occupiedBy}");
                return;
            }

            CreateFolders(sourceRoot, Path.GetDirectoryName(target));
            File.Copy(sourceFile, target);
            File.Copy(sourceFile + StarterLayout.MetaExtension, target + StarterLayout.MetaExtension);
            _report.Changed($"{target} added");
        }

        private static void CreateFolders(string sourceRoot, string targetFolder)
        {
            targetFolder = targetFolder.Replace('\\', '/');

            if(Directory.Exists(targetFolder))
            {
                return;
            }

            CreateFolders(sourceRoot, Path.GetDirectoryName(targetFolder));
            Directory.CreateDirectory(targetFolder);

            var relative = targetFolder.Substring(StarterLayout.AssetsRoot.Length).TrimStart('/');
            var sourceMeta = Path.Combine(sourceRoot, relative) + StarterLayout.MetaExtension;

            if(File.Exists(sourceMeta))
            {
                File.Copy(sourceMeta, targetFolder + StarterLayout.MetaExtension);
            }
        }

        private static string ReadGuid(string metaPath)
        {
            if(!File.Exists(metaPath))
            {
                return null;
            }

            var line = File.ReadLines(metaPath).FirstOrDefault(text => text.StartsWith(GuidPrefix));
            return line?.Substring(GuidPrefix.Length).Trim();
        }
    }
}
