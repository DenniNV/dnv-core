using System;

namespace Libraries.DNV.ContentManagerSystem
{
    [Serializable]
    public abstract class CmsEntityPart
    {
        public abstract string PartName { get; }
    }
}
