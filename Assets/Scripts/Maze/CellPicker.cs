using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth.Maze
{
    public static class CellPicker
    {
        public static List<Vector3> TakeRandom(List<Vector3> pool, int count, System.Random random)
        {
            var result = new List<Vector3>();
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int index = random.Next(pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }
            return result;
        }
    }
}
