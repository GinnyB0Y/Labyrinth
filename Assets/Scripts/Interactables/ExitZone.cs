using UnityEngine;
using Labyrinth.Collectibles;
using Labyrinth.Core;

namespace Labyrinth.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class ExitZone : MonoBehaviour
    {
        private DiamondCollector _diamondCollector;
        private GameSession _session;

        public void Initialize(DiamondCollector diamondCollector, GameSession session)
        {
            _diamondCollector = diamondCollector;
            _session = session;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_diamondCollector == null || !_diamondCollector.AllCollected) return;

            _session.ReportVictory();
        }
    }
}
