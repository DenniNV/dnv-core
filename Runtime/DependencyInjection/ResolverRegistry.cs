using Libraries.DNV.DependencyInjection.Interfaces;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection
{
    internal static class ResolverRegistry
    {
        internal static IContainer Container { get; private set; } = new Container();

        // With Domain Reload disabled statics survive Play Mode restarts, and so would the previous session's resolvers.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlayModeEnter()
        {
            Container = new Container();
        }

        public static void Add(IResolver resolver)
        {
            Container.Add(resolver);
        }

        public static void Remove(IResolver resolver)
        {
            Container.Remove(resolver);
        }
    }
}
