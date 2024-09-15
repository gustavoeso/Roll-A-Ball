using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;  // Referência ao jogador
    public float speed = 5f;  // Velocidade do inimigo
    public float detectionRange = 10f;  // Distância máxima em que o inimigo persegue o jogador
    public Vector3 mapCenter;  // Centro da região onde o inimigo pode se mover
    public float regionRadius = 20f;  // Raio da região permitida para o inimigo

    private Rigidbody rb;
    private PlayerController playerController;  // Referência ao script do jogador

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();  // Obtém a referência ao script do jogador
    }

    void FixedUpdate()
    {
        // Calcula a distância do jogador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Verifica se o jogador está dentro da área de detecção
        if (distanceToPlayer <= detectionRange)
        {
            // Direção em que o inimigo vai se mover
            Vector3 direction = (player.position - transform.position).normalized;

            // Aplica a força para o inimigo se mover na direção do jogador
            rb.AddForce(direction * speed);
        }

        // Verifica se o inimigo está dentro da região permitida
        if (Vector3.Distance(transform.position, mapCenter) > regionRadius)
        {
            // Se o inimigo sair da região, empurra-o de volta para o centro
            Vector3 directionToCenter = (mapCenter - transform.position).normalized;
            rb.AddForce(directionToCenter * speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o inimigo colidiu com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verifica se o PlayerController existe e chama a função para tirar vida e fazer o respawn
            if (playerController != null)
            {
                playerController.HandlePlayerFall();  // Chama a função de respawn e perda de vida do jogador
            }
        }
    }

    // Método para desenhar o raio de detecção e a área da região
    private void OnDrawGizmos()
    {
        // Define a cor para o raio de detecção do jogador
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  // Desenha o raio de detecção

        // Define a cor para a área onde o inimigo pode se mover
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(mapCenter, regionRadius);  // Desenha o limite da região permitida
    }
}
