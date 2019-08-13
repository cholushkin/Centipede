using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ControlScoreLabel : MonoBehaviour
    {
        public Text ScoreText;

        public void SetScore(int score)
        {
            ScoreText.text = score.ToString();
        }
    }
}