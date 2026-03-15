using UnityEngine;

namespace CarSimulator.World
{
    public class ParkedCarVariants : MonoBehaviour
    {
        public enum CarVariant
        {
            Sedan,
            SUV,
            Truck,
            Sports,
            Compact,
            Van,
            Motorcycle
        }

        [Header("Variant Settings")]
        [SerializeField] private CarVariant m_variant = CarVariant.Sedan;

        [Header("Color Settings")]
        [SerializeField] private Color m_bodyColor = Color.white;
        [SerializeField] private Color m_roofColor = Color.gray;

        private void Awake()
        {
            ApplyVariantSettings();
        }

        public void SetVariant(CarVariant variant)
        {
            m_variant = variant;
            ApplyVariantSettings();
        }

        private void ApplyVariantSettings()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                string name = renderer.gameObject.name.ToLower();

                if (name.Contains("body") || name.Contains("hood") || name.Contains("trunk") || name.Contains("door"))
                {
                    renderer.material.color = m_bodyColor;
                }
                else if (name.Contains("roof"))
                {
                    renderer.material.color = m_roofColor;
                }
                else if (name.Contains("wheel"))
                {
                    renderer.material.color = Color.black;
                }
                else if (name.Contains("window"))
                {
                    renderer.material.color = new Color(0.2f, 0.3f, 0.4f, 0.5f);
                }
            }
        }

        public void SetBodyColor(Color color)
        {
            m_bodyColor = color;
            ApplyVariantSettings();
        }

        public static GameObject CreateVariantPrefab(CarVariant variant, string name)
        {
            GameObject car = new GameObject(name);
            car.tag = "Prop";
            car.layer = LayerMask.NameToLayer("Prop");

            Material bodyMat = new Material(Shader.Find("Standard"));
            Material roofMat = new Material(Shader.Find("Standard"));
            Material wheelMat = new Material(Shader.Find("Standard"));
            Material windowMat = new Material(Shader.Find("Standard"));

            switch (variant)
            {
                case CarVariant.Sedan:
                    CreateSedanParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.SUV:
                    CreateSUVParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.Truck:
                    CreateTruckParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.Sports:
                    CreateSportsParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.Compact:
                    CreateCompactParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.Van:
                    CreateVanParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
                case CarVariant.Motorcycle:
                    CreateMotorcycleParts(car, bodyMat, roofMat, wheelMat, windowMat);
                    break;
            }

            car.AddComponent<ParkedCar>();

            return car;
        }

        private static void CreateSedanParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.5f, 0);
            bodyObj.transform.localScale = new Vector3(2f, 0.8f, 4f);
            bodyObj.GetComponent<Renderer>().material = body;

            GameObject roofObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roofObj.name = "Roof";
            roofObj.transform.SetParent(car.transform);
            roofObj.transform.localPosition = new Vector3(0, 1.2f, -0.3f);
            roofObj.transform.localScale = new Vector3(1.8f, 0.6f, 2f);
            roofObj.GetComponent<Renderer>().material = roof;

            CreateWheels(car, wheel, new Vector3(-1f, 0.35f, 1.2f), new Vector3(1f, 0.35f, 1.2f),
                         new Vector3(-1f, 0.35f, -1.2f), new Vector3(1f, 0.35f, -1.2f));
        }

        private static void CreateSUVParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.7f, 0);
            bodyObj.transform.localScale = new Vector3(2.2f, 1f, 4.5f);
            bodyObj.GetComponent<Renderer>().material = body;

            GameObject roofObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roofObj.name = "Roof";
            roofObj.transform.SetParent(car.transform);
            roofObj.transform.localPosition = new Vector3(0, 1.5f, -0.2f);
            roofObj.transform.localScale = new Vector3(2f, 0.7f, 2.2f);
            roofObj.GetComponent<Renderer>().material = roof;

            CreateWheels(car, wheel, new Vector3(-1.1f, 0.4f, 1.3f), new Vector3(1.1f, 0.4f, 1.3f),
                         new Vector3(-1.1f, 0.4f, -1.3f), new Vector3(1.1f, 0.4f, -1.3f));
        }

        private static void CreateTruckParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject cabObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabObj.name = "Cab";
            cabObj.transform.SetParent(car.transform);
            cabObj.transform.localPosition = new Vector3(0, 1f, 1f);
            cabObj.transform.localScale = new Vector3(2.2f, 1.2f, 2f);
            cabObj.GetComponent<Renderer>().material = body;

            GameObject bedObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bedObj.name = "Bed";
            bedObj.transform.SetParent(car.transform);
            bedObj.transform.localPosition = new Vector3(0, 0.8f, -1.5f);
            bedObj.transform.localScale = new Vector3(2.2f, 0.6f, 2.5f);
            bedObj.GetComponent<Renderer>().material = body;

            CreateWheels(car, wheel, new Vector3(-1.2f, 0.45f, 1.5f), new Vector3(1.2f, 0.45f, 1.5f),
                         new Vector3(-1.2f, 0.45f, -1.5f), new Vector3(1.2f, 0.45f, -1.5f));
        }

        private static void CreateSportsParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.4f, 0);
            bodyObj.transform.localScale = new Vector3(1.9f, 0.5f, 4.2f);
            bodyObj.GetComponent<Renderer>().material = body;

            GameObject roofObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roofObj.name = "Roof";
            roofObj.transform.SetParent(car.transform);
            roofObj.transform.localPosition = new Vector3(0, 0.85f, -0.2f);
            roofObj.transform.localScale = new Vector3(1.6f, 0.4f, 1.8f);
            roofObj.GetComponent<Renderer>().material = roof;

            CreateWheels(car, wheel, new Vector3(-1f, 0.3f, 1.3f), new Vector3(1f, 0.3f, 1.3f),
                         new Vector3(-1f, 0.3f, -1.3f), new Vector3(1f, 0.3f, -1.3f));
        }

        private static void CreateCompactParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.45f, 0);
            bodyObj.transform.localScale = new Vector3(1.6f, 0.6f, 3f);
            bodyObj.GetComponent<Renderer>().material = body;

            GameObject roofObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roofObj.name = "Roof";
            roofObj.transform.SetParent(car.transform);
            roofObj.transform.localPosition = new Vector3(0, 1f, -0.2f);
            roofObj.transform.localScale = new Vector3(1.4f, 0.5f, 1.5f);
            roofObj.GetComponent<Renderer>().material = roof;

            CreateWheels(car, wheel, new Vector3(-0.8f, 0.3f, 0.9f), new Vector3(0.8f, 0.3f, 0.9f),
                         new Vector3(-0.8f, 0.3f, -0.9f), new Vector3(0.8f, 0.3f, -0.9f));
        }

        private static void CreateVanParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.9f, 0);
            bodyObj.transform.localScale = new Vector3(2f, 1.4f, 4f);
            bodyObj.GetComponent<Renderer>().material = body;

            CreateWheels(car, wheel, new Vector3(-1f, 0.4f, 1.2f), new Vector3(1f, 0.4f, 1.2f),
                         new Vector3(-1f, 0.4f, -1.2f), new Vector3(1f, 0.4f, -1.2f));
        }

        private static void CreateMotorcycleParts(GameObject car, Material body, Material roof, Material wheel, Material window)
        {
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "Body";
            bodyObj.transform.SetParent(car.transform);
            bodyObj.transform.localPosition = new Vector3(0, 0.6f, 0);
            bodyObj.transform.localScale = new Vector3(0.5f, 0.6f, 1.8f);
            bodyObj.GetComponent<Renderer>().material = body;

            GameObject wheelFront = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelFront.name = "Wheel_Front";
            wheelFront.transform.SetParent(car.transform);
            wheelFront.transform.localPosition = new Vector3(0, 0.3f, 0.8f);
            wheelFront.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            wheelFront.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelFront.GetComponent<Renderer>().material = wheel;

            GameObject wheelBack = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelBack.name = "Wheel_Back";
            wheelBack.transform.SetParent(car.transform);
            wheelBack.transform.localPosition = new Vector3(0, 0.3f, -0.8f);
            wheelBack.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            wheelBack.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelBack.GetComponent<Renderer>().material = wheel;
        }

        private static void CreateWheels(GameObject car, Material wheelMat, Vector3 fl, Vector3 fr, Vector3 rl, Vector3 rr)
        {
            GameObject wheelFL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelFL.name = "Wheel_FL";
            wheelFL.transform.SetParent(car.transform);
            wheelFL.transform.localPosition = fl;
            wheelFL.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            wheelFL.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelFL.GetComponent<Renderer>().material = wheelMat;

            GameObject wheelFR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelFR.name = "Wheel_FR";
            wheelFR.transform.SetParent(car.transform);
            wheelFR.transform.localPosition = fr;
            wheelFR.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            wheelFR.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelFR.GetComponent<Renderer>().material = wheelMat;

            GameObject wheelRL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelRL.name = "Wheel_RL";
            wheelRL.transform.SetParent(car.transform);
            wheelRL.transform.localPosition = rl;
            wheelRL.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            wheelRL.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelRL.GetComponent<Renderer>().material = wheelMat;

            GameObject wheelRR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheelRR.name = "Wheel_RR";
            wheelRR.transform.SetParent(car.transform);
            wheelRR.transform.localPosition = rr;
            wheelRR.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            wheelRR.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheelRR.GetComponent<Renderer>().material = wheelMat;
        }
    }
}
