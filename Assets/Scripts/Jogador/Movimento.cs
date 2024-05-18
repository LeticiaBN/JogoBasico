using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    [SerializeField] private float Velocidade;
    [SerializeField] private Transform PeDoPersonagem;
    [SerializeField] private LayerMask Chao;
    //O corpo do jogador
    [SerializeField] private Rigidbody2D Corpo;
    //Para ele não pular infinitamente
    [SerializeField] private float ForcaPulo;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private LayerMask wallLayer;

    private bool isFacingRight = true;
    //Contador de pulos para implementar pulo duplo
    private int ContadorPulos = 0;

    private bool PodePular = false;
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(4f, 8f);
    private float jumpingPower = 8f;

    void Update()
    {
        //Neste caso, não se usa Time.deltaTime, porque RigidBody2D.velocity já opera baseado na taxa de frames
        Corpo.velocity = new Vector2(movimento_horizontal(), Corpo.velocity.y);
        
        Flip();

        wallJump();

        Slide();

        Jump();
    }

    private float movimento_horizontal() {
        //Define a velocidade do corpo baseada na tecla pressionada (Input.GetAxisRaw("Horizontal"))
        //A função retorna 1 se a seta pra direita ou D foram pressionados
        //Retorna -1 se a seta da esquerda ou A foram pressionados
        //Retorna 0 se nenhum direcional foi pressionado
        return (Velocidade * Input.GetAxisRaw("Horizontal"));
    }
    private bool PertoDoChao() {
        //Cria uma caixa, se a caixa colidir com o chao, pode pular
        //Nessa função se passa a posição, tamanho, angulo e distancia(tamanho) em relação a direção
        //Tambem passa um layer mask, pra que somente os layers associados a Chao sejam considerados
        bool pertoChao = Physics2D.BoxCast(PeDoPersonagem.position, new Vector2(0.5f, 0.2f), 0f, Vector2.down, 0.1f, Chao);
        
        return pertoChao;
    }

    private bool isWalled() {
        //verifica se esta na parede
        bool walled = Physics2D.OverlapCircle(wallCheckLeft.position, 0.2f, wallLayer);
        if(!walled) {
            walled = Physics2D.OverlapCircle(wallCheckRight.position, 0.2f, wallLayer);
        }

        return walled;
    }

    //funcao para o walljump
    private void wallJump() {
        //verifica se o personagem esta deslizando na parede
        if(isWallSliding) {
            //se estiver deslizando, nao esta fazendo walljump e atualiza a direçao do personagem
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter -= Time.deltaTime; 
        }

        //se a tecla de espaço for pressionada e o contador de pulo na parede for maior que 0
        //o personagem ira fazer o walljump
        if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f) {
            //atualiza as variaveis necessarias
            //ou seja, velocidade do corpo, contador de pulos e o boolean que esta em um walljump
            isWallJumping = true; 
            Corpo.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            //atualiza a direça do personagem
            if(transform.localScale.x != wallJumpingDirection) {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    //para o walljump
    private void StopWallJumping() {
        isWallJumping = false;
    }

    //para fazer com que o personagem esteja olhando na direcao certa
    //ou seja, se o jogador estiver indo pra direita, ele estara virado para a direita, e o mesmo ocorrera com a esquerda
    private void Flip() {
        if(isFacingRight && movimento_horizontal() < 0f || !isFacingRight && movimento_horizontal() > 0f) {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //pulo
    private void Jump() {
        //Se o acerto tem um resultado não nulo, o contador de pulos zera
        if(PertoDoChao())
        {
            ContadorPulos = 0;
        }

        //se ja tiver usado os dois pulos, nao pode pular ate encostar no chao
        if(ContadorPulos < 1) {
            PodePular = true;
        } else {
            PodePular = false;
        }

        //Se a barra de espaço foi pressionada e o jogador pode pular
        if (Input.GetKeyDown(KeyCode.Space) && PodePular)
        {
            //Adiciona uma força para cima proporcional à ForçaPulo
            Corpo.velocity = new Vector2(Corpo.velocity.x, jumpingPower);
            //Incrementa o contador de pulos
            ContadorPulos++;
            //Proíbe o jogador de pular
            PodePular = false;
        }
    }
    
    //slide na parede utilizado para o walljump
    private void Slide() {
        //se estiver na parede e nao estiver no chao, vai "deslizar" na parede
        if(isWalled() && !PertoDoChao() && movimento_horizontal() != 0f) {
            isWallSliding = true;
            Corpo.velocity = new Vector2(Corpo.velocity.x, Mathf.Clamp(Corpo.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }
}
