using System;
using UnityEngine;

namespace Libraries.Attributes.Conditional
{
    /// <summary>
    /// Base for attributes that hide or disable a field in the inspector depending on another member's value.
    /// The condition member is a field, a property or a parameterless method declared next to the marked field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ConditionalDrawAttribute : PropertyAttribute
    {
        protected ConditionalDrawAttribute(string memberName, bool inverted, ConditionalMode mode, object[] expectedValues)
        {
            MemberName = memberName;
            Inverted = inverted;
            Mode = mode;
            ExpectedValues = expectedValues ?? Array.Empty<object>();
        }

        public string MemberName { get; }

        /// <summary>
        /// Values the condition member is compared against, any match satisfies the condition.
        /// When empty, the member value is treated as a flag: bool is used as is, object references
        /// and strings satisfy the condition while they are assigned.
        /// </summary>
        public object[] ExpectedValues { get; }

        public bool Inverted { get; }

        public ConditionalMode Mode { get; }
    }
}
