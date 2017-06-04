using UnityEngine;

namespace Assets.AbstractWireEffect.Scripts
{
    [RequireComponent(typeof(AbstractWireEffect))]
    internal sealed class CollisionEffectSpawner : MonoBehaviour
    {
        public float MaxDistance;
        public GameObject CollisionEffect;
        public float LifeTime = 1f;

        private bool _spawned;
        private Transform _transform;
        private AbstractWireEffect _abstractWireEffect;

        private void Start()
        {
            _transform = transform;
            _abstractWireEffect = GetComponent<AbstractWireEffect>();
        }

        private void Update()
        {
            RaycastHit hit;
            if (!Physics.Raycast(_transform.position, -_transform.up, out hit, 1))
                return;

            var distance = Vector3.Distance(_transform.position, hit.point);

            if (distance >= MaxDistance)
            {
                _spawned = false;
                return;
            }

            if (_spawned)
                return;

            var impactEffect = Instantiate(CollisionEffect, hit.point, CollisionEffect.transform.rotation);
            _abstractWireEffect.SetParticleSystem(impactEffect.GetComponent<ParticleSystem>());
            Destroy(impactEffect, LifeTime);

            _spawned = true;
        }
    }
}
