using UnityEngine;

namespace Libraries.DNV.CodePresets
{
    public abstract class AutoComponent<TPrimary, TSecondary> : AutoComponent<TPrimary>
        where TPrimary : Component where TSecondary : Component
    {
        private TSecondary _secondaryComponent;

        protected TSecondary SecondComponent => _secondaryComponent ??= GetComponent<TSecondary>();
    }
}
