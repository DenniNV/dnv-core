using System;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Libraries.DNV.Editor.Toolbar
{
    /// <summary>
    /// Unity has no public API for custom buttons in the main toolbar, so two docks are inserted around
    /// the Play/Pause block of the internal MainToolbarWindow (Unity 6000.3+).
    /// The window appears a few frames after a domain reload, hence polling on <see cref="EditorApplication.update"/>.
    /// </summary>
    [InitializeOnLoad]
    internal static class MainToolbarInjector
    {
        private const string MainToolbarTypeName = "UnityEditor.MainToolbarWindow";
        private const string PlayModeContainerClass = "unity-overlay-container__middle-container";
        private const string LeftDockName = "DNV_Toolbar_Left";
        private const string RightDockName = "DNV_Toolbar_Right";
        private const int MaxSetupAttempts = 200;

        private static readonly Type MainToolbarType = typeof(UnityEditor.Editor).Assembly.GetType(MainToolbarTypeName);

        private static int _setupAttempts;

        static MainToolbarInjector()
        {
            EditorApplication.update -= TryInject;
            EditorApplication.update += TryInject;
        }

        private static void TryInject()
        {
            var playModeContainer = FindPlayModeContainer();
            if(playModeContainer?.parent != null)
            {
                Inject(playModeContainer);
                StopPolling();
                return;
            }

            if(++_setupAttempts < MaxSetupAttempts)
            {
                return;
            }

            Log.Warning(LogCategory.Editor, $"[{nameof(MainToolbarInjector)}] Main toolbar not found, custom toolbar buttons are disabled.");
            StopPolling();
        }

        private static VisualElement FindPlayModeContainer()
        {
            if(MainToolbarType == null)
            {
                return null;
            }

            var toolbars = Resources.FindObjectsOfTypeAll(MainToolbarType);
            if(toolbars.Length == 0)
            {
                return null;
            }

            return ((EditorWindow)toolbars[0]).rootVisualElement?.Q(className: PlayModeContainerClass);
        }

        private static void Inject(VisualElement playModeContainer)
        {
            var parent = playModeContainer.parent;

            // The toolbar window can outlive a domain reload; drop docks left by the previous domain.
            parent.Q(LeftDockName)?.RemoveFromHierarchy();
            parent.Q(RightDockName)?.RemoveFromHierarchy();

            parent.Insert(parent.IndexOf(playModeContainer), CreateDock(LeftDockName, ToolbarSide.Left, Justify.FlexEnd));
            parent.Insert(parent.IndexOf(playModeContainer) + 1, CreateDock(RightDockName, ToolbarSide.Right, Justify.FlexStart));
        }

        private static VisualElement CreateDock(string name, ToolbarSide side, Justify alignment)
        {
            var dock = new VisualElement
            {
                name = name,
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    flexDirection = FlexDirection.Row,
                    justifyContent = alignment,
                    alignItems = Align.Center
                }
            };

            dock.Add(new IMGUIContainer(() => ToolbarButtonRegistry.Draw(side)));
            return dock;
        }

        private static void StopPolling()
        {
            EditorApplication.update -= TryInject;
        }
    }
}
