using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Labyrinth.Core
{
    public class GameSession : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Playing;
        public bool IsPlaying => State == GameState.Playing;

        public event Action<GameState> StateChanged;

        public void ReportVictory()
        {
            if (!IsPlaying) return;
            SetState(GameState.Won);
        }

        public void ReportDefeat()
        {
            if (!IsPlaying) return;
            SetState(GameState.Lost);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetState(GameState newState)
        {
            State = newState;
            StateChanged?.Invoke(newState);
        }
    }
}
