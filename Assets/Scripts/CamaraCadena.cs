using UnityEngine;

public class CamaraCadena : MonoBehaviour
{
    public Transform[] puntos;   // empties con la vista de cada etapa
    public float suavizado = 0.6f;
    public float giro = 3f;

    int actual = 0;
    Vector3 velocidad;

    void Start()
    {
        if (puntos.Length == 0) return;
        transform.position = puntos[0].position;
        transform.rotation = puntos[0].rotation;
    }

    public void IrAEtapa(int i)
    {
        if (i > actual && i < puntos.Length) actual = i;
    }

    void LateUpdate()
    {
        if (puntos.Length == 0) return;
        Transform p = puntos[actual];
        transform.position = Vector3.SmoothDamp(transform.position, p.position, ref velocidad, suavizado);
        transform.rotation = Quaternion.Slerp(transform.rotation, p.rotation, giro * Time.deltaTime);
    }
}