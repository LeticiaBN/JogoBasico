using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;

    void Update()
    {
        //quando o jogador estiver perto do npc e apertar "E", o dialogo ira iniciar
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {   
            //se o painel de dialogo ja estiver ativo, reinicia o texto
            if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            //se o painel nao estiver ativo, ativa e inicia o dialogo
            else if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        //se o texto na tela ja estiver completo, ativa o botao de continue para ir para o proximo dialogo
        if (dialogueText != null && dialogueText.text == dialogue[index])
        {
            if (contButton != null)
            {
                contButton.SetActive(true);
            }
        }
    }

    //funcao para reiniciar o dialogo
    public void zeroText()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        index = 0;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if (contButton != null)
        {
            contButton.SetActive(false); 
        }
    }

    //coroutine que digita a fala do NPC letra por letra
    IEnumerator Typing()
    {
        if (dialogueText != null)
        {   
            //inicia com um texto vazio
            dialogueText.text = ""; 
            //para cada letra do dialogo, vai "caminhando" no texto e mostrando letra por letra na tela
            foreach (char letter in dialogue[index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }
    }

    //funcao para passar para a proxima linha do dialogo
    public void NextLine()
    {
        if (contButton != null)
        {
            contButton.SetActive(false);
        }

        if (index < dialogue.Length - 1)
        {
            index++;
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
            StartCoroutine(Typing());
        }
        else
        { //o dialogo com o npc eh o dialogo final do jogo
          //entao, restaura o tempo normal do jogo e carrega a cena de fim de jogo com vitoria
            zeroText();
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(2);
        }
    }

    //funcao chamada quando o jogador entrar na area de colisao do NPC
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    //funcao chamada quando o jogador sair da area de colisao do NPC
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }
}
