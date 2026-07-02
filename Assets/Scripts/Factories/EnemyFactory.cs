using UnityEngine;
using UnityEngine.AI;
using Labyrinth.Core;
using Labyrinth.Enemies;
using Labyrinth.Rendering;

namespace Labyrinth.Factories
{
    public static class EnemyFactory
    {
        public static EnemyAI Create(Vector3 spawnPosition, Vector3[] patrolPoints, Transform player, GameSession session, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy";
            go.transform.SetParent(parent, false);
            go.transform.position = spawnPosition + Vector3.up;

            var collider = go.GetComponent<CapsuleCollider>();
            collider.isTrigger = true;
            go.GetComponent<MeshRenderer>().material = CreateEnemyMaterial();

            NavMeshAgent agent = go.AddComponent<NavMeshAgent>();
            agent.radius = 0.4f;
            agent.height = 2f;
            agent.speed = 3.5f;
            agent.acceleration = 12f;
            agent.baseOffset = 1f;

            EnemyAI enemy = go.AddComponent<EnemyAI>();
            enemy.Initialize(player, patrolPoints, session.ReportDefeat, session);
            return enemy;
        }

        private static Material CreateEnemyMaterial()
        {
            return RuntimeMaterialFactory.CreateOpaque(new Color(0.85f, 0.1f, 0.1f));
        }
    }
}
