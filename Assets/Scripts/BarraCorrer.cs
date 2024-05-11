using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraCorrer : MonoBehaviour
{
    [SerializeField] private RectTransform barraTiempoCorrer;
    [SerializeField] private Image barraCorrer;
    [SerializeField] private Image barraCorreFondo;
    private float anchoMaximo;

    void Start()
    {
        anchoMaximo = barraTiempoCorrer.sizeDelta.x;
        Desabilitar();
    }

    void Update()
    {
        if (BarraCorrerCompleta()) Desabilitar();
        else ActualizarBarra();
    }

    private void ActualizarBarra()
    {
        Habilitar();

        float porcentajeTiempoCorrer = MiniShooter.instance.GetPorcentajeDuracionCorrer();
        barraTiempoCorrer.sizeDelta = new Vector2(anchoMaximo * porcentajeTiempoCorrer, barraTiempoCorrer.sizeDelta.y);

        if (porcentajeTiempoCorrer > 0.8f) barraCorrer.color = Color.green;
        else if (porcentajeTiempoCorrer > 0.3f) barraCorrer.color = Color.yellow;
        else barraCorrer.color = Color.red;
    }

    private void Desabilitar()
    {
        barraCorrer.enabled = false;
        barraCorreFondo.enabled = false;
    }

    private void Habilitar()
    {
        barraCorrer.enabled = true;
        barraCorreFondo.enabled = true;
    }

    private bool BarraCorrerCompleta()
    {
        if (MiniShooter.instance.GetDuracionRestante == MiniShooter.instance.GetDuracionCorrer)
            return true;
        else
            return false;
    }
}
