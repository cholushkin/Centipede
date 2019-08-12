using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class ControlLifeBar : MonoBehaviour
    {
        public GameObject PrefabLifeIcon;

        public void SetLifes(int amount)
        {
            if (amount > transform.childCount) // add icons?
            {
                Assert.IsNotNull(PrefabLifeIcon);
                for (int i = 0; i < amount - transform.childCount; ++i)
                    Instantiate(PrefabLifeIcon, transform);
            }
            else if (amount < transform.childCount) // remove ?
            {
                for (int i = 0; i < transform.childCount - amount; ++i)
                    Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}