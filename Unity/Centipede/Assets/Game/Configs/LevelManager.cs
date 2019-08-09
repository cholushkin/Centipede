using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int _configIndex;
    public List<BalanceConfig> Configs;

    public void ResetProgression()
    {
        _configIndex = 0;
    }

    public BalanceConfig GetNext()
    {
        var cfg = Configs[_configIndex];
        _configIndex = Mathf.Clamp(_configIndex, 0, Configs.Count - 1);
        return cfg;
    }
}
