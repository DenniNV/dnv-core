using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// EntryPoint loads the contexts through Addressables by GUID, so their folders must be addressable.
    /// </summary>
    internal sealed class AddressablesSetup
    {
        private readonly SetupReport _report;

        public AddressablesSetup(SetupReport report)
        {
            _report = report;
        }

        public void Apply()
        {
            var settings = GetOrCreateSettings();
            var changed = false;

            foreach (var folder in StarterLayout.AddressableFolders)
            {
                changed |= AddFolder(settings, folder);
            }

            if(changed)
            {
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
                AssetDatabase.SaveAssets();
            }
        }

        private AddressableAssetSettings GetOrCreateSettings()
        {
            if(AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                return AddressableAssetSettingsDefaultObject.Settings;
            }

            _report.Changed("Addressables settings created");
            return AddressableAssetSettingsDefaultObject.GetSettings(true);
        }

        private bool AddFolder(AddressableAssetSettings settings, string folder)
        {
            var guid = AssetDatabase.AssetPathToGUID(folder);

            if(string.IsNullOrEmpty(guid))
            {
                _report.Warning($"{folder} not found, not made addressable");
                return false;
            }

            if(settings.FindAssetEntry(guid, true) != null)
            {
                _report.Unchanged($"Addressable: {folder}");
                return false;
            }

            var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            entry.address = folder;
            _report.Changed($"Addressable: {folder} → {settings.DefaultGroup.Name}");
            return true;
        }
    }
}
