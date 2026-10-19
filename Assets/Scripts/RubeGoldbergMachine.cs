using UnityEngine;

public class RubeGoldbergMachine : MonoBehaviour
{
    private const int StationCount = 12;
    private static readonly string[] StationNames =
    {
        "Launch ramp", "Domino cascade", "Seesaw lever", "Pendulum swing",
        "Hinged gate", "Bouncy bumper", "Second domino cascade", "Counterweight seesaw",
        "Second pendulum", "Collapsing tower", "Ball ramp", "Finish trigger"
    };

    private readonly Color[] colors =
    {
        new Color(0.95f, 0.28f, 0.22f), new Color(1f, 0.65f, 0.1f),
        new Color(0.98f, 0.85f, 0.2f), new Color(0.25f, 0.75f, 0.38f),
        new Color(0.12f, 0.68f, 0.78f), new Color(0.25f, 0.42f, 0.95f),
        new Color(0.55f, 0.32f, 0.85f), new Color(0.85f, 0.32f, 0.68f),
        new Color(0.95f, 0.42f, 0.2f), new Color(0.5f, 0.72f, 0.25f),
        new Color(0.2f, 0.65f, 0.9f), new Color(0.95f, 0.82f, 0.28f)
    };

    private Rigidbody marble;
    private int currentStation = -1;
    private bool started;
    private bool finished;

    private void Start()
    {
        BuildMachine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !started)
        {
            started = true;
            marble.isKinematic = false;
            marble.AddForce(new Vector3(5f, 1.5f, 0f), ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);

            started = false;
            finished = false;
            currentStation = -1;
            BuildMachine();
        }
    }

    private void BuildMachine()
    {
        Physics.gravity = new Vector3(0f, -9.81f, 0f);
        PhysicsMaterial lowFriction = new PhysicsMaterial("Machine low friction")
        {
            dynamicFriction = 0.15f,
            staticFriction = 0.15f,
            bounciness = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };
        PhysicsMaterial bounce = new PhysicsMaterial("Machine bumper")
        {
            dynamicFriction = 0.1f,
            staticFriction = 0.1f,
            bounciness = 0.75f,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        GameObject floor = MakeCube("Runway", new Vector3(29f, -0.35f, 0f),
            new Vector3(64f, 0.7f, 5f), new Color(0.22f, 0.24f, 0.28f));
        floor.GetComponent<Collider>().material = lowFriction;
        MakeCube("Back wall", new Vector3(29f, 2.5f, 2.7f),
            new Vector3(64f, 5f, 0.25f), new Color(0.3f, 0.32f, 0.36f));

        marble = MakeSphere("Starter marble", new Vector3(0f, 0.7f, 0f), 0.7f,
            colors[0], true, true).GetComponent<Rigidbody>();
        marble.isKinematic = true;
        marble.mass = 1f;
        marble.drag = 0.03f;
        marble.GetComponent<Collider>().material = lowFriction;

        for (int i = 0; i < StationCount; i++)
        {
            float x = 4f + i * 4.8f;
            MakeCube("Station " + (i + 1) + " marker", new Vector3(x, 2.9f, 0f),
                new Vector3(0.55f, 0.55f, 0.55f), colors[i]);
            BuildStation(i, x, lowFriction, bounce);
            MakeCheckpoint(i, x + 1.6f);
        }

        MakeCube("Finish platform", new Vector3(60.5f, 0.15f, 0f),
            new Vector3(2.5f, 0.3f, 3.5f), colors[11]);

        GameObject lightObject = new GameObject("Machine light");
        lightObject.transform.SetParent(transform);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

        GameObject cameraObject = new GameObject("Machine camera");
        cameraObject.transform.SetParent(transform);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 35f;
        camera.transform.position = new Vector3(29f, 36f, -48f);
        camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -0.62f, 0.78f));
        camera.backgroundColor = new Color(0.12f, 0.16f, 0.22f);
    }

    private void BuildStation(int index, float x, PhysicsMaterial lowFriction, PhysicsMaterial bounce)
    {
        switch (index)
        {
            case 0:
            case 10:
                MakeRamp(x, 0.75f, 2.6f, 0.15f, colors[index]);
                break;
            case 1:
            case 6:
                BuildDominoes(x, colors[index]);
                break;
            case 2:
            case 7:
                BuildSeesaw(x, colors[index]);
                break;
            case 3:
            case 8:
                BuildPendulum(x, colors[index]);
                break;
            case 4:
                BuildHingedGate(x, colors[index]);
                break;
            case 5:
                GameObject bumper = MakeSphere("Spring bumper", new Vector3(x, 0.65f, 0f),
                    1f, colors[index], true, false);
                bumper.GetComponent<Collider>().material = bounce;
                bumper.GetComponent<Rigidbody>().mass = 2f;
                break;
            case 9:
                for (int row = 0; row < 3; row++)
                    for (int column = 0; column < 2; column++)
                        MakeCube("Collapsing tower block",
                            new Vector3(x + row * 0.55f, 0.35f + column * 0.65f, 0f),
                            new Vector3(0.55f, 0.6f, 0.8f), colors[index], true, true);
                break;
            case 11:
                BuildFinishTrigger(x);
                break;
        }

        if (index != 11)
            MakeCube("Station base", new Vector3(x, 0.08f, 0f),
                new Vector3(3.2f, 0.16f, 2.4f), new Color(0.33f, 0.36f, 0.4f));
    }

    private void BuildDominoes(float x, Color color)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject domino = MakeCube("Domino " + (i + 1),
                new Vector3(x - 0.8f + i * 0.32f, 0.65f, 0f),
                new Vector3(0.16f, 1.1f, 0.45f), color, true, true);
            domino.GetComponent<Rigidbody>().mass = 0.25f;
        }
    }

    private void BuildSeesaw(float x, Color color)
    {
        MakeCube("Seesaw fulcrum", new Vector3(x, 0.45f, 0f),
            new Vector3(0.45f, 0.7f, 0.7f), new Color(0.65f, 0.65f, 0.68f));
        GameObject plank = MakeCube("Hinged seesaw", new Vector3(x, 0.95f, 0f),
            new Vector3(2.8f, 0.18f, 0.8f), color, true, true);
        plank.GetComponent<Rigidbody>().mass = 1.5f;
        AddHinge(plank, Vector3.zero, new Vector3(0f, 0f, 1f), -25f, 25f);
    }

    private void BuildPendulum(float x, Color color)
    {
        MakeCube("Pendulum support", new Vector3(x, 2.65f, 0f),
            new Vector3(1.8f, 0.18f, 0.8f), new Color(0.65f, 0.65f, 0.68f));
        GameObject bob = MakeSphere("Pendulum bob", new Vector3(x, 1.35f, 0f),
            0.65f, color, true, true);
        bob.GetComponent<Rigidbody>().mass = 1.2f;
        AddHinge(bob, new Vector3(0f, 1.3f, 0f), new Vector3(0f, 0f, 1f), -70f, 70f);
    }

    private void BuildHingedGate(float x, Color color)
    {
        GameObject gate = MakeCube("Hinged gate", new Vector3(x, 1.05f, 0f),
            new Vector3(0.25f, 2f, 1.2f), color, true, true);
        gate.GetComponent<Rigidbody>().mass = 1f;
        AddHinge(gate, new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f), -85f, 5f);
    }

    private void BuildFinishTrigger(float x)
    {
        GameObject goal = MakeCube("Finish trigger", new Vector3(x, 0.75f, 0f),
            new Vector3(0.4f, 1.5f, 2.5f), colors[11]);
        Collider trigger = goal.GetComponent<Collider>();
        trigger.isTrigger = true;
        goal.AddComponent<FinishRelay>().machine = this;
    }

    private void MakeCheckpoint(int index, float x)
    {
        GameObject checkpoint = new GameObject("Checkpoint " + (index + 1));
        checkpoint.transform.SetParent(transform);
        checkpoint.transform.position = new Vector3(x, 0.7f, 0f);
        BoxCollider trigger = checkpoint.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(0.5f, 2.5f, 3f);
        StationRelay relay = checkpoint.AddComponent<StationRelay>();
        relay.machine = this;
        relay.station = index;
    }

    private void MakeRamp(float x, float y, float length, float angle, Color color)
    {
        GameObject ramp = MakeCube("Downhill ramp", new Vector3(x, y, 0f),
            new Vector3(length, 0.2f, 2.2f), color);
        ramp.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        ramp.GetComponent<Collider>().material = new PhysicsMaterial("Ramp friction")
        {
            dynamicFriction = 0.15f,
            staticFriction = 0.15f,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };
    }

    private void AddHinge(GameObject body, Vector3 anchor, Vector3 axis, float min, float max)
    {
        HingeJoint hinge = body.AddComponent<HingeJoint>();
        hinge.anchor = anchor;
        hinge.axis = axis;
        hinge.useLimits = true;
        JointLimits limits = hinge.limits;
        limits.min = min;
        limits.max = max;
        hinge.limits = limits;
    }

    private GameObject MakeCube(string name, Vector3 position, Vector3 scale, Color color,
        bool dynamic = false, bool useGravity = false)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(transform);
        item.transform.position = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().material = MakeMaterial(color);
        if (dynamic)
        {
            Rigidbody body = item.AddComponent<Rigidbody>();
            body.useGravity = useGravity;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }
        return item;
    }

    private GameObject MakeSphere(string name, Vector3 position, float diameter, Color color,
        bool dynamic, bool useGravity)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = name;
        item.transform.SetParent(transform);
        item.transform.position = position;
        item.transform.localScale = Vector3.one * diameter;
        item.GetComponent<Renderer>().material = MakeMaterial(color);
        if (dynamic)
        {
            Rigidbody body = item.AddComponent<Rigidbody>();
            body.useGravity = useGravity;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }
        return item;
    }

    private Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Lit");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    private void OnStationReached(int station)
    {
        if (station <= currentStation || finished)
            return;

        currentStation = station;
        if (started && marble != null)
            marble.AddForce(Vector3.right * 2f, ForceMode.Impulse);
    }

    private void OnFinish()
    {
        finished = true;
        currentStation = StationCount - 1;
    }

    private void OnGUI()
    {
        string status = finished ? "Machine complete!" :
            started ? "Interaction " + Mathf.Clamp(currentStation + 1, 1, StationCount) +
                      " / " + StationCount + ": " + StationNames[Mathf.Max(currentStation, 0)] :
            "Press SPACE to start | R to reset";
        GUI.Box(new Rect(12f, 12f, 390f, 48f), "RUBE GOLDBERG MACHINE   •   " + status);
        GUI.Box(new Rect(12f, 66f, 250f, 30f), "12 physics interactions • spheres + cubes");
    }

    private class StationRelay : MonoBehaviour
    {
        public RubeGoldbergMachine machine;
        public int station;

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                machine.OnStationReached(station);
        }
    }

    private class FinishRelay : MonoBehaviour
    {
        public RubeGoldbergMachine machine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                machine.OnFinish();
        }
    }
}
