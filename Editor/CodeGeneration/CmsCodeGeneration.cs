using System;
using System.Collections.Generic;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.Logging;
using UnityEditor;

namespace Libraries.DNV.Editor.CodeGeneration
{
    public static class CmsCodeGeneration
    {
        public static void RegenerateAll(CmsDatabase database)
        {
            if(database == null)
            {
                Log.Warning(LogCategory.Editor, $"[{nameof(CmsCodeGeneration)}] CmsDatabase is null. Cannot generate code.");
                return;
            }

            foreach(var generator in CreateGenerators())
            {
                generator.Generate(database);
            }

            AssetDatabase.Refresh();
        }

        private static IEnumerable<ICmsCodeGenerator> CreateGenerators()
        {
            foreach(var type in TypeCache.GetTypesDerivedFrom<ICmsCodeGenerator>())
            {
                if(type.IsAbstract || type.IsGenericTypeDefinition)
                {
                    continue;
                }

                yield return (ICmsCodeGenerator)Activator.CreateInstance(type);
            }
        }
    }
}
