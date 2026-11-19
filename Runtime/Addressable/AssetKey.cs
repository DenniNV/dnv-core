using System;

namespace Libraries.DNV.Addressable
{
    internal readonly struct AssetKey : IEquatable<AssetKey>
    {
        public string Guid { get; }
        public Type Type { get; }

        private AssetKey(string guid, Type type)
        {
            Guid = guid;
            Type = type;
        }

        public static AssetKey Of<T>(string guid) => new AssetKey(guid, typeof(T));

        public bool Equals(AssetKey other) => Type == other.Type && Guid == other.Guid;

        public override bool Equals(object obj) => obj is AssetKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Guid, Type);

        public override string ToString() => $"{Guid} ({Type.Name})";
    }
}
