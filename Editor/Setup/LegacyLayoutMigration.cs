using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Renames the misspelled Assets/Adressable of projects set up before 0.3.2 to Assets/Addressable. The folder is moved
    /// through AssetDatabase, so GUIDs and every reference to the assets stay intact; only the Addressables addresses,
    /// which are the folder paths, are renamed by hand. Runs before the starter is installed, otherwise the installer
    /// would copy a second Addressable folder next to the old one.
    /// </summary>
    internal sealed class LegacyLayoutMigration
    {
        private readonly SetupReport _report;

        public LegacyLayoutMigration(SetupReport report)
        {
            _report = report;
        }

        public void Apply()
        {
            if(!AssetDatabase.IsValidFolder(StarterLayout.LegacyAddressableRoot))
            {
                return;
            }

            if(AssetDatabase.IsValidFolder(StarterLayout.AddressableRoot))
            {
                _report.Warning($"{StarterLayout.LegacyAddressableRoot} and {StarterLayout.AddressableRoot} both exist, " +
                                "not migrated: move the contents of the old folder by hand");
                return;
            }

            var error = AssetDatabase.MoveAsset(StarterLayout.LegacyAddressableRoot, StarterLayout.AddressableRoot);

            if(!string.IsNullOrEmpty(error))
            {
                _report.Warning($"{StarterLayout.LegacyAddressableRoot} not migrated: {error}");
                return;
            }

            _report.Changed($"{StarterLayout.LegacyAddressableRoot} → {StarterLayout.AddressableRoot}");
            RenameAddresses();
        }

        private void RenameAddresses()
        {
            if(!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var renamed = false;

            foreach (var folder in StarterLayout.AddressableFolders)
            {
                renamed |= RenameAddress(settings, folder);
            }

            if(renamed)
            {
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
                AssetDatabase.SaveAssets();
            }
        }

        private bool RenameAddress(AddressableAssetSettings settings, string folder)
        {
            var legacyAddress = StarterLayout.LegacyAddressableRoot + folder.Substring(StarterLayout.AddressableRoot.Length);
            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(folder));

            if(entry == null || entry.address != legacyAddress)
            {
                return false;
            }

            entry.address = folder;
            _report.Changed($"Addressable address: {legacyAddress} → {folder}");
            return true;
        }
    }
}
