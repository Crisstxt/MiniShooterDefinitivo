using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{

    #region Variables
    [Header("Rotacion y Moviemiento")]
    [SerializeField] private float velocidad = 10f;
    private int puntos;
    private int ladoZonaRespawn;
    private TipoMoneda tipoMoneda;

    public delegate void recolectar(int puntos);
    public static event recolectar Monedarecolectada;

    #endregion

    public enum TipoMoneda
    {
        Oro,
        Bronce,
        Plata
    }

    void Start()
    {
        ladoZonaRespawn = MiniShooter.instance.GetLadoZonaRespawn();
        CambiarTipo();
    }

    void Update()
    {
        GirarMoneda();
    }

     private void GirarMoneda()
     {
        transform.Rotate(new Vector3(0, 0, velocidad * Time.deltaTime));
    }

    public void RecolectarMoneda()
    {
        Monedarecolectada?.Invoke(puntos);
        CambiarTipo();
        Respawn();
    }

    private void CambiarTipo()
    {
        System.Array valores = System.Enum.GetValues(typeof(TipoMoneda));
        tipoMoneda = (TipoMoneda)valores.GetValue(new System.Random().Next(valores.Length));

        GetComponent<Renderer>().material.color = tipoMoneda switch
        {
            TipoMoneda.Oro => MiniShooter.instance.GetColorOro(),
            TipoMoneda.Plata => MiniShooter.instance.GetColorPlata(),
            TipoMoneda.Bronce => MiniShooter.instance.GetColorBronce(),
            _ => MiniShooter.instance.GetColorOro(),
        };

        puntos = tipoMoneda switch
        {
            TipoMoneda.Oro => 5,
            TipoMoneda.Plata => 3,
            TipoMoneda.Bronce => 1,
            _ => 0
        };
    }

    private void Respawn()
    {
        transform.position = new Vector3
            (Random.Range(-ladoZonaRespawn, ladoZonaRespawn),
            transform.position.y, Random.Range(-ladoZonaRespawn, ladoZonaRespawn));
    }
}
