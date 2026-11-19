using System.IO;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Locates the starter assets inside the installed package.
    /// </summary>
    internal static class StarterPackage
    {
        public static string FindSamplesFolder()
        {
            var package = FindPackage();

            if(package == null)
            {
                return null;
            }

            var folder = Path.Combine(package.resolvedPath, StarterLayout.SamplesFolder);
            return Directory.Exists(folder) ? folder : null;
        }

        /// <summary>
        /// Only the host project holds the package as an embedded folder. In a game the package is read-only,
        /// so there is nothing to sync back to it.
        /// </summary>
        public static bool IsEmbedded()
        {
            var package = FindPackage();
            return package != null && package.source == PackageSource.Embedded;
        }

        private static PackageInfo FindPackage()
        {
            return PackageInfo.FindForAssembly(typeof(StarterPackage).Assembly);
        }
    }
}
