using UnityEngine;

namespace CarSimulator.Optimization
{
    public class CachedComponent<T> where T : Component
    {
        private T m_component;
        private bool m_isCached;

        public T Component
        {
            get
            {
                if (!m_isCached)
                {
                    m_component = GetComponent<T>();
                    m_isCached = true;
                }
                return m_component;
            }
        }

        public bool HasComponent => m_component != null || !m_isCached ? GetComponent<T>() != null : m_component != null;

        public CachedComponent(MonoBehaviour owner)
        {
            m_component = owner.GetComponent<T>();
            m_isCached = true;
        }

        public void Invalidate()
        {
            m_isCached = false;
            m_component = null;
        }
    }

    public class CachedTransform
    {
        private Transform m_transform;
        private bool m_isCached;

        public Transform Transform
        {
            get
            {
                if (!m_isCached)
                {
                    m_transform = GetComponent<Transform>();
                    m_isCached = true;
                }
                return m_transform;
            }
        }

        public Vector3 Position => Transform.position;
        public Vector3 Forward => Transform.forward;
        public Vector3 Right => Transform.right;

        public CachedTransform(MonoBehaviour owner)
        {
            m_transform = owner.transform;
            m_isCached = true;
        }
    }

    public class CachedReference<T> where T : class
    {
        private T m_reference;
        private bool m_isCached;
        private readonly System.Func<T> m_getter;

        public T Value
        {
            get
            {
                if (!m_isCached)
                {
                    m_reference = m_getter?.Invoke();
                    m_isCached = true;
                }
                return m_reference;
            }
        }

        public bool IsValid => Value != null;

        public CachedReference(System.Func<T> getter)
        {
            m_getter = getter;
        }

        public void Invalidate()
        {
            m_isCached = false;
            m_reference = null;
        }

        public void Refresh()
        {
            Invalidate();
            _ = Value;
        }
    }

    public class Timer
    {
        private float m_elapsed;
        private float m_interval;
        private bool m_enabled = true;

        public float Interval
        {
            get => m_interval;
            set => m_interval = value;
        }

        public bool Enabled
        {
            get => m_enabled;
            set => m_enabled = value;
        }

        public float Elapsed => m_elapsed;

        public Timer(float interval)
        {
            m_interval = interval;
        }

        public bool Tick(float deltaTime)
        {
            if (!m_enabled) return false;

            m_elapsed += deltaTime;

            if (m_elapsed >= m_interval)
            {
                m_elapsed -= m_interval;
                return true;
            }

            return false;
        }

        public bool TickUnscaled(float deltaTime)
        {
            if (!m_enabled) return false;

            m_elapsed += deltaTime;

            if (m_elapsed >= m_interval)
            {
                m_elapsed -= m_interval;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            m_elapsed = 0;
        }

        public void SetInterval(float interval)
        {
            m_interval = interval;
        }
    }

    public class UpdateThrottler
    {
        private float m_nextUpdate;
        private float m_interval;
        private readonly float m_initialDelay;

        public UpdateThrottler(float interval, float initialDelay = 0f)
        {
            m_interval = interval;
            m_initialDelay = initialDelay;
            m_nextUpdate = initialDelay > 0 ? -1 : 0;
        }

        public bool ShouldUpdate(float deltaTime)
        {
            if (m_nextUpdate < 0)
            {
                m_nextUpdate = 0;
                return true;
            }

            m_nextUpdate += deltaTime;

            if (m_nextUpdate >= m_interval)
            {
                m_nextUpdate -= m_interval;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            m_nextUpdate = m_initialDelay > 0 ? -1 : 0;
        }

        public void SetInterval(float interval)
        {
            m_interval = interval;
        }
    }

    public class DistanceChecker
    {
        private Transform m_target;
        private float m_distanceSqr;
        private bool m_lastResult;

        public DistanceChecker(Transform target, float distance)
        {
            m_target = target;
            m_distanceSqr = distance * distance;
        }

        public bool IsWithinDistance(Vector3 position)
        {
            m_lastResult = m_target != null && (position - m_target.position).sqrMagnitude <= m_distanceSqr;
            return m_lastResult;
        }

        public bool WasWithinDistance => m_lastResult;

        public void SetTarget(Transform target)
        {
            m_target = target;
        }

        public void SetDistance(float distance)
        {
            m_distanceSqr = distance * distance;
        }
    }

    public abstract class OptimizedMonoBehaviour : MonoBehaviour
    {
        protected Timer UpdateTimer { get; private set; }
        protected Timer FixedTimer { get; private set; }
        protected Timer LateTimer { get; private set; }

        protected virtual void InitializeTimers(float updateInterval = 0.1f, float fixedInterval = 0.05f, float lateInterval = 0.1f)
        {
            UpdateTimer = new Timer(updateInterval);
            FixedTimer = new Timer(fixedInterval);
            LateTimer = new Timer(lateInterval);
        }

        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }

        protected virtual void ManagedUpdate()
        {
            if (UpdateTimer != null && UpdateTimer.Tick(Time.deltaTime))
            {
                OnUpdate();
            }
        }

        protected virtual void ManagedFixedUpdate()
        {
            if (FixedTimer != null && FixedTimer.Tick(Time.fixedDeltaTime))
            {
                OnFixedUpdate();
            }
        }

        protected virtual void ManagedLateUpdate()
        {
            if (LateTimer != null && LateTimer.Tick(Time.deltaTime))
            {
                OnLateUpdate();
            }
        }
    }
}
