using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Labyrinth.Collectibles;
using Labyrinth.Factories;
using Labyrinth.Maze;
using Labyrinth.Rendering;
using Labyrinth.UI;

namespace Labyrinth.Core
{
    public class GameEntryPoint : MonoBehaviour
    {
        [Header("Maze")]
        [SerializeField] private int mazeWidth = 8;
        [SerializeField] private int mazeHeight = 8;
        [SerializeField] private float cellSize = 4f;

        [Header("Gameplay")]
        [SerializeField] private int diamondCount = 7;
        [SerializeField] private int enemyCount = 2;
        [SerializeField] private int patrolPointsPerEnemy = 4;
        [SerializeField] private int randomSeed = 0;

        private void Awake()
        {
            QualitySettings.vSyncCount = 1;

            ServiceLocator.Clear();

            var gameSession = gameObject.AddComponent<GameSession>();
            var diamondCollector = gameObject.AddComponent<DiamondCollector>();
            ServiceLocator.Register(gameSession);
            ServiceLocator.Register(diamondCollector);

            var random = randomSeed == 0 ? new System.Random() : new System.Random(randomSeed);
            ServiceLocator.Register<IGameFactory>(new GameFactory(random));
            ServiceLocator.Register<IUIFactory>(new UIFactory());

            Transform mazeRoot = new GameObject("Maze").transform;
            LabyrinthData maze = LabyrinthGenerator.Generate(mazeWidth, mazeHeight, cellSize, random);
            var builder = new LabyrinthBuilder();
            List<Vector3> allCells = builder.Build(maze, mazeRoot,
                RuntimeMaterialFactory.CreateOpaque(new Color(0.55f, 0.55f, 0.55f)),
                RuntimeMaterialFactory.CreateOpaque(new Color(0.75f, 0.35f, 0.15f)));

            BakeNavMesh(mazeRoot);

            var availableCells = new List<Vector3>(allCells);
            Vector3 startPosition = allCells[0];
            Vector3 exitPosition = allCells[allCells.Count - 1];
            availableCells.Remove(startPosition);
            availableCells.Remove(exitPosition);

            IGameFactory gameFactory = ServiceLocator.Get<IGameFactory>();

            Transform player = gameFactory.CreatePlayer(startPosition);

            int diamondsToSpawn = Mathf.Clamp(diamondCount, 1, availableCells.Count);
            List<Vector3> diamondPositions = CellPicker.TakeRandom(availableCells, diamondsToSpawn, random);
            gameFactory.CreateDiamonds(diamondPositions, mazeRoot);

            int enemiesToSpawn = Mathf.Clamp(enemyCount, 1, availableCells.Count);
            List<Vector3> enemySpawnPoints = CellPicker.TakeRandom(availableCells, enemiesToSpawn, random);
            gameFactory.CreateEnemies(enemySpawnPoints, allCells, patrolPointsPerEnemy, player, mazeRoot);

            gameFactory.CreateExit(exitPosition);

            ServiceLocator.Get<IUIFactory>().CreateHud();
        }

        private static void BakeNavMesh(Transform mazeRoot)
        {
            var surface = mazeRoot.gameObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.BuildNavMesh();
        }
    }
}
