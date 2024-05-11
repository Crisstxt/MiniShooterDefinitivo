using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniShooter : MonoBehaviour
{
    #region Variables
    [Header("HUD")]
    [SerializeField] TextMeshProUGUI txtFinPartidad;
    [SerializeField] TextMeshProUGUI txtPuntuacion;
    [SerializeField] TextMeshProUGUI txtOleada;
    [SerializeField] TextMeshProUGUI txtEnemigos;
    [SerializeField] TextMeshProUGUI txtBalas;

    [Header("Jugador Principal")]
    [SerializeField] private GameObject jugadorPrincipal;   
    [SerializeField] private float velocidadCaminar = 5f;
    [SerializeField] private float velocidadCorrer = 15f;
    [SerializeField] private float duracionCorrer = 3f;
    [SerializeField] private float sensibilidadRaton = 10f;
    [SerializeField][Range(1, 10)] private float suavizado = 5f;
    [SerializeField] private float limiteRotacionVertical = 45.0f;
    [SerializeField] private float fuerzaSalto = 2f;
    [SerializeField] private float gravedad = -9.81f;

    [Header("Posiciones Camara Principal")]
    [SerializeField] private Vector3[] posicionesCamara;
    private int posicionActual = 0;

    [Header("Enemigos y Monedas")]
    [SerializeField] private int ladoZonaRespawn = 30;
    [SerializeField] private float velocidadEnemigos = 5f;
    [SerializeField] private float fuerzaEmpuje = 5f;
    [SerializeField] private float fuerzaGiro = 5f;
    [SerializeField] Color Oro;
    [SerializeField] Color Bronce;
    [SerializeField] Color Plata;


    private List<Enemigo> enemigos;

    private float duracionRestante;
    private int enemigosRestantes;
    private float velocidadJugador;
    private int oleada;
    private int puntuacion = 0;
    public static MiniShooter instance;

    #endregion

    void Awake()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        oleada = 0;
        enemigosRestantes = 3;
        txtFinPartidad.enabled = false;
        duracionRestante = duracionCorrer;
        velocidadJugador = velocidadCaminar;
        enemigos = new List<Enemigo>();
    }

    public void AgregarEnemigo(Enemigo enemigo)
    {
        enemigos.Add(enemigo);
        enemigosRestantes--;
        enemigo.gameObject.SetActive(false);

        if(enemigosRestantes == 0)
        {
            GameObject nuevoEnemigo = Instantiate(enemigo.gameObject);
            nuevoEnemigo.gameObject.SetActive(true);
            nuevoEnemigo.GetComponent<Enemigo>().CambiarPos();

            Invoke("ActivarOleada", 5f);
        }
    }

    private void ActivarOleada()
    {
        velocidadEnemigos *= 1.2f;
        enemigosRestantes += 2;
        oleada++;

        foreach (Enemigo enemy in enemigos)
        {
            enemy.CambiarPos();
            enemy.gameObject.SetActive(true);
        }
        enemigos.Clear();

        txtOleada.SetText($"Oleada: {oleada}");
        txtEnemigos.SetText($"Enemigos: {enemigosRestantes}");
    }

    public void FinPartida()
    {
        txtFinPartidad.enabled = true;
        Time.timeScale = 0f;
    }

    public Vector3 CambiarPosicionCamara()
    {
        return posicionesCamara[++posicionActual % posicionesCamara.Length]; 
    }

    public void Correr()
    {
        if (duracionRestante > 0)
        {
            velocidadJugador = velocidadCorrer;
            duracionRestante = Mathf.Max(0, duracionRestante - Time.deltaTime);
        }
        else Caminar();
    }

    public void Caminar()
    {
        if (duracionRestante < duracionCorrer)
        {
            velocidadJugador = velocidadCaminar;
            duracionRestante = Mathf.Min(duracionCorrer, duracionRestante + Time.deltaTime);
        }
    }

    public bool EstaEnPrimeraPersona()
    {
        return posicionActual == 0;
    }

    public Vector3 PosActualCamara()
    {
        return posicionesCamara[posicionActual];
    }

    #region Getters
    public GameObject GetJugador
    {
        get { return jugadorPrincipal;  }
    }

    public float GetSensibilidadRaton
    {
        get { return sensibilidadRaton; }
    }

    public float GetLimiteRotacionVertical
    {
       get { return limiteRotacionVertical; }
    }

    public Vector3[] GetPosicionesCamara
    {
        get { return posicionesCamara; }
    }

    public float GetVelocidadEnemigos()
    {
        return velocidadEnemigos;
    }

    public float GetGravedad
    {
        get { return gravedad;  }
    }

    public int GetLadoZonaRespawn()
    {
        return ladoZonaRespawn;
    }

    public Color GetColorOro()
    {
        return Oro;
    }

    public Color GetColorBronce()
    {
        return Bronce;
    }
    public Color GetColorPlata()
    {
        return Plata;
    }

    public float GetSuavizado()
    {
        return suavizado;
    }

    public float GetFuerzaSalto()
    {
        return fuerzaSalto;
    }

    public float GetPorcentajeDuracionCorrer()
    {
        return duracionRestante / duracionCorrer;
    }

    public float GetDuracionRestante
    {
        get { return duracionRestante; }
    }

    public float GetDuracionCorrer
    {
        get { return duracionCorrer; }
    }

    public float GetVelocidadJugador
    {
        get { return velocidadJugador; }
    }

    public float GetFuerzaEmpuje()
    {
        return fuerzaEmpuje;
    }

    public float GetFuerzaGiro()
    {
        return fuerzaGiro;
    }
    #endregion

    #region Eventos
    private void OnEnable()
    {
        Enemigo.EnemigoImpacto += ActualizarPuntuacion;
        Moneda.Monedarecolectada += ActualizarPuntuacion;
        JugadorCC.balasActualizadas += ActualizarBalas;
    }

    private void OnDisable()
    {
        Enemigo.EnemigoImpacto -= ActualizarPuntuacion;
        Moneda.Monedarecolectada -= ActualizarPuntuacion;
        JugadorCC.balasActualizadas -= ActualizarBalas;
    }

    private void ActualizarBalas(int balas)
    {
        txtBalas.SetText($"Balas: {balas}");
    }

    private void ActualizarPuntuacion(int puntos)
    {
        puntuacion += puntos;
        txtPuntuacion.SetText($"Puntos: {puntuacion}");
        txtOleada.SetText($"Oleada: {oleada}");
        txtEnemigos.SetText($"Enemigos: {enemigosRestantes}");
    }
    #endregion
}
