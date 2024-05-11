using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorCC : MonoBehaviour
{
    #region Variables

    private CharacterController characterController;
    private Vector3 velocidadJugador;
    private Vector2 velRotacion;
    private Animator animator;

    private float amplitudCabeceo;
    private float frecuenciaCabeceo;
    private float tiempoCabeceo;



    [Header("Provisional Arma Configuración")]
    [SerializeField] private float fuerzaRetroceso = 3f;
    [SerializeField] private float velocidadRetorno = 10f;
    [SerializeField] private float cantidadRetroceso = 0f;
    [SerializeField] private int balas = 50;
    [SerializeField] private float tiempoRecarga = 3f;
    private bool recargando;
    private float tiempoUltimoTiro;
    [SerializeField] [Range(0.1f, 1f)] private float velocidadDisparo = 0.5f;

    public delegate void balasAct(int balas);
    public static event balasAct balasActualizadas;

    #endregion

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        recargando = false;
        tiempoUltimoTiro = 0;
    }

    void Update()
    {
        Mover();
        Camara();

        if (!recargando) tiempoUltimoTiro += Time.deltaTime;

        if (Input.GetMouseButton(0)) Disparar();

        if (Input.GetKeyDown(KeyCode.R)) Recargar();
    }

    void LateUpdate()
    {
        if (cantidadRetroceso > 0)
        {
            Camera.main.transform.Rotate(-cantidadRetroceso, 0 , 0);
            cantidadRetroceso -= velocidadRetorno * Time.deltaTime;
        }
    }

    private void AplicarRetroceso()
    {
        cantidadRetroceso += fuerzaRetroceso;
    }

    private void Mover()
    {
        float movimientoX = Input.GetAxis("Horizontal");
        float movimientoZ = Input.GetAxis("Vertical");

        animator.SetFloat("MovX", movimientoX * MiniShooter.instance.GetVelocidadJugador);
        animator.SetFloat("MovZ", movimientoZ * MiniShooter.instance.GetVelocidadJugador);

        if (movimientoX == 0 && movimientoZ == 0) animator.SetBool("quieto", true);
        else animator.SetBool("quieto", false);

        Vector3 movimiento = transform.right * movimientoX + transform.forward * movimientoZ;
        characterController.Move(MiniShooter.instance.GetVelocidadJugador * Time.deltaTime * movimiento);

        if (characterController.isGrounded && velocidadJugador.y < 0) velocidadJugador.y = 0f;
        velocidadJugador.y += MiniShooter.instance.GetGravedad * Time.deltaTime;
        characterController.Move(velocidadJugador * Time.deltaTime);

        if (animator.GetBool("saltando") && characterController.isGrounded) animator.SetBool("saltando", false); 

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            if (animator.GetBool("quieto")) Invoke("Saltar", .5f);
            else Saltar();

            animator.SetBool("saltando", true);
        }
           
        if (Input.GetKey(KeyCode.LeftShift)) MiniShooter.instance.Correr();
        else MiniShooter.instance.Caminar();

        SimularCabeceo();
    }

    private void Saltar()
    {
        velocidadJugador.y += Mathf.Sqrt(3 * -2f * MiniShooter.instance.GetGravedad);
    }

    private void Camara()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * MiniShooter.instance.GetSensibilidadRaton;

        velRotacion.x = Mathf.Lerp(velRotacion.x, velRotacion.x + mouseDelta.x, MiniShooter.instance.GetSuavizado() * Time.deltaTime);
        velRotacion.y = Mathf.Lerp(velRotacion.y, velRotacion.y + mouseDelta.y, MiniShooter.instance.GetSuavizado() * Time.deltaTime);

        velRotacion.y = Mathf.Clamp(velRotacion.y, -MiniShooter.instance.GetLimiteRotacionVertical, MiniShooter.instance.GetLimiteRotacionVertical);

        Camera.main.transform.localRotation = Quaternion.AngleAxis(-velRotacion.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(velRotacion.x, Vector3.up);

        if (MiniShooter.instance.GetPosicionesCamara.Length > 0 && Input.GetKeyDown(KeyCode.C))
            Camera.main.transform.localPosition = MiniShooter.instance.CambiarPosicionCamara();
    }

    private void SimularCabeceo()
    {
        if(!animator.GetBool("quieto") && MiniShooter.instance.EstaEnPrimeraPersona())
        {
            amplitudCabeceo = MiniShooter.instance.GetVelocidadJugador / 50f;
            frecuenciaCabeceo = MiniShooter.instance.GetVelocidadJugador;
            tiempoCabeceo += Time.deltaTime * frecuenciaCabeceo;
            float alturaCabeceo = Mathf.Sin(tiempoCabeceo) * amplitudCabeceo;

            Camera.main.transform.localPosition = new Vector3(0, alturaCabeceo, 0) + MiniShooter.instance.PosActualCamara();
        }
    }

    private void Disparar()
    {
        if (tiempoUltimoTiro >= velocidadDisparo && balas > 0 && !recargando)
        {
            tiempoUltimoTiro = 0;
            balas--;
            AplicarRetroceso();
            balasActualizadas?.Invoke(balas);

            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayo, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<Enemigo>()?.Destruir();
            }
        }

        if(balas == 0)
        {
            Recargar();
        }
    }

    private void Recargar()
    {
        recargando = true;
        Invoke(nameof(Recargando), tiempoRecarga);
    }

    private void Recargando()
    {
        recargando = false;
        balas = 50;
        balasActualizadas?.Invoke(balas);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.CompareTag("Enemigo")) MiniShooter.instance.FinPartida();

            if (other.CompareTag("Moneda")) other.GetComponent<Moneda>()?.RecolectarMoneda();
        }
    }
}
