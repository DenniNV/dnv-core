using UnityEngine;

namespace Libraries.DNV.MVC.Core
{
    public abstract class UIElement : MonoBehaviour
    {
        private bool _activate;

        protected bool Activate
        {
            set
            {
                _activate = value;
                gameObject.SetActive(_activate);
            }

            get => _activate;
        }
        
        public abstract void Show();
        public abstract void Hide();
        public abstract void Construct();
        public abstract void Release();
    }
}