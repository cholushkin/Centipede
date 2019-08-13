using UnityEngine.UI;
using Utils;

namespace Game.UI
{
    public class ScreenPopupLevelWin : GUIScreenBase
    {
        public Text LevelComplete;
        public Text Level;

        public override void Awake()
        {
            base.Awake();
            LevelComplete.text = string.Format("Level {0} complete!", StateGameplay.Instance.LevelManager.GetCurrentLevelIndex());
            Level.text = string.Format("Level {0}", StateGameplay.Instance.LevelManager.GetCurrentLevelIndex() + 1);
        }

        public void OnAnyKeyPressed()
        {
            SimpleGui.PopScreen(); // pop self
            AppStateManager.Instance.Start(StateGameplay.Instance, false);
        }
    }
}