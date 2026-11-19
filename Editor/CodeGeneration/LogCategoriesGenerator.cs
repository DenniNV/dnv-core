using System.Globalization;
using System.Linq;
using Libraries.DNV.Logging;
using UnityEditor;

namespace Libraries.DNV.Editor.CodeGeneration
{
    /// <summary>
    /// Game log categories declared in Project Settings > Logger become GameLogCategories.Xxx,
    /// used exactly like the library ones: Log.Info(GameLogCategories.Combat, ...).
    /// </summary>
    public static class LogCategoriesGenerator
    {
        private const string ClassName = "GameLogCategories";

        public static void Generate(LoggerSettings settings)
        {
            var constants = settings.GameCategories
                .Select(entry => new GeneratedConstant(entry.Name, entry.Id.ToString(CultureInfo.InvariantCulture)));

            var members = GeneratedCodeWriter.BuildMembers(ClassName, constants,
                (identifier, constant) =>
                    $"public static readonly LogCategory {identifier} = new({constant.Value}, \"{Escape(constant.Key)}\");");

            GeneratedCodeWriter.WriteMembers(ClassName, members, typeof(LogCategory).Namespace);
            AssetDatabase.Refresh();
        }

        private static string Escape(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
