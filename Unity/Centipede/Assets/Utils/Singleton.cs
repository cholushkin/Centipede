using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public bool IsPersistantSingleton;
        public static T Instance { get; private set; }

        public static void KillGameObject()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
                Instance = null;
            }
        }

        public void AssignInstance()
        {
            Instance = (T)this;
            if (IsPersistantSingleton)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void Awake()
        {
            // found second instance
            if (Instance != null && Instance != this)
            {
                if (IsPersistantSingleton)
                {
                    Destroy(gameObject);
                    return;
                }
                Debug.LogErrorFormat("Got a second instance of the class {0} {1}", GetType(), name);
                Debug.LogErrorFormat("First instance: '{0}'", name);
            }
            Debug.LogFormat("Singleton instance assigning. Type:{0}, Transform:{1}", GetType(), name);
            AssignInstance();
        }
    }
}
