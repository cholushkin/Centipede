using GameGUI;
using UnityEngine.Assertions;

public class StateGameplay : AppStateManager.AppState<StateGameplay>
{
    public SimpleGUI GameGUI;

    #region AppStates
    public override void AppStateEnter(bool animated)
    {
        Assert.IsNotNull(GameGUI);
        gameObject.SetActive(true);
        GameGUI.PushScreen("Screen.GameHUD");
    }

    public override void AppStateLeave(bool animated)
    {
        gameObject.SetActive(false);
    }
    #endregion
}
