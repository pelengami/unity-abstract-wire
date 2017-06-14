using UnityEngine;

namespace Assets.AbstractWiresEffect.Scripts
{
    [RequireComponent(typeof(CollisionDetector))]
    internal sealed class CollisionSpawner : MonoBehaviour
    {
        public GameObject CollisionEffect;
        public float LifeTime = 1f;

        private bool _isAlreadySpawned;
        private CollisionDetector _collisionDetector;

        private void Start()
        {
            _collisionDetector = GetComponent<CollisionDetector>();
            _collisionDetector.CollisionDetected += OnCollisionDetected;
        }

        private void OnCollisionDetected(Vector3 hit)
        {
            if (_isAlreadySpawned)
                return;

            InstantiateImpactEffect(hit);

            ////todo add decal
            ////InstantiateDecal();
        }

        private void InstantiateImpactEffect(Vector3 pos)
        {
            var impactEffect = Instantiate(CollisionEffect, pos, CollisionEffect.transform.rotation);
            impactEffect.transform.position = pos;

            //todo add ivoke with delay and params
            Destroy(impactEffect, LifeTime);
            Invoke("Reset", LifeTime);

            _isAlreadySpawned = true;
        }

        private void Reset()
        {
            _isAlreadySpawned = false;
        }

        private void OnDestroy()
        {
            _collisionDetector.CollisionDetected -= OnCollisionDetected;
        }
    }
}
