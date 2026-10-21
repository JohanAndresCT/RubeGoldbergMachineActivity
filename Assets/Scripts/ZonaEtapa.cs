using UnityEngine;

public class ZonaEtapa : MonoBehaviour
{
    public CamaraCadena camara;
    public int etapa;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            camara.IrAEtapa(etapa);
    }
}