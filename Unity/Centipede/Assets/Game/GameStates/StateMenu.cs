using GameGUI;
using UnityEngine.Assertions;

public class StateMenu : AppStateManager.AppState<StateMenu>
{
    public SimpleGUI GameGUI;

    #region AppStates
    public override void AppStateEnter(bool animated)
    {
        Assert.IsNotNull(GameGUI);
        gameObject.SetActive(true);
        GameGUI.PushScreen("Screen.Menu");
    }

    public override void AppStateLeave(bool animated)
    {
        gameObject.SetActive(false);
        GameGUI.PopScreen(); // Screen.Menu
    }
    #endregion
}
