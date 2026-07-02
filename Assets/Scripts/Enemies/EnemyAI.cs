using System;
using UnityEngine;
using UnityEngine.AI;
using Labyrinth.Core;

namespace Labyrinth.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            Patrol,
            Chase
        }

        [SerializeField] private float detectionRadius = 6f;
        [SerializeField] private float loseInterestRadius = 9f;
        [SerializeField] private float waypointTolerance = 0.3f;
        [SerializeField] private float chaseRepathInterval = 0.2f;

        private NavMeshAgent _agent;
        private Transform _player;
        private Vector3[] _patrolPoints;
        private Action _onCatchPlayer;
        private GameSession _session;

        private State _state = State.Patrol;
        private int _patrolIndex;
        private float _repathTimer;
        private bool _hasCaughtPlayer;

        public void Initialize(Transform player, Vector3[] patrolPoints, Action onCatchPlayer, GameSession session)
        {
            _player = player;
            _patrolPoints = patrolPoints;
            _onCatchPlayer = onCatchPlayer;
            _session = session;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            if (_patrolPoints != null && _patrolPoints.Length > 0)
                _agent.SetDestination(_patrolPoints[0]);
        }

        private void Update()
        {
            if (_session != null && !_session.IsPlaying)
            {
                _agent.isStopped = true;
                return;
            }

            if (_player == null || _hasCaughtPlayer) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (_state == State.Patrol && distanceToPlayer <= detectionRadius)
                _state = State.Chase;
            else if (_state == State.Chase && distanceToPlayer > loseInterestRadius)
                _state = State.Patrol;

            if (_state == State.Patrol)
                UpdatePatrol();
            else
                UpdateChase();
        }

        private void UpdatePatrol()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0) return;
            if (_agent.pathPending || _agent.remainingDistance > waypointTolerance) return;

            _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
            _agent.SetDestination(_patrolPoints[_patrolIndex]);
        }

        private void UpdateChase()
        {
            _repathTimer -= Time.deltaTime;
            if (_repathTimer > 0f) return;

            _repathTimer = chaseRepathInterval;
            _agent.SetDestination(_player.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasCaughtPlayer) return;
            if (!other.CompareTag("Player")) return;

            _hasCaughtPlayer = true;
            _agent.isStopped = true;
            _onCatchPlayer?.Invoke();
        }
    }
}
