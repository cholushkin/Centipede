using UnityEngine;

namespace Game.UI
{
    public class GameplayScoreLabelHandler : MonoBehaviour
    {
        public ControlScoreLabel ScoreLabel;

        private void Update()
        {
            // instead of event handling we just directly access needed values for this demo
            if (StateGameplay.Instance.PlayerSessionData == null)
                return;
            var valueToShow = StateGameplay.Instance.PlayerSessionData.Scores;
            ScoreLabel.SetScore(valueToShow);
        }
    }
}