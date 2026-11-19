using System;

namespace Libraries.Attributes.Conditional
{
    /// <summary>
    /// Keeps the field visible but editable only while the condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnableIfAttribute : ConditionalDrawAttribute
    {
        public EnableIfAttribute(string memberName, params object[] expectedValues)
            : base(memberName, false, ConditionalMode.Disable, expectedValues)
        {
        }
    }
}
