using System;

namespace Libraries.DNV.Addressable
{
    [Serializable]
    public class CustomReferences<T> : ComponentReference<T>
    {
        public CustomReferences(string guid) : base(guid)
        {
        }
    }
}