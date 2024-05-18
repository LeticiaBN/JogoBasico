using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnJogarController : MonoBehaviour
{
    public void Jogar()
    {
        //Entra na cena do jogo
        SceneManager.LoadScene(1);
    }
}
