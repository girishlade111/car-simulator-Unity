using UnityEngine;
using System.Collections.Generic;
using CarSimulator.AI;

namespace CarSimulator.World
{
    public class TrafficPrefabGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private bool m_generateOnStart = false;

        [Header("Vehicle Types")]
        [SerializeField] private bool m_generateSedan = true;
        [SerializeField] private bool m_generateSUV = true;
        [SerializeField] private bool m_generateTruck = true;
        [SerializeField] private bool m_generateSportsCar = true;
        [SerializeField] private bool m_generateTaxi = true;
        [SerializeField] private bool m_generateBus = true;

        [Header("Materials")]
        [SerializeField] private Material[] m_vehicleMaterials;

        private List<GameObject> m_generatedVehicles = new List<GameObject>();

        private void Start()
        {
            if (m_generateOnStart)
            {
                GenerateAll();
            }
        }

        public void GenerateAll()
        {
            m_generatedVehicles.Clear();

            if (m_generateSedan) m_generatedVehicles.Add(CreateSedan());
            if (m_generateSUV) m_generatedVehicles.Add(CreateSUV());
            if (m_generateTruck) m_generatedVehicles.Add(CreateTruck());
            if (m_generateSportsCar) m_generatedVehicles.Add(CreateSportsCar());
            if (m_generateTaxi) m_generatedVehicles.Add(CreateTaxi());
            if (m_generateBus) m_generatedVehicles.Add(CreateBus());

            Debug.Log($"[TrafficPrefabGenerator] Generated {m_generatedVehicles.Count} vehicle types");
        }

        private Material GetRandomMaterial(Color defaultColor)
        {
            if (m_vehicleMaterials != null && m_vehicleMaterials.Length > 0)
            {
                return m_vehicleMaterials[Random.Range(0, m_vehicleMaterials.Length)];
            }

            Material mat = new Material(Shader.Find("Standard"));
            mat.color = defaultColor;
            return mat;
        }

        private GameObject CreateSedan()
        {
            GameObject car = new GameObject("TrafficSedan");
            car.transform.position = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(car.transform);
            body.transform.localPosition = new Vector3(0, 0.4f, 0);
            body.transform.localScale = new Vector3(1.7f, 0.6f, 4.2f);
            body.GetComponent<Renderer>().material = GetRandomMaterial(Color.gray);

            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(car.transform);
            cabin.transform.localPosition = new Vector3(0, 1f, -0.2f);
            cabin.transform.localScale = new Vector3(1.5f, 0.55f, 2f);
            cabin.GetComponent<Renderer>().material = GetRandomMaterial(Color.black);

            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.name = "Hood";
            hood.transform.SetParent(car.transform);
            hood.transform.localPosition = new Vector3(0, 0.5f, 1.3f);
            hood.transform.localScale = new Vector3(1.6f, 0.3f, 1.2f);
            hood.GetComponent<Renderer>().material = GetRandomMaterial(Color.gray);

            AddWheels(car, 1.7f, 0.35f);
            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.Sedan);

            return car;
        }

        private GameObject CreateSUV()
        {
            GameObject car = new GameObject("TrafficSUV");
            car.transform.position = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(car.transform);
            body.transform.localPosition = new Vector3(0, 0.5f, 0);
            body.transform.localScale = new Vector3(1.9f, 0.7f, 4.5f);
            body.GetComponent<Renderer>().material = GetRandomMaterial(Color.black);

            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(car.transform);
            cabin.transform.localPosition = new Vector3(0, 1.15f, -0.3f);
            cabin.transform.localScale = new Vector3(1.7f, 0.65f, 2.2f);
            cabin.GetComponent<Renderer>().material = GetRandomMaterial(Color.black);

            AddWheels(car, 1.9f, 0.4f);
            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.SUV);

            return car;
        }

        private GameObject CreateTruck()
        {
            GameObject car = new GameObject("TrafficTruck");
            car.transform.position = Vector3.zero;

            GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cab.name = "Cab";
            cab.transform.SetParent(car.transform);
            cab.transform.localPosition = new Vector3(0, 0.8f, -1.5f);
            cab.transform.localScale = new Vector3(2f, 1f, 2f);
            cab.GetComponent<Renderer>().material = GetRandomMaterial(Color.white);

            GameObject bed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bed.name = "Bed";
            bed.transform.SetParent(car.transform);
            bed.transform.localPosition = new Vector3(0, 1f, 1.5f);
            bed.transform.localScale = new Vector3(2.2f, 0.8f, 4f);
            bed.GetComponent<Renderer>().material = GetRandomMaterial(Color.gray);

            AddWheels(car, 2f, 0.45f, 6);
            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.Truck);

            return car;
        }

        private GameObject CreateSportsCar()
        {
            GameObject car = new GameObject("TrafficSportsCar");
            car.transform.position = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(car.transform);
            body.transform.localPosition = new Vector3(0, 0.35f, 0);
            body.transform.localScale = new Vector3(1.8f, 0.45f, 4f);
            body.GetComponent<Renderer>().material = GetRandomMaterial(Color.red);

            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(car.transform);
            cabin.transform.localPosition = new Vector3(0, 0.75f, -0.3f);
            cabin.transform.localScale = new Vector3(1.4f, 0.4f, 1.5f);
            cabin.GetComponent<Renderer>().material = GetRandomMaterial(Color.black);

            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.name = "Hood";
            hood.transform.SetParent(car.transform);
            hood.transform.localPosition = new Vector3(0, 0.4f, 1.2f);
            hood.transform.localScale = new Vector3(1.6f, 0.2f, 1f);
            hood.GetComponent<Renderer>().material = GetRandomMaterial(Color.red);

            AddWheels(car, 1.6f, 0.35f);
            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.SportsCar);

            return car;
        }

        private GameObject CreateTaxi()
        {
            GameObject car = CreateSedan();
            car.name = "TrafficTaxi";

            Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.name == "Body" || renderer.gameObject.name == "Hood")
                {
                    renderer.material.color = new Color(1f, 0.9f, 0.2f);
                }
            }

            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.Taxi);

            return car;
        }

        private GameObject CreateBus()
        {
            GameObject car = new GameObject("TrafficBus");
            car.transform.position = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(car.transform);
            body.transform.localPosition = new Vector3(0, 1.2f, 0);
            body.transform.localScale = new Vector3(2.4f, 2f, 10f);
            body.GetComponent<Renderer>().material = GetRandomMaterial(Color.yellow);

            GameObject windows = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windows.name = "Windows";
            windows.transform.SetParent(car.transform);
            windows.transform.localPosition = new Vector3(0, 1.5f, 0);
            windows.transform.localScale = new Vector3(2.2f, 1f, 9.5f);
            windows.GetComponent<Renderer>().material = GetRandomMaterial(Color.cyan);

            AddWheels(car, 2.2f, 0.5f, 6);
            SetupTrafficAI(car, EnhancedTrafficCar.VehicleType.Bus);

            return car;
        }

        private void AddWheels(GameObject car, float width, float wheelRadius, int wheelCount = 4)
        {
            float[] positions = wheelCount == 6 
                ? new float[] { -1.2f, 1.2f, -3f, 3f }
                : new float[] { -1.2f, 1.2f };

            int rows = wheelCount == 6 ? 2 : 1;
            
            for (int side = 0; side < 2; side++)
            {
                float xPos = side == 0 ? -width / 2 : width / 2;
                float xFlip = side == 0 ? 1 : -1;

                for (int i = 0; i < positions.Length; i++)
                {
                    GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    wheel.name = $"Wheel_{side}_{i}";
                    wheel.transform.SetParent(car.transform);
                    wheel.transform.localPosition = new Vector3(xPos, wheelRadius, positions[i]);
                    wheel.transform.localScale = new Vector3(wheelRadius * 2, wheelRadius * 0.3f, wheelRadius * 2);
                    wheel.transform.rotation = Quaternion.Euler(0, 0, 90);

                    wheel.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                    wheel.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }

        private void SetupTrafficAI(GameObject car, EnhancedTrafficCar.VehicleType type)
        {
            EnhancedTrafficCar ai = car.AddComponent<EnhancedTrafficCar>();
            ai.SetVehicleType(type);

            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = car.AddComponent<Rigidbody>();
            }
            rb.mass = 1500f;
            rb.useGravity = true;
            rb.isKinematic = false;

            BoxCollider collider = car.AddComponent<BoxCollider>();
            collider.size = new Vector3(1.8f, 1f, 4f);
            collider.center = new Vector3(0, 0.5f, 0);
        }

        public List<GameObject> GetGeneratedVehicles() => m_generatedVehicles;
    }
}
