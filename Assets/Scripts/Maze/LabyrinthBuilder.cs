using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth.Maze
{
    public class LabyrinthBuilder
    {
        private readonly float _wallHeight;
        private readonly float _wallThickness;

        public LabyrinthBuilder(float wallHeight = 3f, float wallThickness = 0.2f)
        {
            _wallHeight = wallHeight;
            _wallThickness = wallThickness;
        }

        public List<Vector3> Build(LabyrinthData maze, Transform parent, Material floorMaterial, Material wallMaterial)
        {
            BuildFloor(maze, parent, floorMaterial);
            BuildWalls(maze, parent, wallMaterial);

            var cellCenters = new List<Vector3>();
            for (int x = 0; x < maze.Width; x++)
            for (int y = 0; y < maze.Height; y++)
                cellCenters.Add(maze.CellToWorld(x, y));
            return cellCenters;
        }

        private static void BuildFloor(LabyrinthData maze, Transform parent, Material material)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(parent, false);

            float width = maze.Width * maze.CellSize;
            float depth = maze.Height * maze.CellSize;
            floor.transform.localScale = new Vector3(width, 0.2f, depth);
            floor.transform.position = new Vector3(width / 2f, -0.1f, depth / 2f);
            floor.GetComponent<MeshRenderer>().material = material;
        }

        private void BuildWalls(LabyrinthData maze, Transform parent, Material material)
        {
            Transform wallsRoot = new GameObject("Walls").transform;
            wallsRoot.SetParent(parent, false);

            float half = maze.CellSize / 2f;

            for (int x = 0; x < maze.Width; x++)
            {
                for (int y = 0; y < maze.Height; y++)
                {
                    Vector3 center = maze.CellToWorld(x, y);

                    if (maze.HasWallNorth(x, y))
                        PlaceWall(wallsRoot, material, center + new Vector3(0f, 0f, half), new Vector3(maze.CellSize, _wallHeight, _wallThickness));

                    if (y == 0)
                        PlaceWall(wallsRoot, material, center - new Vector3(0f, 0f, half), new Vector3(maze.CellSize, _wallHeight, _wallThickness));

                    if (maze.HasWallEast(x, y))
                        PlaceWall(wallsRoot, material, center + new Vector3(half, 0f, 0f), new Vector3(_wallThickness, _wallHeight, maze.CellSize));

                    if (x == 0)
                        PlaceWall(wallsRoot, material, center - new Vector3(half, 0f, 0f), new Vector3(_wallThickness, _wallHeight, maze.CellSize));
                }
            }
        }

        private void PlaceWall(Transform parent, Material material, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall";
            wall.transform.SetParent(parent, false);
            wall.transform.localScale = scale;
            wall.transform.position = position + Vector3.up * (_wallHeight / 2f);
            wall.GetComponent<MeshRenderer>().material = material;
        }
    }
}
