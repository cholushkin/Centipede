using UnityEngine;

namespace Utils
{
    public class GUIScreenBase : MonoBehaviour
    {
        public bool IsModal;
        public bool IsDestroyOnPop;

        public Transform ModalBg;

        public SimpleGUI SimpleGui { get; set; }
        public bool IsInTransaction { get; protected set; }

        private int _inputEnabledRefs;
        public bool IsInputEnabled
        {
            get => _inputEnabledRefs > 0;
            set => _inputEnabledRefs += value ? 1 : -1;
        }

        public int GetRefsCount()
        {
            return _inputEnabledRefs;
        }

        public virtual void Awake()
        {
            SetupBackground(IsModal);
            SimpleGui = GetComponentInParent<SimpleGUI>();
        }

        public void OnNewScreenPushed()
        {
            _inputEnabledRefs = 1;
        }

        public virtual void StartAppearAnimation()
        {
            IsInTransaction = true;
            IsInputEnabled = false;
            OnAppear();
        }

        public virtual void StartDisappearAnimation()
        {
            IsInTransaction = true;
            IsInputEnabled = false;
            OnDisappear();
        }

        public virtual void OnAppear()
        {
            IsInTransaction = false;
            IsInputEnabled = true;
        }

        public virtual void OnDisappear()
        {
            IsInTransaction = false;
            SimpleGui.OnScreenPoped(this);
        }

        protected void SetupBackground(bool isbgEnabled)
        {
            if (ModalBg)
                ModalBg.gameObject.SetActive(isbgEnabled);
        }

        public void OnBecomeUnderModal(bool isUnder)
        {
            IsInputEnabled = !isUnder;
        }
    }
}
