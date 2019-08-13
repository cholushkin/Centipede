using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public GameObject PrefabExplosion;

    public void PlayDead()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        DoExplosion(transform.position);
    }

    private void DoExplosion( Vector3 at)
    {
        var explosion = Instantiate(PrefabExplosion);
        explosion.transform.position = at;
        Destroy(explosion, 2f);
    }
}
