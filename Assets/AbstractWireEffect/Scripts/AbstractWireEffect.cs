using UnityEngine;

namespace Assets.AbstractWireEffect.Scripts
{
    internal sealed class AbstractWireEffect : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
        public GameObject LineRendererPrefab;
        public float MaxDistance;
        public int MaxConnections;
        public int MaxLineRenderers;

        private ParticleSystem.MainModule _particleSystemMainModule;
        private ParticleSystem.Particle[] _particles;
        private LineRenderer[] _lineRenderers;
        private Transform _psTransform;

        public void SetParticleSystem(ParticleSystem psSystem)
        {
            if (psSystem == null)
                return;

            _particleSystemMainModule = psSystem.main;
            _psTransform = psSystem.GetComponent<Transform>();

            int maxParticles = _particleSystemMainModule.maxParticles;
            _particles = new ParticleSystem.Particle[maxParticles];

            ParticleSystem = psSystem;
        }

        public void ClearOutdated()
        {
            foreach (var lineRenderer in _lineRenderers)
                if (lineRenderer != null)
                    Destroy(lineRenderer);
        }

        private void Start()
        {
            SetParticleSystem(ParticleSystem);
            _lineRenderers = new LineRenderer[MaxLineRenderers];
        }

        private void Stop()
        {
            ClearOutdated();
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnEnable()
        {
            Start();
        }

        private void LateUpdate()
        {
            if (ParticleSystem == null || _lineRenderers == null)
                return;

            var liveParticles = ParticleSystem.GetParticles(_particles);
            var lineRenderersCount = _lineRenderers.Length;

            var lrIndex = 0;
            for (int i = 0; i < liveParticles; i++)
            {
                var p1Pos = _particles[i].position;

                var particleConnections = 0;

                for (int j = i + 1; j < liveParticles; j++)
                {
                    if (particleConnections == MaxConnections || lrIndex == MaxLineRenderers)
                        break;

                    var p2Pos = _particles[j].position;

                    float distance = Vector3.Distance(p2Pos, p1Pos);
                    if (distance > MaxDistance)
                        continue;

                    LineRenderer lineRenderer;

                    if (_lineRenderers[lrIndex] == null)
                    {
                        lineRenderer = Instantiate(LineRendererPrefab, _psTransform.position, _psTransform.rotation)
                            .GetComponent<LineRenderer>();
                        lineRenderer.transform.parent = ParticleSystem.transform;
                        _lineRenderers[lrIndex] = lineRenderer;
                    }

                    lineRenderer = _lineRenderers[lrIndex];

                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, p1Pos);
                    lineRenderer.SetPosition(1, p2Pos);

                    lrIndex++;
                    particleConnections++;
                }
            }

            var outdatedLineRenderers = lineRenderersCount - lrIndex;

            for (int i = lrIndex; i < outdatedLineRenderers; i++)
            {
                if (_lineRenderers[i] != null)
                    _lineRenderers[i].enabled = false;
            }
        }
    }
}