using Utils;

namespace Game.UI
{
    public class ScreenMenu : GUIScreenBase
    {
        public void OnAnyKeyPressed()
        {
            // start new game
            StateGameplay.Instance.ResetSessionProgression();
            AppStateManager.Instance.Start(StateGameplay.Instance, false);
        }
    }
}