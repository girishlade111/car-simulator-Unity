using UnityEngine;
using CarSimulator.World;

namespace CarSimulator.Setup
{
    public class TestDistrictSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool m_setupOnStart = true;
        [SerializeField] private bool m_createDistrict = true;
        [SerializeField] private bool m_createVehicle = true;

        [Header("References")]
        [SerializeField] private TestDistrictBuilder m_districtBuilder;
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private void Start()
        {
            if (m_setupOnStart)
            {
                SetupTestDistrict();
            }
        }

        [ContextMenu("Setup Test District")]
        public void SetupTestDistrict()
        {
            if (m_createDistrict)
            {
                CreateDistrict();
            }

            if (m_createVehicle)
            {
                CreateVehicle();
            }

            SetupCamera();

            Debug.Log("[TestDistrictSetup] Test district setup complete!");
        }

        private void CreateDistrict()
        {
            if (m_districtBuilder == null)
            {
                GameObject builderObj = new GameObject("DistrictBuilder");
                m_districtBuilder = builderObj.AddComponent<TestDistrictBuilder>();
            }

            m_districtBuilder.BuildTestDistrict();
        }

        private void CreateVehicle()
        {
            if (m_vehicleSpawner == null)
            {
                GameObject spawnerObj = new GameObject("VehicleSpawner");
                m_vehicleSpawner = spawnerObj.AddComponent<VehicleSpawner>();
            }

            SpawnPoint defaultSpawn = FindObjectOfType<SpawnPoint>();
            if (defaultSpawn != null)
            {
                m_vehicleSpawner.SetSpawnPoint(defaultSpawn.transform);
            }

            m_vehicleSpawner.SpawnVehicle();
        }

        private void SetupCamera()
        {
            var followCam = FindObjectOfType<Camera.FollowCamera>();
            if (followCam == null)
            {
                followCam = CreateFollowCamera();
            }

            followCam.FindTarget();
        }

        private Camera.FollowCamera CreateFollowCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            mainCam.tag = "MainCamera";
            mainCam.gameObject.tag = "Untagged";

            GameObject camHolder = new GameObject("FollowCamera");
            Camera newCam = mainCam;
            newCam.transform.SetParent(camHolder.transform);
            newCam.transform.localPosition = Vector3.zero;
            newCam.transform.localRotation = Quaternion.identity;

            Camera.FollowCamera followCam = camHolder.AddComponent<Camera.FollowCamera>();
            followCam.SetTarget(m_vehicleSpawner != null ? m_vehicleSpawner.VehicleTransform : null);

            return followCam;
        }
    }
}
