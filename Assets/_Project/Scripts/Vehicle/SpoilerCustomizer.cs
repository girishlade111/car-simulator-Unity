using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class SpoilerCustomizer : MonoBehaviour
    {
        [Header("Spoiler Settings")]
        [SerializeField] private bool m_spoilerEnabled = true;
        [SerializeField] private SpoilerType m_currentSpoiler = SpoilerType.None;

        [Header("Spoiler Parts")]
        [SerializeField] private GameObject m_spoilerWing;
        [SerializeField] private GameObject m_spoilerStandL;
        [SerializeField] private GameObject m_spoilerStandR;
        [SerializeField] private GameObject m_spoilerEndplateL;
        [SerializeField] private GameObject m_spoilerEndplateR;

        [Header("References")]
        [SerializeField] private Transform m_rearMountPoint;

        public enum SpoilerType
        {
            None,
            Lip,
            Ducktail,
            Wing,
            RacingWing,
            GTWing
        }

        private Dictionary<SpoilerType, SpoilerData> m_spoilerData = new Dictionary<SpoilerType, SpoilerData>();

        [System.Serializable]
        public class SpoilerData
        {
            public string name;
            public int price;
            public float downforceBonus;
            public float dragPenalty;
            public float visualScale = 1f;
        }

        private void Start()
        {
            InitializeSpoilers();
            if (m_spoilerEnabled)
            {
                ApplySpoiler(m_currentSpoiler);
            }
        }

        private void InitializeSpoilers()
        {
            m_spoilerData = new Dictionary<SpoilerType, SpoilerData>
            {
                { SpoilerType.None, new SpoilerData { name = "None", price = 0, downforceBonus = 0, dragPenalty = 0 } },
                { SpoilerType.Lip, new SpoilerData { name = "Lip Spoiler", price = 300, downforceBonus = 0.05f, dragPenalty = 0.01f } },
                { SpoilerType.Ducktail, new SpoilerData { name = "Ducktail", price = 500, downforceBonus = 0.08f, dragPenalty = 0.02f } },
                { SpoilerType.Wing, new SpoilerData { name = "Sport Wing", price = 800, downforceBonus = 0.15f, dragPenalty = 0.05f } },
                { SpoilerType.RacingWing, new SpoilerData { name = "Racing Wing", price = 1500, downforceBonus = 0.25f, dragPenalty = 0.08f } },
                { SpoilerType.GTWing, new SpoilerData { name = "GT Wing", price = 2500, downforceBonus = 0.4f, dragPenalty = 0.12f } }
            };
        }

        public void ApplySpoiler(SpoilerType spoilerType)
        {
            m_currentSpoiler = spoilerType;
            ClearSpoiler();

            if (spoilerType == SpoilerType.None) return;

            switch (spoilerType)
            {
                case SpoilerType.Lip:
                    CreateLipSpoiler();
                    break;
                case SpoilerType.Ducktail:
                    CreateDucktail();
                    break;
                case SpoilerType.Wing:
                    CreateSportWing();
                    break;
                case SpoilerType.RacingWing:
                    CreateRacingWing();
                    break;
                case SpoilerType.GTWing:
                    CreateGTWing();
                    break;
            }

            ApplySpoilerStats(spoilerType);
        }

        private void ClearSpoiler()
        {
            if (m_spoilerWing != null) { Destroy(m_spoilerWing); m_spoilerWing = null; }
            if (m_spoilerStandL != null) { Destroy(m_spoilerStandL); m_spoilerStandL = null; }
            if (m_spoilerStandR != null) { Destroy(m_spoilerStandR); m_spoilerStandR = null; }
            if (m_spoilerEndplateL != null) { Destroy(m_spoilerEndplateL); m_spoilerEndplateL = null; }
            if (m_spoilerEndplateR != null) { Destroy(m_spoilerEndplateR); m_spoilerEndplateR = null; }
        }

        private void CreateLipSpoiler()
        {
            m_spoilerWing = CreateCube("LipSpoiler", new Vector3(0, 1f, -1.9f), new Vector3(1.8f, 0.1f, 0.3f));
        }

        private void CreateDucktail()
        {
            m_spoilerWing = CreateCube("Ducktail", new Vector3(0, 1.1f, -1.7f), new Vector3(1.2f, 0.3f, 0.6f));
        }

        private void CreateSportWing()
        {
            m_spoilerWing = CreateCube("Wing", new Vector3(0, 1.4f, -1.85f), new Vector3(1.6f, 0.08f, 0.4f));
            m_spoilerStandL = CreateCube("StandL", new Vector3(-0.6f, 1.1f, -1.85f), new Vector3(0.1f, 0.3f, 0.1f));
            m_spoilerStandR = CreateCube("StandR", new Vector3(0.6f, 1.1f, -1.85f), new Vector3(0.1f, 0.3f, 0.1f));
        }

        private void CreateRacingWing()
        {
            m_spoilerWing = CreateCube("RacingWing", new Vector3(0, 1.6f, -1.85f), new Vector3(1.8f, 0.1f, 0.5f));
            m_spoilerStandL = CreateCube("StandL", new Vector3(-0.7f, 1.2f, -1.85f), new Vector3(0.12f, 0.4f, 0.12f));
            m_spoilerStandR = CreateCube("StandR", new Vector3(0.7f, 1.2f, -1.85f), new Vector3(0.12f, 0.4f, 0.12f));
            m_spoilerEndplateL = CreateCube("EndplateL", new Vector3(-0.95f, 1.6f, -1.85f), new Vector3(0.1f, 0.5f, 0.5f));
            m_spoilerEndplateR = CreateCube("EndplateR", new Vector3(0.95f, 1.6f, -1.85f), new Vector3(0.1f, 0.5f, 0.5f));
        }

        private void CreateGTWing()
        {
            m_spoilerWing = CreateCube("GTWing", new Vector3(0, 1.8f, -1.9f), new Vector3(2f, 0.12f, 0.6f));
            m_spoilerStandL = CreateCube("StandL", new Vector3(-0.8f, 1.4f, -1.9f), new Vector3(0.15f, 0.45f, 0.15f));
            m_spoilerStandR = CreateCube("StandR", new Vector3(0.8f, 1.4f, -1.9f), new Vector3(0.15f, 0.45f, 0.15f));
            m_spoilerEndplateL = CreateCube("EndplateL", new Vector3(-1.05f, 1.8f, -1.9f), new Vector3(0.12f, 0.6f, 0.6f));
            m_spoilerEndplateR = CreateCube("EndplateR", new Vector3(1.05f, 1.8f, -1.9f), new Vector3(0.12f, 0.6f, 0.6f));
        }

        private GameObject CreateCube(string name, Vector3 pos, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            return obj;
        }

        private void ApplySpoilerStats(SpoilerType type)
        {
            if (!m_spoilerData.ContainsKey(type)) return;

            SpoilerData data = m_spoilerData[type];
            var physics = GetComponent<VehiclePhysics>();
            if (physics != null && physics.m_tuning != null)
            {
                physics.m_tuning.downforce += data.downforceBonus * 10f;
            }
        }

        public SpoilerData GetCurrentSpoilerData()
        {
            return m_spoilerData.ContainsKey(m_currentSpoiler) ? m_spoilerData[m_currentSpoiler] : null;
        }

        public SpoilerType GetCurrentSpoiler() => m_currentSpoiler;

        public void CycleSpoiler()
        {
            int currentIndex = (int)m_currentSpoiler;
            int nextIndex = (currentIndex + 1) % System.Enum.GetValues(typeof(SpoilerType)).Length;
            ApplySpoiler((SpoilerType)nextIndex);
        }
    }
}
