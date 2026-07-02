using UnityEngine;
using UnityEngine.InputSystem;
using Labyrinth.Core;

namespace Labyrinth.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStamina))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.7f;
        [SerializeField] private float gravity = -20f;

        private CharacterController _controller;
        private PlayerStamina _stamina;
        private GameSession _session;
        private float _verticalVelocity;

        public void Initialize(GameSession session)
        {
            _session = session;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _stamina = GetComponent<PlayerStamina>();
        }

        private void Update()
        {
            if (_session != null && !_session.IsPlaying) return;

            Vector2 input = ReadMoveInput();
            bool isMoving = input.sqrMagnitude > 0.0001f;
            bool isSprinting = _stamina.Tick(Time.deltaTime, IsSprintHeld(), isMoving);

            float speed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
            Vector3 move = (transform.right * input.x + transform.forward * input.y) * speed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            _verticalVelocity += gravity * Time.deltaTime;
            move.y = _verticalVelocity;

            _controller.Move(move * Time.deltaTime);
        }

        private static Vector2 ReadMoveInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return Vector2.zero;

            Vector2 input = Vector2.zero;
            if (keyboard.wKey.isPressed) input.y += 1f;
            if (keyboard.sKey.isPressed) input.y -= 1f;
            if (keyboard.dKey.isPressed) input.x += 1f;
            if (keyboard.aKey.isPressed) input.x -= 1f;
            return input.normalized;
        }

        private static bool IsSprintHeld()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.shiftKey.isPressed;
        }
    }
}
