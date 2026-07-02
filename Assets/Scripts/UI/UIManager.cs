using UnityEngine;
using UnityEngine.UI;
using Labyrinth.Collectibles;
using Labyrinth.Core;
using Labyrinth.Player;

namespace Labyrinth.UI
{
    public class UIManager : MonoBehaviour
    {
        private Text _diamondCounterText;
        private Image _staminaFillImage;
        private GameObject _winPanel;
        private Text _winCounterText;
        private GameObject _losePanel;
        private DiamondCollector _diamondCollector;
        private GameSession _session;
        private PlayerStamina _stamina;

        public void Initialize(
            Text diamondCounterText,
            Image staminaFillImage,
            GameObject winPanel,
            Text winCounterText,
            GameObject losePanel,
            Button winRestartButton,
            Button loseRestartButton,
            DiamondCollector diamondCollector,
            GameSession session,
            PlayerStamina stamina)
        {
            _diamondCounterText = diamondCounterText;
            _staminaFillImage = staminaFillImage;
            _winPanel = winPanel;
            _winCounterText = winCounterText;
            _losePanel = losePanel;
            _diamondCollector = diamondCollector;
            _session = session;
            _stamina = stamina;

            _winPanel.SetActive(false);
            _losePanel.SetActive(false);

            winRestartButton.onClick.AddListener(_session.RestartLevel);
            loseRestartButton.onClick.AddListener(_session.RestartLevel);

            _diamondCollector.CountChanged += OnDiamondCountChanged;
            _stamina.StaminaChanged += OnStaminaChanged;
            _session.StateChanged += OnGameStateChanged;

            OnDiamondCountChanged(_diamondCollector.Collected, _diamondCollector.Total);
            OnStaminaChanged(_stamina.Fraction);
        }

        private void OnDiamondCountChanged(int collected, int total)
        {
            _diamondCounterText.text = $"Diamonds: {collected} / {total}";
        }

        private void OnStaminaChanged(float fraction)
        {
            _staminaFillImage.fillAmount = fraction;
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Won:
                    _winCounterText.text = $"Diamonds collected: {_diamondCollector.Collected} / {_diamondCollector.Total}";
                    _winPanel.SetActive(true);
                    ReleaseCursor();
                    break;
                case GameState.Lost:
                    _losePanel.SetActive(true);
                    ReleaseCursor();
                    break;
            }
        }

        private static void ReleaseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDestroy()
        {
            if (_diamondCollector != null) _diamondCollector.CountChanged -= OnDiamondCountChanged;
            if (_stamina != null) _stamina.StaminaChanged -= OnStaminaChanged;
            if (_session != null) _session.StateChanged -= OnGameStateChanged;
        }
    }
}
