using System;
using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor.Toolbar
{
    /// <summary>
    /// Creates every non-abstract <see cref="ToolbarButton"/> (found via <see cref="TypeCache"/>) once per domain
    /// and draws them by side.
    /// </summary>
    internal static class ToolbarButtonRegistry
    {
        private static readonly IReadOnlyList<ToolbarButton> LeftButtons;
        private static readonly IReadOnlyList<ToolbarButton> RightButtons;

        static ToolbarButtonRegistry()
        {
            var buttons = CreateButtons();
            LeftButtons = SelectSide(buttons, ToolbarSide.Left);
            RightButtons = SelectSide(buttons, ToolbarSide.Right);
        }

        public static void Draw(ToolbarSide side)
        {
            var buttons = side == ToolbarSide.Left ? LeftButtons : RightButtons;

            GUILayout.BeginHorizontal();
            foreach(var button in buttons)
            {
                button.Draw();
            }
            GUILayout.EndHorizontal();
        }

        private static List<ToolbarButton> CreateButtons()
        {
            var buttons = new List<ToolbarButton>();

            foreach(var type in TypeCache.GetTypesDerivedFrom<ToolbarButton>())
            {
                if(type.IsAbstract)
                {
                    continue;
                }

                try
                {
                    buttons.Add((ToolbarButton)Activator.CreateInstance(type));
                }
                catch(Exception e)
                {
                    Log.Warning(LogCategory.Editor, $"[{nameof(ToolbarButtonRegistry)}] Cannot create {type.Name}: {e.Message}");
                }
            }

            return buttons;
        }

        private static IReadOnlyList<ToolbarButton> SelectSide(IEnumerable<ToolbarButton> buttons, ToolbarSide side)
        {
            return buttons.Where(button => button.Side == side).OrderBy(button => button.Order).ToList();
        }
    }
}
