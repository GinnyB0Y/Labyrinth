using UnityEngine;

namespace Labyrinth.Collectibles
{
    [RequireComponent(typeof(Collider))]
    public class Diamond : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float bobAmplitude = 0.15f;
        [SerializeField] private float bobSpeed = 2f;

        private DiamondCollector _collector;
        private Vector3 _basePosition;

        public void Initialize(DiamondCollector collector)
        {
            _collector = collector;
        }

        private void Start()
        {
            _basePosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = _basePosition + Vector3.up * bobOffset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _collector?.NotifyCollected();
            Destroy(gameObject);
        }
    }
}
