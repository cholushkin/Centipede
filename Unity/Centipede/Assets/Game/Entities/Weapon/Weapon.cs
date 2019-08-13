using UnityEngine;

namespace Game
{
    public interface IWeapon
    {
        void Shoot(Vector3 direction);
    }

    public class Weapon : MonoBehaviour, IWeapon
    {
        public Laser PrefabLaserBullet;
        public BoardController Board { get; set; }
        public float BulletSpeed { get; set; }
        private Laser _bullet;

        public void Shoot(Vector3 direction)
        {
            if (_bullet != null)
                return;

            _bullet = Instantiate(PrefabLaserBullet);
            _bullet.Init(Board, transform.position, BulletSpeed);
        }
    }
}