using System;

namespace Libraries.Attributes.Conditional
{
    /// <summary>
    /// Draws the field only while the condition is met.
    /// <code>
    /// [ShowIf(nameof(_useGravity))]
    /// [ShowIf(nameof(_mode), MovementMode.Physical, MovementMode.Kinematic)]
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShowIfAttribute : ConditionalDrawAttribute
    {
        public ShowIfAttribute(string memberName, params object[] expectedValues)
            : base(memberName, false, ConditionalMode.Hide, expectedValues)
        {
        }
    }
}
