using UnityEngine;

namespace Libraries.DNV.CodePresets
{
    public abstract class AutoComponent<TPrimary, TSecondary, TTertiary> : AutoComponent<TPrimary, TSecondary>
        where TPrimary : Component where TSecondary : Component where TTertiary : Component
    {
        private TTertiary _tertiaryComponent;

        protected TTertiary Tertiary => _tertiaryComponent ??= GetComponent<TTertiary>();
    }
}
