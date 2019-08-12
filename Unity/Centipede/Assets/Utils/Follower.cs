using UnityEngine;

namespace Utils
{
    public class Follower : MonoBehaviour
    {
        public Transform Target;

        public float SmoothTime;
        public bool ConstrainX;
        public bool ConstrainY;
        public bool ConstrainZ;

        private Vector3 _velocity = Vector3.zero;

        void Reset()
        {
            SmoothTime = 0.3f;
        }

        void LateUpdate()
        {
            if (Target == null)
                return;

            var v3 = Vector3.SmoothDamp(
                transform.position, Target.position,
                ref _velocity, SmoothTime);

            transform.position = new Vector3(
                ConstrainX ? transform.position.x : v3.x,
                ConstrainY ? transform.position.y : v3.y,
                ConstrainZ ? transform.position.z : v3.z
            );
        }

        public void Follow(Transform target)
        {
            Target = target;
        }

        public void Follow(Vector3 position, bool isInstant = false)
        {
            Target = ObtainProxyObject(position);
            if (isInstant)
                transform.position = new Vector3(
                    ConstrainX ? transform.position.x : Target.position.x,
                    ConstrainY ? transform.position.y : Target.position.y,
                    ConstrainZ ? transform.position.z : Target.position.z
                );
        }

        private GameObject _proxyTarget;

        private Transform ObtainProxyObject(Vector3 position)
        {
            if (_proxyTarget == null)
                _proxyTarget = new GameObject("_folllowerProxy" + name);
            _proxyTarget.transform.position = position;
            return _proxyTarget.transform;
        }

        private void OnDestroy()
        {
            if (_proxyTarget != null)
                Destroy(_proxyTarget);
        }
    }
}