using UnityEngine;

namespace CarSimulator.World
{
    public class PlaceholderFactory : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material m_groundMaterial;
        [SerializeField] private Material m_roadMaterial;
        [SerializeField] private Material m_buildingMaterial;
        [SerializeField] private Material m_roofMaterial;

        public GameObject CreateGround(Vector3 position, Vector2 size)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = position;
            ground.transform.localScale = new Vector3(size.x / 10f, 1f, size.y / 10f);
            ground.layer = LayerMask.NameToLayer("Ground");
            
            if (m_groundMaterial != null)
                ground.GetComponent<Renderer>().material = m_groundMaterial;
            
            return ground;
        }

        public GameObject CreateRoad(Vector3 position, Vector2 size, float rotationY = 0f)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Road";
            road.transform.position = position + Vector3.up * 0.05f;
            road.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            road.transform.localScale = new Vector3(size.x, 0.1f, size.y);
            
            if (m_roadMaterial != null)
                road.GetComponent<Renderer>().material = m_roadMaterial;
            
            return road;
        }

        public GameObject CreateBuilding(Vector3 position, float width, float height, float depth)
        {
            GameObject building = new GameObject("Building");
            building.transform.position = position;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(building.transform);
            body.transform.localPosition = new Vector3(0, height / 2f, 0);
            body.transform.localScale = new Vector3(width, height, depth);
            body.name = "Body";

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.transform.SetParent(building.transform);
            roof.transform.localPosition = new Vector3(0, height + 0.5f, 0);
            roof.transform.localScale = new Vector3(width + 1, 1, depth + 1);
            roof.name = "Roof";

            if (m_buildingMaterial != null)
            {
                body.GetComponent<Renderer>().material = m_buildingMaterial;
                roof.GetComponent<Renderer>().material = m_roofMaterial ?? m_buildingMaterial;
            }

            return building;
        }

        public GameObject CreateTree(Vector3 position)
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.position = position;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
            trunk.name = "Trunk";

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.transform.SetParent(tree.transform);
            foliage.transform.localPosition = new Vector3(0, 4f, 0);
            foliage.transform.localScale = new Vector3(3f, 3f, 3f);
            foliage.name = "Foliage";

            return tree;
        }

        public GameObject CreateRock(Vector3 position)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Rock";
            rock.transform.position = position;
            rock.transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(0.5f, 1.5f), Random.Range(1f, 3f));
            rock.transform.rotation = Quaternion.Euler(Random.Range(0, 30), Random.Range(0, 360), Random.Range(0, 30));
            return rock;
        }

        public GameObject CreateParkedCar(Vector3 position, float rotationY = 0f)
        {
            GameObject car = new GameObject("ParkedCar");
            car.transform.position = position;
            car.transform.rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(car.transform);
            body.transform.localPosition = new Vector3(0, 0.5f, 0);
            body.transform.localScale = new Vector3(2f, 0.8f, 4f);
            body.name = "Body";

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.transform.SetParent(car.transform);
            roof.transform.localPosition = new Vector3(0, 1.2f, -0.3f);
            roof.transform.localScale = new Vector3(1.8f, 0.7f, 2f);
            roof.name = "Roof";

            return car;
        }

        public GameObject CreateStreetLamp(Vector3 position)
        {
            GameObject lamp = new GameObject("StreetLamp");
            lamp.transform.position = position;

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.SetParent(lamp.transform);
            pole.transform.localPosition = new Vector3(0, 2.5f, 0);
            pole.transform.localScale = new Vector3(0.1f, 2.5f, 0.1f);
            pole.name = "Pole";

            return lamp;
        }
    }
}
