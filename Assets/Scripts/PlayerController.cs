using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;
    public float jumpForce = 5f;
    private bool isGrounded;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    private int count;
    private bool jump = false;
    private Vector3 startPosition;

    // Variáveis para o cronômetro
    public float timeLimit = 180.0f;  // 3 minutos (180 segundos)
    private float timeRemaining;
    private bool timerIsRunning = true;
    private bool gameHasEnded = false;  // Novo: flag para checar se o jogo terminou

    public TextMeshProUGUI timerText;
    public GameObject loseTextObject;

    void Start(){
        rb = GetComponent<Rigidbody>();
        startPosition = new Vector3(0, 0, 0); // Define a posição inicial (ponto 0,0,0)
        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);  // Desativar o texto de derrota no início

        // Inicializando o cronômetro
        timeRemaining = timeLimit;
        UpdateTimerText();
    }

    void OnMove(InputValue movementValue){
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText(){
        countText.text = "Count: " + count.ToString();

        if(count >= 12 && !gameHasEnded){  // Verificação para não exibir a vitória se o jogo acabou
            winTextObject.SetActive(true);
            timerIsRunning = false;  // Para o cronômetro quando o jogador vencer
            gameHasEnded = true;     // Marca o jogo como terminado
        }
    }

    private void Update() {
        if (transform.position.y < -10){
            // Retornando o player para a posição inicial
            transform.position = startPosition;

            // Zerando a velocidade do player
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Cronômetro
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                // O tempo acabou, o jogador perdeu
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("Você perdeu!");

                // Exibir a mensagem de derrota
                loseTextObject.SetActive(true);
                gameHasEnded = true;  // Marca o jogo como terminado
                UpdateTimerText();     // Garante que o cronômetro mostre 00:00
            }
        }
    }

    void OnJump(){
        if (isGrounded)
           jump = true;
    }

    private void FixedUpdate(){
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        if (jump && isGrounded)
        {
            isGrounded = false;
            jump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Aplica uma força de impulso para o pulo
        }
    }
  
    private void OnCollisionEnter(Collision other) {
        isGrounded = true;
    }

    void OnCollisionExit()
    {
        // Quando a bola sai do chão, ela não pode mais pular
        isGrounded = false;
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("PickUp") && !gameHasEnded)  // Verificação para só contar se o jogo não terminou
        {
            other.gameObject.SetActive(false);
            count++;

            SetCountText();
        }
    }

    // Atualiza o texto do cronômetro na interface
    void UpdateTimerText(){
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Garante que o cronômetro não mostre valores negativos
        minutes = Mathf.Max(0, minutes);
        seconds = Mathf.Max(0, seconds);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
