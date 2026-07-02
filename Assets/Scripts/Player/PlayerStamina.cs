using System;
using UnityEngine;

namespace Labyrinth.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        [SerializeField] private float sprintDuration = 3f;
        [SerializeField] private float rechargeDuration = 5f;

        private float _fraction = 1f;
        private bool _isLocked;

        public event Action<float> StaminaChanged;

        public float Fraction => _fraction;

        public bool Tick(float deltaTime, bool wantsSprint, bool isMoving)
        {
            bool canSprint = !_isLocked && _fraction > 0f;
            bool isSprinting = wantsSprint && isMoving && canSprint;
            float rate = isSprinting ? -1f / sprintDuration : 1f / rechargeDuration;

            float previous = _fraction;
            _fraction = Mathf.Clamp01(_fraction + rate * deltaTime);

            if (_fraction <= 0f) _isLocked = true;
            else if (_fraction >= 1f) _isLocked = false;

            if (!Mathf.Approximately(previous, _fraction))
                StaminaChanged?.Invoke(_fraction);

            return isSprinting;
        }
    }
}
