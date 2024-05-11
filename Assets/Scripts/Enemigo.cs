using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    #region Variables
    private int puntos = 1;

    public delegate void impacto(int puntos);
    public static event impacto EnemigoImpacto;
    #endregion

    void Update()
    {
        SeguirJugador();
    }

    private void SeguirJugador()
    {

        transform.position = Vector3.MoveTowards(transform.position, MiniShooter.instance.GetJugador.transform.position,
            MiniShooter.instance.GetVelocidadEnemigos() * Time.deltaTime);
    }

    public void Destruir()
    {
        EnemigoImpacto?.Invoke(++puntos);
        MiniShooter.instance.AgregarEnemigo(this);
    }

    public void CambiarPos()
    {
        Vector3 nuevaPos;
        do
        {
            nuevaPos = 
                new Vector3 (Random.Range(-MiniShooter.instance.GetLadoZonaRespawn(), MiniShooter.instance.GetLadoZonaRespawn()),
            transform.position.y, Random.Range(-MiniShooter.instance.GetLadoZonaRespawn(), MiniShooter.instance.GetLadoZonaRespawn()));

        } while (Vector3.Distance(MiniShooter.instance.GetJugador.transform.position, nuevaPos) < MiniShooter.instance.GetLadoZonaRespawn());

        transform.position = new (nuevaPos.x, transform.position.y, nuevaPos.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemigo enemigo = collision.gameObject.GetComponent<Enemigo>();

        if(enemigo != null)
        {
            Rigidbody rb = collision.rigidbody;
            if (rb != null)
            {
                Vector3 direccionEmpuje = collision.transform.position - transform.position;
                direccionEmpuje = direccionEmpuje.normalized * MiniShooter.instance.GetFuerzaEmpuje();
                rb.AddForce(direccionEmpuje, ForceMode.Impulse);

                Vector3 torque = Random.insideUnitSphere * MiniShooter.instance.GetFuerzaGiro();
                rb.AddTorque(torque);
            }
        }
    }
}
