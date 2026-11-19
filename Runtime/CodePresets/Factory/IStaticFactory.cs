using UnityEngine;

namespace Libraries.DNV.CodePresets.Factory
{
    public interface IStaticFactory<out TType>
    {
        public TType Create();
    }


    public class StaticFactory<TType> : IStaticFactory<TType> where TType : Object
    {
        private readonly TType _origin;

        public StaticFactory(TType origin)
        {
            _origin = origin;
        }

        public TType Create()
        {
            return Object.Instantiate(_origin);
        }
    }
   
}