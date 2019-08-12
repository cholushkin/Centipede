using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public GameObject PrefabExplosion;

    public void PlayDead()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
