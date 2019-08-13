using UnityEngine;

namespace Game.UI
{
    public class GameplayLifeBarHandler : MonoBehaviour
    {
        public ControlLifeBar LifeBar;

        void Update()
        {
            // instead of event handling we just directly access needed values for this demo
            var valueToShow = StateGameplay.Instance.PlayerSessionData.PlayerLives;
            LifeBar.SetLifes(valueToShow);
        }
    }
}