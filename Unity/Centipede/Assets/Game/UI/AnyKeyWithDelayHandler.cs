using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class AnyKeyWithDelayHandler : MonoBehaviour
    {
        public UnityEvent OnAnyKeyPressed;

        [Range(0.25f, 3f)]
        public float Cooldown;
        private float _currentCooldown;

        void OnEnable()
        {
            ResetCooldown();
        }

        void Update()
        {
            _currentCooldown -= Time.deltaTime;
            if (Input.anyKeyDown && _currentCooldown < 0f)
                ProcessAnyKeyPressed();
        }

        private void ResetCooldown()
        {
            _currentCooldown = Cooldown;
        }

        private void ProcessAnyKeyPressed()
        {
            ResetCooldown();
            OnAnyKeyPressed.Invoke();
        }
    }
}
