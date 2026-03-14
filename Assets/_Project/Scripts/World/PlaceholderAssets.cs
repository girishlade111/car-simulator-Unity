using UnityEngine;

public class PlaceholderAssets : MonoBehaviour
{
    [Header("Material References")]
    [SerializeField] private Material m_roadMaterial;
    [SerializeField] private Material m_groundMaterial;
    [SerializeField] private Material m_buildingMaterial;
    [SerializeField] private Material m_treeTrunkMaterial;
    [SerializeField] private Material m_treeLeafMaterial;

    public static PlaceholderAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject CreateRoadSegment(Vector3 position, Vector3 size, float rotationY = 0f)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "RoadSegment";
        road.transform.position = position;
        road.transform.localScale = size;
        road.transform.rotation = Quaternion.Euler(0, rotationY, 0);

        if (m_roadMaterial != null)
        {
            road.GetComponent<Renderer>().material = m_roadMaterial;
        }

        return road;
    }

    public GameObject CreateGroundPlane(Vector3 position, Vector2 size)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "GroundPlane";
        ground.transform.position = position;
        ground.transform.localScale = new Vector3(size.x / 10f, 1f, size.y / 10f);

        if (m_groundMaterial != null)
        {
            ground.GetComponent<Renderer>().material = m_groundMaterial;
        }

        return ground;
    }

    public GameObject CreateBuilding(Vector3 position, Vector3 size, string name = "Building")
    {
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = name;
        building.transform.position = position + Vector3.up * (size.y / 2f);
        building.transform.localScale = size;

        if (m_buildingMaterial != null)
        {
            building.GetComponent<Renderer>().material = m_buildingMaterial;
        }

        return building;
    }

    public GameObject CreateApartmentBuilding(Vector3 position)
    {
        GameObject apartment = new GameObject("Apartment_Placeholder");
        apartment.transform.position = position;

        GameObject mainBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainBody.transform.SetParent(apartment.transform);
        mainBody.transform.localPosition = new Vector3(0, 10, 0);
        mainBody.transform.localScale = new Vector3(15, 20, 15);
        mainBody.name = "Body";

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.SetParent(apartment.transform);
        roof.transform.localPosition = new Vector3(0, 21, 0);
        roof.transform.localScale = new Vector3(16, 1, 16);
        roof.name = "Roof";

        for (int i = 0; i < 4; i++)
        {
            GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
            window.transform.SetParent(apartment.transform);
            window.transform.localPosition = new Vector3(-7.6f, 12 + (i * 4), 0);
            window.transform.localScale = new Vector3(2, 3, 1);
            window.transform.rotation = Quaternion.Euler(0, 90, 0);
            window.name = $"Window_{i}";
        }

        if (m_buildingMaterial != null)
        {
            mainBody.GetComponent<Renderer>().material = m_buildingMaterial;
            roof.GetComponent<Renderer>().material = m_buildingMaterial;
        }

        return apartment;
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

        GameObject wheelFL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelFL.transform.SetParent(car.transform);
        wheelFL.transform.localPosition = new Vector3(-1f, 0.35f, 1.2f);
        wheelFL.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        wheelFL.transform.rotation = Quaternion.Euler(0, 0, 90);
        wheelFL.name = "Wheel_FL";

        GameObject wheelFR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelFR.transform.SetParent(car.transform);
        wheelFR.transform.localPosition = new Vector3(1f, 0.35f, 1.2f);
        wheelFR.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        wheelFR.transform.rotation = Quaternion.Euler(0, 0, 90);
        wheelFR.name = "Wheel_FR";

        GameObject wheelRL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelRL.transform.SetParent(car.transform);
        wheelRL.transform.localPosition = new Vector3(-1f, 0.35f, -1.2f);
        wheelRL.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        wheelRL.transform.rotation = Quaternion.Euler(0, 0, 90);
        wheelRL.name = "Wheel_RL";

        GameObject wheelRR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelRR.transform.SetParent(car.transform);
        wheelRR.transform.localPosition = new Vector3(1f, 0.35f, -1.2f);
        wheelRR.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        wheelRR.transform.rotation = Quaternion.Euler(0, 0, 90);
        wheelRR.name = "Wheel_RR";

        return car;
    }

    public GameObject CreateStreetProp(string propType, Vector3 position)
    {
        switch (propType.ToLower())
        {
            case "lamp":
                return CreateStreetLamp(position);
            case "bench":
                return CreateBench(position);
            case "bin":
                return CreateTrashBin(position);
            case "barrier":
                return CreateBarrier(position);
            case "sign":
                return CreateSign(position);
            default:
                return CreateTrashDebris(position);
        }
    }

    private GameObject CreateStreetLamp(Vector3 position)
    {
        GameObject lamp = new GameObject("StreetLamp");
        lamp.transform.position = position;

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(lamp.transform);
        pole.transform.localPosition = new Vector3(0, 2.5f, 0);
        pole.transform.localScale = new Vector3(0.1f, 2.5f, 0.1f);
        pole.name = "Pole";

        GameObject light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        light.transform.SetParent(lamp.transform);
        light.transform.localPosition = new Vector3(0, 5f, 0);
        light.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        light.name = "Light";

        return lamp;
    }

    private GameObject CreateBench(Vector3 position)
    {
        GameObject bench = new GameObject("Bench");
        bench.transform.position = position;

        GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.transform.SetParent(bench.transform);
        seat.transform.localPosition = new Vector3(0, 0.5f, 0);
        seat.transform.localScale = new Vector3(2f, 0.1f, 0.5f);
        seat.name = "Seat";

        GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.transform.SetParent(bench.transform);
        back.transform.localPosition = new Vector3(0, 1f, -0.2f);
        back.transform.localScale = new Vector3(2f, 0.5f, 0.1f);
        back.name = "Back";

        return bench;
    }

    private GameObject CreateTrashBin(Vector3 position)
    {
        GameObject bin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bin.name = "TrashBin";
        bin.transform.position = position + Vector3.up * 0.5f;
        bin.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        return bin;
    }

    private GameObject CreateBarrier(Vector3 position)
    {
        GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrier.name = "Barrier";
        barrier.transform.position = position + Vector3.up * 0.5f;
        barrier.transform.localScale = new Vector3(2f, 1f, 0.2f);
        return barrier;
    }

    private GameObject CreateSign(Vector3 position)
    {
        GameObject sign = new GameObject("Sign");
        sign.transform.position = position;

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(sign.transform);
        pole.transform.localPosition = new Vector3(0, 1.5f, 0);
        pole.transform.localScale = new Vector3(0.1f, 1.5f, 0.1f);
        pole.name = "Pole";

        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.transform.SetParent(sign.transform);
        board.transform.localPosition = new Vector3(0, 3f, 0);
        board.transform.localScale = new Vector3(1f, 1f, 0.1f);
        board.name = "Board";

        return sign;
    }

    private GameObject CreateTrashDebris(Vector3 position)
    {
        GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Quad);
        debris.name = "Debris";
        debris.transform.position = position + Vector3.up * 0.01f;
        debris.transform.rotation = Quaternion.Euler(90, Random.Range(0, 360), 0);
        debris.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
        return debris;
    }
}
