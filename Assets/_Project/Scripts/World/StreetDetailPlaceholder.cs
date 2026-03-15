using UnityEngine;

namespace CarSimulator.World
{
    public class StreetDetailPlaceholder : MonoBehaviour
    {
        [Header("Detail Type")]
        [SerializeField] private DetailType m_detailType = DetailType.Bench;
        [SerializeField] private string m_displayName = "Street Detail";

        [Header("Variation")]
        [SerializeField] private float m_randomScale = 0.1f;
        [SerializeField] private float m_randomRotation = 15f;
        [SerializeField] private Color m_customColor = Color.white;

        [Header("Physics")]
        [SerializeField] private bool m_hasCollider = true;
        [SerializeField] private bool m_isStatic = true;

        public enum DetailType
        {
            Bench,
            StreetLamp,
            TrafficPole,
            FireHydrant,
            TrashBin,
            Mailbox,
            NewsStand,
            BusStop,
            Barrier,
            Bollard,
            StopSign,
            StreetSign,
            Crosswalk,
            Manhole,
            Drain,
            Vent,
            Meter,
            Plant,
            Fountain,
            Statue
        }

        private void Awake()
        {
            ApplyVariation();
        }

        private void ApplyVariation()
        {
            if (m_randomScale > 0)
            {
                float scale = 1f + Random.Range(-m_randomScale, m_randomScale);
                transform.localScale *= scale;
            }

            if (m_randomRotation > 0)
            {
                float rotY = Random.Range(-m_randomRotation, m_randomRotation);
                transform.Rotate(Vector3.up, rotY);
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (m_customColor != Color.white)
                {
                    renderer.material.color = m_customColor;
                }
            }
        }

        public static GameObject CreatePlaceholder(DetailType type, string name)
        {
            GameObject obj = new GameObject(name);
            obj.tag = "Prop";
            obj.layer = LayerMask.NameToLayer("Prop");

            switch (type)
            {
                case DetailType.Bench:
                    CreateBench(obj);
                    break;
                case DetailType.StreetLamp:
                    CreateStreetLamp(obj);
                    break;
                case DetailType.TrafficPole:
                    CreateTrafficPole(obj);
                    break;
                case DetailType.FireHydrant:
                    CreateFireHydrant(obj);
                    break;
                case DetailType.TrashBin:
                    CreateTrashBin(obj);
                    break;
                case DetailType.Mailbox:
                    CreateMailbox(obj);
                    break;
                case DetailType.BusStop:
                    CreateBusStop(obj);
                    break;
                case DetailType.Barrier:
                    CreateBarrier(obj);
                    break;
                case DetailType.Bollard:
                    CreateBollard(obj);
                    break;
                case DetailType.StopSign:
                    CreateStopSign(obj);
                    break;
                case DetailType.StreetSign:
                    CreateStreetSign(obj);
                    break;
                case DetailType.Manhole:
                    CreateManhole(obj);
                    break;
                case DetailType.Drain:
                    CreateDrain(obj);
                    break;
                case DetailType.Vent:
                    CreateVent(obj);
                    break;
                case DetailType.Plant:
                    CreatePlant(obj);
                    break;
                case DetailType.Fountain:
                    CreateFountain(obj);
                    break;
                default:
                    CreateDefaultProp(obj);
                    break;
            }

            obj.AddComponent<StreetDetailPlaceholder>().m_detailType = type;
            return obj;
        }

        private static void CreateBench(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.4f, 0.25f, 0.1f);

            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Seat";
            seat.transform.SetParent(parent.transform);
            seat.transform.localPosition = new Vector3(0, 0.5f, 0);
            seat.transform.localScale = new Vector3(1.5f, 0.1f, 0.5f);
            seat.GetComponent<Renderer>().material = mat;

            GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
            back.name = "Back";
            back.transform.SetParent(parent.transform);
            back.transform.localPosition = new Vector3(0, 1f, -0.2f);
            back.transform.localScale = new Vector3(1.5f, 0.5f, 0.1f);
            back.GetComponent<Renderer>().material = mat;

            for (int i = -1; i <= 1; i += 2)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = $"Leg_{i}";
                leg.transform.SetParent(parent.transform);
                leg.transform.localPosition = new Vector3(i * 0.6f, 0.25f, 0);
                leg.transform.localScale = new Vector3(0.1f, 0.5f, 0.4f);
                leg.GetComponent<Renderer>().material = mat;
            }
        }

        private static void CreateStreetLamp(GameObject parent)
        {
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = Color.gray;

            Material lightMat = new Material(Shader.Find("Standard"));
            lightMat.color = Color.yellow;
            lightMat.EnableKeyword("_EMISSION");

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(0, 2f, 0);
            pole.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            pole.GetComponent<Renderer>().material = poleMat;

            GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arm.name = "Arm";
            arm.transform.SetParent(parent.transform);
            arm.transform.localPosition = new Vector3(0.5f, 3.8f, 0);
            arm.transform.localScale = new Vector3(0.05f, 0.5f, 0.05f);
            arm.transform.rotation = Quaternion.Euler(0, 0, 90);
            arm.GetComponent<Renderer>().material = poleMat;

            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lamp.name = "Lamp";
            lamp.transform.SetParent(parent.transform);
            lamp.transform.localPosition = new Vector3(1f, 3.8f, 0);
            lamp.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            lamp.GetComponent<Renderer>().material = lightMat;

            Light light = lamp.AddComponent<Light>();
            light.color = Color.yellow;
            light.intensity = 0.5f;
            light.range = 10f;
        }

        private static void CreateTrafficPole(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.gray;

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(0, 1.5f, 0);
            pole.transform.localScale = new Vector3(0.08f, 1.5f, 0.08f);
            pole.GetComponent<Renderer>().material = mat;

            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Sign";
            sign.transform.SetParent(parent.transform);
            sign.transform.localPosition = new Vector3(0, 2.5f, 0);
            sign.transform.localScale = new Vector3(0.4f, 0.4f, 0.05f);
            sign.GetComponent<Renderer>().material.color = Color.red;
        }

        private static void CreateFireHydrant(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.red;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Body";
            body.transform.SetParent(parent.transform);
            body.transform.localPosition = new Vector3(0, 0.4f, 0);
            body.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
            body.GetComponent<Renderer>().material = mat;

            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            top.name = "Top";
            top.transform.SetParent(parent.transform);
            top.transform.localPosition = new Vector3(0, 0.7f, 0);
            top.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
            top.GetComponent<Renderer>().material = mat;
        }

        private static void CreateTrashBin(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.3f, 0.3f, 0.3f);

            GameObject bin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bin.name = "Bin";
            bin.transform.SetParent(parent.transform);
            bin.transform.localPosition = new Vector3(0, 0.5f, 0);
            bin.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
            bin.GetComponent<Renderer>().material = mat;
        }

        private static void CreateMailbox(GameObject parent)
        {
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = Color.gray;

            Material boxMat = new Material(Shader.Find("Standard"));
            boxMat.color = Color.blue;

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(0, 0.6f, 0);
            pole.transform.localScale = new Vector3(0.08f, 0.6f, 0.08f);
            pole.GetComponent<Renderer>().material = poleMat;

            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "Box";
            box.transform.SetParent(parent.transform);
            box.transform.localPosition = new Vector3(0, 1.2f, 0);
            box.transform.localScale = new Vector3(0.4f, 0.3f, 0.25f);
            box.GetComponent<Renderer>().material = boxMat;
        }

        private static void CreateBusStop(GameObject parent)
        {
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = Color.gray;

            Material signMat = new Material(Shader.Find("Standard"));
            signMat.color = new Color(0.2f, 0.6f, 1f);

            for (int i = 0; i < 2; i++)
            {
                GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pole.name = $"Pole_{i}";
                pole.transform.SetParent(parent.transform);
                pole.transform.localPosition = new Vector3((i == 0 ? -0.8f : 0.8f), 1.5f, 0);
                pole.transform.localScale = new Vector3(0.1f, 1.5f, 0.1f);
                pole.GetComponent<Renderer>().material = poleMat;
            }

            GameObject shelter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shelter.name = "Shelter";
            shelter.transform.SetParent(parent.transform);
            shelter.transform.localPosition = new Vector3(0, 2.5f, 0);
            shelter.transform.localScale = new Vector3(2f, 0.1f, 1f);
            shelter.GetComponent<Renderer>().material = signMat;
        }

        private static void CreateBarrier(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(1f, 0.5f, 0f);

            GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = "Barrier";
            barrier.transform.SetParent(parent.transform);
            barrier.transform.localPosition = new Vector3(0, 0.5f, 0);
            barrier.transform.localScale = new Vector3(2f, 1f, 0.2f);
            barrier.GetComponent<Renderer>().material = mat;
        }

        private static void CreateBollard(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.gray;

            GameObject bollard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bollard.name = "Bollard";
            bollard.transform.SetParent(parent.transform);
            bollard.transform.localPosition = new Vector3(0, 0.5f, 0);
            bollard.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
            bollard.GetComponent<Renderer>().material = mat;
        }

        private static void CreateStopSign(GameObject parent)
        {
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = Color.gray;

            Material signMat = new Material(Shader.Find("Standard"));
            signMat.color = Color.red;

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(0, 1f, 0);
            pole.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
            pole.GetComponent<Renderer>().material = poleMat;

            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Sign";
            sign.transform.SetParent(parent.transform);
            sign.transform.localPosition = new Vector3(0, 1.8f, 0);
            sign.transform.localScale = new Vector3(0.4f, 0.4f, 0.05f);
            sign.GetComponent<Renderer>().material = signMat;
        }

        private static void CreateStreetSign(GameObject parent)
        {
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = Color.gray;

            Material signMat = new Material(Shader.Find("Standard"));
            signMat.color = Color.white;

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(parent.transform);
            pole.transform.localPosition = new Vector3(0, 1.5f, 0);
            pole.transform.localScale = new Vector3(0.05f, 1.5f, 0.05f);
            pole.GetComponent<Renderer>().material = poleMat;

            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Sign";
            sign.transform.SetParent(parent.transform);
            sign.transform.localPosition = new Vector3(0, 2.5f, 0);
            sign.transform.localScale = new Vector3(0.3f, 0.3f, 0.02f);
            sign.GetComponent<Renderer>().material = signMat;
        }

        private static void CreateManhole(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.2f, 0.2f);

            GameObject manhole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            manhole.name = "Manhole";
            manhole.transform.SetParent(parent.transform);
            manhole.transform.localPosition = new Vector3(0, 0.01f, 0);
            manhole.transform.localScale = new Vector3(0.6f, 0.02f, 0.6f);
            manhole.GetComponent<Renderer>().material = mat;
        }

        private static void CreateDrain(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.15f, 0.15f, 0.15f);

            GameObject drain = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            drain.name = "Drain";
            drain.transform.SetParent(parent.transform);
            drain.transform.localPosition = new Vector3(0, 0.01f, 0);
            drain.transform.localScale = new Vector3(0.3f, 0.02f, 0.3f);
            drain.GetComponent<Renderer>().material = mat;
        }

        private static void CreateVent(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.gray;

            GameObject vent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vent.name = "Vent";
            vent.transform.SetParent(parent.transform);
            vent.transform.localPosition = new Vector3(0, 0.3f, 0);
            vent.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
            vent.GetComponent<Renderer>().material = mat;
        }

        private static void CreatePlant(GameObject parent)
        {
            Material potMat = new Material(Shader.Find("Standard"));
            potMat.color = new Color(0.5f, 0.3f, 0.2f);

            Material plantMat = new Material(Shader.Find("Standard"));
            plantMat.color = new Color(0.2f, 0.5f, 0.2f);

            GameObject pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pot.name = "Pot";
            pot.transform.SetParent(parent.transform);
            pot.transform.localPosition = new Vector3(0, 0.2f, 0);
            pot.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);
            pot.GetComponent<Renderer>().material = potMat;

            GameObject plant = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            plant.name = "Plant";
            plant.transform.SetParent(parent.transform);
            plant.transform.localPosition = new Vector3(0, 0.6f, 0);
            plant.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
            plant.GetComponent<Renderer>().material = plantMat;
        }

        private static void CreateFountain(GameObject parent)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.7f, 0.7f, 0.75f);

            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.name = "Base";
            baseObj.transform.SetParent(parent.transform);
            baseObj.transform.localPosition = new Vector3(0, 0.3f, 0);
            baseObj.transform.localScale = new Vector3(1.5f, 0.3f, 1.5f);
            baseObj.GetComponent<Renderer>().material = mat;

            GameObject center = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            center.name = "Center";
            center.transform.SetParent(parent.transform);
            center.transform.localPosition = new Vector3(0, 0.8f, 0);
            center.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            center.GetComponent<Renderer>().material = mat;
        }

        private static void CreateDefaultProp(GameObject parent)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(parent.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one;
        }
    }
}
