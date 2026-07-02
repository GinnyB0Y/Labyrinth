using System.Collections.Generic;
using UnityEngine;
using Labyrinth.Collectibles;
using Labyrinth.Core;
using Labyrinth.Interactables;
using Labyrinth.Maze;
using Labyrinth.Player;
using Labyrinth.Rendering;

namespace Labyrinth.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly System.Random _random;

        public GameFactory(System.Random random)
        {
            _random = random;
        }

        public Transform CreatePlayer(Vector3 position)
        {
            var session = ServiceLocator.Get<GameSession>();

            var playerGO = new GameObject("Player") { tag = "Player" };
            playerGO.transform.position = position;

            var controller = playerGO.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            var rb = playerGO.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var stamina = playerGO.AddComponent<PlayerStamina>();
            ServiceLocator.Register(stamina);

            var motor = playerGO.AddComponent<PlayerMovement>();
            motor.Initialize(session);

            var cameraGO = new GameObject("PlayerCamera");
            cameraGO.transform.SetParent(playerGO.transform, false);
            cameraGO.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();

            var look = cameraGO.AddComponent<PlayerCameraController>();
            look.Initialize(playerGO.transform, session);

            return playerGO.transform;
        }

        public void CreateDiamonds(IReadOnlyList<Vector3> positions, Transform parent)
        {
            var collector = ServiceLocator.Get<DiamondCollector>();
            collector.SetTotal(positions.Count);

            foreach (Vector3 position in positions)
                DiamondFactory.Create(position + Vector3.up, collector, parent);
        }

        public void CreateEnemies(IReadOnlyList<Vector3> spawnPositions, IReadOnlyList<Vector3> patrolPool, int patrolPointsPerEnemy, Transform player, Transform parent)
        {
            var session = ServiceLocator.Get<GameSession>();
            int patrolCount = Mathf.Clamp(patrolPointsPerEnemy, 1, patrolPool.Count);

            foreach (Vector3 spawnPosition in spawnPositions)
            {
                var patrolRoute = CellPicker.TakeRandom(new List<Vector3>(patrolPool), patrolCount, _random).ToArray();
                EnemyFactory.Create(spawnPosition, patrolRoute, player, session, parent);
            }
        }

        public void CreateExit(Vector3 position)
        {
            var collector = ServiceLocator.Get<DiamondCollector>();
            var session = ServiceLocator.Get<GameSession>();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Exit";
            go.transform.position = position + Vector3.up * 0.05f;
            go.transform.localScale = new Vector3(1.6f, 0.05f, 1.6f);

            Object.Destroy(go.GetComponent<Collider>());
            var trigger = go.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, 40f, 1f);

            go.GetComponent<MeshRenderer>().material = RuntimeMaterialFactory.CreateEmissive(new Color(0.2f, 1f, 0.3f));

            ExitZone exitZone = go.AddComponent<ExitZone>();
            exitZone.Initialize(collector, session);
        }
    }
}
