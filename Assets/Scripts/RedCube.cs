using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCube : MonoBehaviour
{
    public Transform pointA;  // Ponto inicial da movimentação
    public Transform pointB;  // Ponto final da movimentação
    public float speed = 2.0f;  // Velocidade do cubo

    private Vector3 targetPosition;  // Alvo atual do cubo

    void Start()
    {
        // Inicia o movimento em direção ao ponto B
        targetPosition = pointB.position;
    }

    void Update()
    {
        // Move o cubo em direção ao ponto alvo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Alterna o alvo entre ponto A e B quando chega a um deles
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador colidiu com o cubo vermelho
        if (other.CompareTag("Player"))
        {
            // Obtém o componente PlayerController do jogador e chama o HandlePlayerFall
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.HandlePlayerFall();
            }
        }
    }
}
