using UnityEngine;
using Utils;

namespace Game
{
    public class PlayerVisual : MonoBehaviour
    {
        public GameObject PrefabExplosion;
        public MultiFrameSprite[] Animations;

        public void PlayDead()
        {
            GetComponent<SpriteRenderer>().enabled = false;
            DoExplosion(transform.position);
        }

        public void SetFish(bool flag)
        {
            Animations[0].enabled = flag;
            Animations[1].enabled = !flag;
        }

        private void DoExplosion(Vector3 at)
        {
            var explosion = Instantiate(PrefabExplosion);
            explosion.transform.position = at;
            Destroy(explosion, 2f);
        }
    }
}