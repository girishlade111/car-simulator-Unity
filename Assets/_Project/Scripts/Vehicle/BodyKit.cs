using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class BodyKit : MonoBehaviour
    {
        [Header("Body Kit Settings")]
        [SerializeField] private bool m_enableBodyKit = true;
        [SerializeField] private BodyKitType m_currentKit = BodyKitType.Stock;

        [Header("Kit Parts")]
        [SerializeField] private GameObject m_frontBumper;
        [SerializeField] private GameObject m_rearBumper;
        [SerializeField] private GameObject m_sideSkirts;
        [SerializeField] private GameObject m_rearSpoiler;
        [SerializeField] private GameObject m_roofScoop;
        [SerializeField] private GameObject m_hood;

        [Header("References")]
        [SerializeField] private Transform m_vehicleRoot;

        public enum BodyKitType
        {
            Stock,
            Sport,
            Racing,
            Drift,
            Offroad,
            Luxury
        }

        private Dictionary<BodyKitType, BodyKitData> m_bodyKitData = new Dictionary<BodyKitType, BodyKitData>();

        [System.Serializable]
        public class BodyKitData
        {
            public string name;
            public int price;
            public float downforceBonus;
            public float handlingBonus;
            public float weightPenalty;
        }

        private void Start()
        {
            InitializeBodyKits();
            if (m_enableBodyKit)
            {
                ApplyBodyKit(m_currentKit);
            }
        }

        private void InitializeBodyKits()
        {
            m_bodyKitData = new Dictionary<BodyKitType, BodyKitData>
            {
                { BodyKitType.Stock, new BodyKitData { name = "Stock", price = 0, downforceBonus = 0, handlingBonus = 0, weightPenalty = 0 } },
                { BodyKitType.Sport, new BodyKitData { name = "Sport", price = 1500, downforceBonus = 0.1f, handlingBonus = 0.15f, weightPenalty = -10f } },
                { BodyKitType.Racing, new BodyKitData { name = "Racing", price = 3500, downforceBonus = 0.25f, handlingBonus = 0.2f, weightPenalty = -25f } },
                { BodyKitType.Drift, new BodyKitData { name = "Drift", price = 2800, downforceBonus = 0.15f, handlingBonus = -0.1f, weightPenalty = -20f } },
                { BodyKitType.Offroad, new BodyKitData { name = "Offroad", price = 2000, downforceBonus = 0.05f, handlingBonus = 0.1f, weightPenalty = 10f } },
                { BodyKitType.Luxury, new BodyKitData { name = "Luxury", price = 4000, downforceBonus = 0.05f, handlingBonus = 0.05f, weightPenalty = 5f } }
            };
        }

        public void ApplyBodyKit(BodyKitType kitType)
        {
            m_currentKit = kitType;
            
            ClearAllKits();
            
            switch (kitType)
            {
                case BodyKitType.Sport:
                    ApplySportKit();
                    break;
                case BodyKitType.Racing:
                    ApplyRacingKit();
                    break;
                case BodyKitType.Drift:
                    ApplyDriftKit();
                    break;
                case BodyKitType.Offroad:
                    ApplyOffroadKit();
                    break;
                case BodyKitType.Luxury:
                    ApplyLuxuryKit();
                    break;
            }

            ApplyStats(kitType);
        }

        private void ClearAllKits()
        {
            if (m_frontBumper != null) m_frontBumper.SetActive(false);
            if (m_rearBumper != null) m_rearBumper.SetActive(false);
            if (m_sideSkirts != null) m_sideSkirts.SetActive(false);
            if (m_rearSpoiler != null) m_rearSpoiler.SetActive(false);
            if (m_roofScoop != null) m_roofScoop.SetActive(false);
            if (m_hood != null) m_hood.SetActive(false);
        }

        private void ApplySportKit()
        {
            if (m_rearSpoiler != null)
            {
                m_rearSpoiler = CreateKitPart("RearSpoiler", new Vector3(0, 1f, -1.8f), new Vector3(1.5f, 0.3f, 0.5f));
                m_rearSpoiler.SetActive(true);
            }

            if (m_frontBumper != null)
            {
                m_frontBumper = CreateKitPart("FrontBumper", new Vector3(0, 0.4f, 2f), new Vector3(2f, 0.5f, 0.3f));
                m_frontBumper.SetActive(true);
            }
        }

        private void ApplyRacingKit()
        {
            ApplySportKit();

            if (m_roofScoop != null)
            {
                m_roofScoop = CreateKitPart("RoofScoop", new Vector3(0, 1.8f, -0.3f), new Vector3(0.5f, 0.3f, 0.8f));
                m_roofScoop.SetActive(true);
            }

            if (m_rearBumper != null)
            {
                m_rearBumper = CreateKitPart("RearBumper", new Vector3(0, 0.4f, -2f), new Vector3(2f, 0.5f, 0.3f));
                m_rearBumper.SetActive(true);
            }

            if (m_sideSkirts != null)
            {
                m_sideSkirts = CreateKitPart("SideSkirts", new Vector3(0, 0.3f, 0), new Vector3(0.2f, 0.2f, 4f));
                m_sideSkirts.SetActive(true);
            }
        }

        private void ApplyDriftKit()
        {
            ApplyRacingKit();

            if (m_rearSpoiler != null && m_rearSpoiler.activeSelf)
            {
                m_rearSpoiler.transform.localScale = new Vector3(2f, 0.5f, 0.8f);
            }
        }

        private void ApplyOffroadKit()
        {
            if (m_frontBumper != null)
            {
                m_frontBumper = CreateKitPart("FrontBumper_Offroad", new Vector3(0, 0.3f, 2f), new Vector3(2.2f, 0.6f, 0.4f));
                m_frontBumper.SetActive(true);
            }

            if (m_rearBumper != null)
            {
                m_rearBumper = CreateKitPart("RearBumper_Offroad", new Vector3(0, 0.3f, -2f), new Vector3(2.2f, 0.6f, 0.4f));
                m_rearBumper.SetActive(true);
            }
        }

        private void ApplyLuxuryKit()
        {
            if (m_hood != null)
            {
                m_hood = CreateKitPart("LuxuryHood", new Vector3(0, 1f, 1.2f), new Vector3(1.8f, 0.15f, 1.5f));
                m_hood.SetActive(true);
            }
        }

        private GameObject CreateKitPart(string name, Vector3 localPos, Vector3 scale)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(m_vehicleRoot != null ? m_vehicleRoot : transform);
            part.transform.localPosition = localPos;
            part.transform.localScale = scale;
            return part;
        }

        private void ApplyStats(BodyKitType kitType)
        {
            if (!m_bodyKitData.ContainsKey(kitType)) return;

            BodyKitData data = m_bodyKitData[kitType];
            
            var physics = GetComponent<VehiclePhysics>();
            if (physics != null)
            {
                var tuning = physics.m_tuning;
                if (tuning != null)
                {
                    tuning.downforce += data.downforceBonus * 10f;
                }
            }
        }

        public BodyKitData GetCurrentKitData()
        {
            return m_bodyKitData.ContainsKey(m_currentKit) ? m_bodyKitData[m_currentKit] : null;
        }

        public BodyKitType GetCurrentKit() => m_currentKit;
    }
}
