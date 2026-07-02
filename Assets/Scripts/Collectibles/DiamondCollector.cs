using System;
using UnityEngine;

namespace Labyrinth.Collectibles
{
    // Tracks how many diamonds exist in the level and how many have been collected.
    // Diamonds report to this class; it does not know about the player or UI directly.
    public class DiamondCollector : MonoBehaviour
    {
        public int Total { get; private set; }
        public int Collected { get; private set; }
        public bool AllCollected => Total > 0 && Collected >= Total;

        public event Action<int, int> CountChanged;
        public event Action AllDiamondsCollected;

        public void SetTotal(int total)
        {
            Total = total;
            Collected = 0;
            CountChanged?.Invoke(Collected, Total);
        }

        public void NotifyCollected()
        {
            Collected++;
            CountChanged?.Invoke(Collected, Total);

            if (AllCollected)
                AllDiamondsCollected?.Invoke();
        }
    }
}
