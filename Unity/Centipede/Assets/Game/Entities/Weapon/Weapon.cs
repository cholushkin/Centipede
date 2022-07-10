using UnityEngine;

namespace Game
{
    public interface IWeapon
    {
        bool Shoot(Vector3 direction);
    }

    public class Weapon : MonoBehaviour, IWeapon
    {
        public Laser PrefabLaserBullet;
        public BoardController Board { get; set; }
        public float BulletSpeed { get; set; }
        private Laser _bullet;

        public bool Shoot(Vector3 direction)
        {
            if (IsBulletFlying()) // don't shoot if there is another instance
                return false;
            _bullet = Instantiate(PrefabLaserBullet);
            _bullet.Init(Board, transform.position, BulletSpeed);
            return true;
        }

        public bool IsBulletFlying()
        {
            return (_bullet != null);
        }
    }
}