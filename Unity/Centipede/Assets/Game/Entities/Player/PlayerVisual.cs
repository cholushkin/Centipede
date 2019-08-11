using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public GameObject PrefabExplosion;

    public void PlayDead()
    {
        //Instantiate(PrefabExplosion);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void PlayGodMode()
    {
        // todo:
    }
}
