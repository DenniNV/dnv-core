using System;

namespace Libraries.Attributes.Conditional
{
    /// <summary>
    /// Keeps the field visible but read only while the condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisableIfAttribute : ConditionalDrawAttribute
    {
        public DisableIfAttribute(string memberName, params object[] expectedValues)
            : base(memberName, true, ConditionalMode.Disable, expectedValues)
        {
        }
    }
}
