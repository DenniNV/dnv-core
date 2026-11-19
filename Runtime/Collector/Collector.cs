using System;
using System.Collections.Generic;
using UnityEngine;

namespace Libraries.DNV.Collector
{
    [Serializable]
    public class Collector<TType>  : IComponentCollector<TType> where TType : class
    {
        private IReadOnlyCollection<TType> _collection = new List<TType>();

        public IReadOnlyCollection<TType> Get(Transform parent, bool includeInactive = false)
        {
            if (_collection.Count < 1)
            {
                _collection = parent.GetComponentsInChildren<TType>(includeInactive);
            }

            return _collection;
        }
    }

    public interface IComponentCollector<out TType> where TType : class
    {
        public IReadOnlyCollection<TType> Get(Transform parent, bool includeInactive = false);
    }
}