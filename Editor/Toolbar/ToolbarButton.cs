using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor.Toolbar
{
    public enum ToolbarSide
    {
        Left,
        Right
    }

    /// <summary>
    /// Base class for a main toolbar button. Subclass it in any Editor assembly — the button is discovered
    /// by <see cref="ToolbarButtonRegistry"/> and shown automatically, no registration needed.
    /// </summary>
    public abstract class ToolbarButton
    {
        private const float DefaultWidth = 80f;
        private const int FontSize = 12;

        private GUIStyle _style;

        public abstract string Label { get; }

        public abstract ToolbarSide Side { get; }

        public virtual string Tooltip => string.Empty;

        /// <summary>When set, replaces the label.</summary>
        public virtual Texture2D Icon => null;

        public virtual float Width => DefaultWidth;

        public virtual float SpaceBefore => 0f;

        /// <summary>Lower goes first inside its side.</summary>
        public virtual int Order => 0;

        public abstract void OnClick();

        /// <summary>Override for a fully custom look.</summary>
        public virtual void Draw()
        {
            if(!ShouldDraw())
            {
                return;
            }

            if(SpaceBefore > 0f)
            {
                GUILayout.Space(SpaceBefore);
            }

            if(GUILayout.Button(CreateContent(), GetStyle()))
            {
                OnClick();
            }
        }

        /// <summary>Hidden in Play Mode by default: most toolbar actions only make sense in Edit Mode.</summary>
        protected virtual bool ShouldDraw() => !EditorApplication.isPlaying;

        protected virtual GUIStyle GetStyle()
        {
            return _style ??= new GUIStyle("Command")
            {
                fixedWidth = Width,
                fontSize = FontSize,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = Icon != null ? ImagePosition.ImageOnly : ImagePosition.TextOnly,
            };
        }

        private GUIContent CreateContent()
        {
            return Icon != null
                ? new GUIContent(Icon, Tooltip)
                : new GUIContent(Label, Tooltip);
        }
    }
}
