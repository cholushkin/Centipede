using UnityEngine;

public class ControlGameStarter : MonoBehaviour
{
    [Range(1f,5f)]
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
        AppStateManager.Instance.Start(StateGameplay.Instance, false);
    }
}
