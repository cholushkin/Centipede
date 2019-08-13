using Utils;

namespace Game.UI
{
    public class ScreenPopupGameOver : GUIScreenBase
    {
        public void OnAnyKeyPressed()
        {
            SimpleGui.PopScreen(); // pop self
            AppStateManager.Instance.Start(StateMenu.Instance, false);
        }
    }
}