using UnityEngine;
using System.Collections.Generic;
using CarSimulator.AI;

namespace CarSimulator.AI
{
    public class Pedestrian : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_walkSpeed = 2f;
        [SerializeField] private float m_runSpeed = 5f;
        [SerializeField] private float m_wanderRadius = 20f;
        [SerializeField] private float m_pauseDuration = 3f;

        [Header("Behavior")]
        [SerializeField] private bool m_avoidPlayer = true;
        [SerializeField] private float m_avoidDistance = 5f;
        [SerializeField] private float m_lookAheadDistance = 3f;

        [Header("Animation")]
        [SerializeField] private bool m_useAnimation = true;
        [SerializeField] private float m_animationSpeed = 1f;

        private Vector3 m_targetPosition;
        private bool m_isMoving;
        private float m_pauseTimer;
        private Transform m_playerTransform;
        private CharacterController m_controller;

        private enum State { Idle, Walking, Running, Avoiding }
        private State m_currentState = State.Idle;

        private void Start()
        {
            m_controller = GetComponent<CharacterController>();
            if (m_controller == null)
            {
                m_controller = gameObject.AddComponent<CharacterController>();
                m_controller.height = 1.8f;
                m_controller.radius = 0.3f;
                m_controller.center = new Vector3(0, 0.9f, 0);
            }

            FindPlayer();
            PickNewTarget();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
            }

            UpdateState();
            Move();
            Animate();
        }

        private void UpdateState()
        {
            if (m_avoidPlayer && m_playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
                
                if (distToPlayer < m_avoidDistance)
                {
                    m_currentState = State.Avoiding;
                    return;
                }
            }

            if (m_isMoving)
            {
                float distToTarget = Vector3.Distance(transform.position, m_targetPosition);
                
                if (distToTarget < 1f)
                {
                    m_isMoving = false;
                    m_currentState = State.Idle;
                    m_pauseTimer = m_pauseDuration;
                }
                else
                {
                    m_currentState = State.Walking;
                }
            }
            else
            {
                m_pauseTimer -= Time.deltaTime;
                
                if (m_pauseTimer <= 0)
                {
                    PickNewTarget();
                }
            }
        }

        private void Move()
        {
            Vector3 moveDir = Vector3.zero;

            switch (m_currentState)
            {
                case State.Walking:
                    moveDir = (m_targetPosition - transform.position).normalized;
                    break;

                case State.Running:
                    moveDir = (m_targetPosition - transform.position).normalized;
                    break;

                case State.Avoiding:
                    if (m_playerTransform != null)
                    {
                        Vector3 awayDir = transform.position - m_playerTransform.position;
                        awayDir.y = 0;
                        moveDir = awayDir.normalized;
                    }
                    break;

                case State.Idle:
                default:
                    break;
            }

            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 5f);
            }

            float speed = m_currentState == State.Running ? m_runSpeed : m_walkSpeed;
            
            if (m_controller != null)
            {
                Vector3 gravity = Vector3.down * 9.81f;
                m_controller.Move(moveDir * speed * Time.deltaTime);
                m_controller.Move(gravity * Time.deltaTime);
            }
            else
            {
                transform.position += moveDir * speed * Time.deltaTime;
            }
        }

        private void Animate()
        {
            if (!m_useAnimation) return;

            float animSpeed = m_currentState switch
            {
                State.Walking => m_walkSpeed,
                State.Running => m_runSpeed,
                _ => 0f
            };
        }

        private void PickNewTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * m_wanderRadius;
            m_targetPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            m_isMoving = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_wanderRadius);

            if (m_avoidPlayer)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, m_avoidDistance);
            }
        }
    }
}
