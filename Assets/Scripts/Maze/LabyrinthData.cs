using UnityEngine;

namespace Labyrinth.Maze
{
    public class LabyrinthData
    {
        private readonly bool[,] _wallsNorth;
        private readonly bool[,] _wallsEast; 

        public LabyrinthData(int width, int height, float cellSize, bool[,] wallsNorth, bool[,] wallsEast)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            _wallsNorth = wallsNorth;
            _wallsEast = wallsEast;
        }

        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public bool HasWallNorth(int x, int y) => _wallsNorth[x, y];
        public bool HasWallEast(int x, int y) => _wallsEast[x, y];

        public Vector3 CellToWorld(int x, int y)
        {
            return new Vector3((x + 0.5f) * CellSize, 0f, (y + 0.5f) * CellSize);
        }
    }
}
