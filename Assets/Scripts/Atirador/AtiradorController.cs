using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtiradorController : MonoBehaviour
{
    //Prefab para criar
    [SerializeField] private GameObject FlechaPrefab;
    //Intervalo entre as flehcasflehcas
    [SerializeField] private float Intervalo;
    //Para saber quando pode atirar
    private float UltimoTiro;
    public float shootOffsetY = 1.0f;

    void Start()
    {
        //Salva o momento em que a cena foi carregada, o tempo = 0 da cena
        UltimoTiro = Time.time;
    }

    void Update()
    {
        //Se o tempo � maior que o tempo do �ltimo tiro + o intervalo
        if(Time.time >= Intervalo + UltimoTiro)
        {
            //Cria uma rota��o
            Quaternion rotacao = new Quaternion();
            //Define a rota��o em fun��o de graus (�)
            rotacao.eulerAngles = new Vector3(0, 0, 180);

            // Ajuste a posição inicial da flecha com um deslocamento vertical
            Vector3 adjustedPosition = transform.position + new Vector3(0, shootOffsetY, 0);

            //Instancia a flecha apontando pra frente
            Instantiate(FlechaPrefab, adjustedPosition, rotacao);

            //Salva o momento do tiro
            UltimoTiro = Time.time;
        }
    }
}
