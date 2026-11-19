using UnityEngine;

namespace Libraries.DNV.CodePresets
{
    public class Hash : MonoBehaviour
    {
        private string _value;

        // Lazy instead of Awake: installers read the tag before Awake when the object starts inactive.
        public string Value => _value ??= gameObject.GetHashCode().ToString();
    }
}