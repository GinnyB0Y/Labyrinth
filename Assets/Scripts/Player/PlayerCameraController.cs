using UnityEngine;
using UnityEngine.InputSystem;
using Labyrinth.Core;

namespace Labyrinth.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private Transform _playerBody;
        private GameSession _session;
        private float _pitch;

        public void Initialize(Transform playerBody, GameSession session)
        {
            _playerBody = playerBody;
            _session = session;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_session != null && !_session.IsPlaying) return;

            Mouse mouse = Mouse.current;
            if (mouse == null || _playerBody == null) return;

            Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;

            _pitch = Mathf.Clamp(_pitch - delta.y, minPitch, maxPitch);
            transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            _playerBody.Rotate(Vector3.up * delta.x);
        }
    }
}
