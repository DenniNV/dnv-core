using System.Collections.Generic;
using System.Text;
using Libraries.DNV.Logging;

namespace Libraries.DNV.Editor.Setup
{
    internal sealed class SetupReport
    {
        private readonly List<string> _changed = new();
        private readonly List<string> _unchanged = new();
        private readonly List<string> _warnings = new();

        public bool RestartRequired { get; private set; }

        public void Changed(string message)
        {
            _changed.Add(message);
        }

        public void Unchanged(string message)
        {
            _unchanged.Add(message);
        }

        public void Warning(string message)
        {
            _warnings.Add(message);
        }

        public void RequireRestart()
        {
            RestartRequired = true;
        }

        public string Summary()
        {
            var text = new StringBuilder();
            text.AppendLine($"Changed: {_changed.Count}, already in place: {_unchanged.Count}, warnings: {_warnings.Count}.");

            if(RestartRequired)
            {
                text.AppendLine("Restart the editor to apply Active Input Handling.");
            }

            foreach (var warning in _warnings)
            {
                text.AppendLine("! " + warning);
            }

            return text.ToString();
        }

        public void Write()
        {
            var text = new StringBuilder("[ProjectSetup] ").Append(Summary());
            Append(text, "Changed", _changed);
            Append(text, "Already in place", _unchanged);

            if(_warnings.Count > 0)
            {
                Log.Warning(LogCategory.Editor, text.ToString());
                return;
            }

            Log.Info(LogCategory.Editor, text.ToString());
        }

        private static void Append(StringBuilder text, string title, List<string> lines)
        {
            if(lines.Count == 0)
            {
                return;
            }

            text.AppendLine(title + ":");

            foreach (var line in lines)
            {
                text.AppendLine("  " + line);
            }
        }
    }
}
