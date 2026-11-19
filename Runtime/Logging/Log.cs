using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Logging
{
    public static class Log
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Info(LogCategory category, string message)
        {
            Write(category, message, Debug.Log, null);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Info(LogCategory category, string message, Object context)
        {
            Write(category, message, Debug.Log, context);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(LogCategory category, string message)
        {
            Write(category, message, Debug.LogWarning, null);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(LogCategory category, string message, Object context)
        {
            Write(category, message, Debug.LogWarning, context);
        }

        public static void Error(LogCategory category, string message)
        {
            Error(category, message, null);
        }

        public static void Error(LogCategory category, string message, Object context)
        {
            var settings = LoggerSettings.Instance;
            var color = settings != null ? settings.GetColor(category) : Color.white;
            Debug.LogError(Format(category, message, color), context);
        }

        private static void Write(LogCategory category, string message, Action<object, Object> logAction, Object context)
        {
            var settings = LoggerSettings.Instance;
            if (settings == null || !settings.Enabled)
                return;

            logAction(Format(category, message, settings.GetColor(category)), context);
        }

        private static string Format(LogCategory category, string message, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{category}] {message}</color>";
        }
    }
}
