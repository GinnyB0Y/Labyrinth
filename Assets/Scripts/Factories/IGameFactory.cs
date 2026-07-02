using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth.Factories
{
    public interface IGameFactory
    {
        Transform CreatePlayer(Vector3 position);
        void CreateDiamonds(IReadOnlyList<Vector3> positions, Transform parent);
        void CreateEnemies(IReadOnlyList<Vector3> spawnPositions, IReadOnlyList<Vector3> patrolPool, int patrolPointsPerEnemy, Transform player, Transform parent);
        void CreateExit(Vector3 position);
    }
}
