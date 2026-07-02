using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth.Maze
{
    public static class LabyrinthGenerator
    {
        public static LabyrinthData Generate(int width, int height, float cellSize, System.Random random)
        {
            var wallsNorth = new bool[width, height];
            var wallsEast = new bool[width, height];
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                wallsNorth[x, y] = true;
                wallsEast[x, y] = true;
            }

            var visited = new bool[width, height];
            var stack = new Stack<Vector2Int>();
            var start = new Vector2Int(0, 0);
            visited[0, 0] = true;
            stack.Push(start);

            while (stack.Count > 0)
            {
                Vector2Int current = stack.Peek();
                List<Vector2Int> neighbours = GetUnvisitedNeighbours(current, width, height, visited);

                if (neighbours.Count == 0)
                {
                    stack.Pop();
                    continue;
                }

                Vector2Int next = neighbours[random.Next(neighbours.Count)];
                RemoveWallBetween(current, next, wallsNorth, wallsEast);
                visited[next.x, next.y] = true;
                stack.Push(next);
            }

            return new LabyrinthData(width, height, cellSize, wallsNorth, wallsEast);
        }

        private static List<Vector2Int> GetUnvisitedNeighbours(Vector2Int cell, int width, int height, bool[,] visited)
        {
            var result = new List<Vector2Int>();
            TryAdd(cell + Vector2Int.up, width, height, visited, result);
            TryAdd(cell + Vector2Int.down, width, height, visited, result);
            TryAdd(cell + Vector2Int.left, width, height, visited, result);
            TryAdd(cell + Vector2Int.right, width, height, visited, result);
            return result;
        }

        private static void TryAdd(Vector2Int cell, int width, int height, bool[,] visited, List<Vector2Int> result)
        {
            if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) return;
            if (visited[cell.x, cell.y]) return;
            result.Add(cell);
        }

        private static void RemoveWallBetween(Vector2Int a, Vector2Int b, bool[,] wallsNorth, bool[,] wallsEast)
        {
            if (b.x == a.x && b.y == a.y + 1) wallsNorth[a.x, a.y] = false;
            else if (b.x == a.x && b.y == a.y - 1) wallsNorth[b.x, b.y] = false;
            else if (b.x == a.x + 1 && b.y == a.y) wallsEast[a.x, a.y] = false;
            else if (b.x == a.x - 1 && b.y == a.y) wallsEast[b.x, b.y] = false;
        }
    }
}
