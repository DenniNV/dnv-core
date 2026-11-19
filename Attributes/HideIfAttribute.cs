using System;

namespace Libraries.Attributes.Conditional
{
    /// <summary>
    /// Hides the field while the condition is met.
    /// <code>
    /// [HideIf(nameof(_isStatic))]
    /// [HideIf(nameof(_mode), MovementMode.Kinematic)]
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideIfAttribute : ConditionalDrawAttribute
    {
        public HideIfAttribute(string memberName, params object[] expectedValues)
            : base(memberName, true, ConditionalMode.Hide, expectedValues)
        {
        }
    }
}
