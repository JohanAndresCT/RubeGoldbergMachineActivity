using UnityEngine;

public class KeepOrbiting : MonoBehaviour
{
    public Transform funnel;
    public float extraGravity = 15f;   // más = más rápido todo
    public float startFactor = 0.95f;  // velocidad de entrada vs órbita estable (1 = estable)
    public float decay = 0.05f;        // sube = baja más rápido, baja = más vueltas
    public float minFactor = 0.5f;     // límite inferior del factor
    public float stopRadius = 1.3f;    // cerca del hueco deja de controlar y cae
    public float rimRadius = 5.2f;     // desde aquí se evita que se pegue al muro
    public float accel = 15f;          // qué tan rápido ajusta el giro
    public bool clockwise = true;      // sentido si entra sin giro
    public float wellK = 1.8f;         // constante del embudo v3

    Rigidbody rb;
    float lastContact = -10f;
    bool active;
    float f;
    float dir;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void OnCollisionStay(Collision c)
    {
        if (c.transform == funnel || c.transform.IsChildOf(funnel))
            lastContact = Time.time;
    }

    void FixedUpdate()
    {
        bool inFunnel = Time.time - lastContact < 0.3f;
        if (!inFunnel) { active = false; return; }

        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

        Vector3 off = rb.position - funnel.position;
        off.y = 0;
        float r = off.magnitude;
        if (r < 0.01f) return;

        Vector3 radial = off / r;
        Vector3 tangent = Vector3.Cross(Vector3.up, radial);
        Vector3 vel = rb.linearVelocity;   // Unity anterior a 6: rb.velocity
        float vt = Vector3.Dot(vel, tangent);

        if (!active)
        {
            active = true;
            f = startFactor;
            dir = Mathf.Abs(vt) > 0.3f ? Mathf.Sign(vt) : (clockwise ? -1f : 1f);
        }

        // evita que se quede pegada al muro: quita la velocidad hacia afuera cerca del borde
        float vr = Vector3.Dot(vel, radial);
        if (r > rimRadius && vr > 0f) vel -= radial * vr;

        if (r < stopRadius) { rb.linearVelocity = vel; return; }

        // el factor baja con el tiempo: cada vez gira un poco menos que la órbita estable
        f = Mathf.Max(minFactor, f - decay * Time.fixedDeltaTime);

        float gEff = 9.81f + extraGravity;
        float orbitSpeed = Mathf.Sqrt(gEff * wellK / r);   // velocidad de órbita estable a este radio
        float target = dir * orbitSpeed * f;

        float newVt = Mathf.MoveTowards(vt, target, accel * Time.fixedDeltaTime);
        rb.linearVelocity = vel + tangent * (newVt - vt);
    }
}