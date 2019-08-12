using UnityEngine;
using Utils;

namespace Game.UI
{
    public class ControlGameStarter : MonoBehaviour
    {
        [Range(1f, 5f)]
        public float Cooldown;
        private float _currentCooldown;

        void OnEnable()
        {
            ResetCooldown();
        }

        void Update()
        {
            _currentCooldown -= Time.deltaTime;
            if (Input.anyKey && _currentCooldown < 0f)
                ProcessAnyKeyPressed();
        }

        private void ResetCooldown()
        {
            _currentCooldown = Cooldown;
        }

        private void ProcessAnyKeyPressed()
        {
            ResetCooldown();

            // start new game
            StateGameplay.Instance.ResetSessionProgression();
            AppStateManager.Instance.Start(StateGameplay.Instance, false);
        }
    }
}
