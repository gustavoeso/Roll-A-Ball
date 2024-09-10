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
    [SerializeField] private float rayDistance = 0.5f;  // Distância do raycast para verificar se está no chão
    public LayerMask groundLayer;  // Camada do chão para detectar o solo com o raycast
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    private int count;
    private bool jump = false;
    private Vector3 startPosition;

    // Variáveis para o cronômetro
    public float timeLimit = 180.0f;  // 3 minutos (180 segundos)
    private float timeRemaining;
    private bool timerIsRunning = true;
    private bool gameHasEnded = false;  // Flag para verificar se o jogo terminou

    // Variáveis para o sistema de vidas
    public int maxLives = 3;
    private int currentLives;
    public TextMeshProUGUI livesText;  // UI para exibir as vidas
    public GameObject loseTextObject;  // Objeto de texto para a mensagem de derrota

    public TextMeshProUGUI timerText;

    void Start(){
        rb = GetComponent<Rigidbody>();
        startPosition = new Vector3(0, 0, 0); // Define a posição inicial (ponto 0,0,0)
        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);  // Desativa o texto de derrota no início

        // Inicializando o cronômetro
        timeRemaining = timeLimit;
        UpdateTimerText();

        // Inicializando o sistema de vidas
        currentLives = maxLives;
        UpdateLivesText();  // Atualiza o texto das vidas na interface
    }

    void OnMove(InputValue movementValue){
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText(){
        countText.text = "Count: " + count.ToString();

        if(count >= 12 && !gameHasEnded){
            winTextObject.SetActive(true);
            timerIsRunning = false;
            gameHasEnded = true;
        }
    }

    private void Update() {
        if (transform.position.y < -10 && !gameHasEnded){  // Verifica se o jogo terminou antes de processar a queda
            HandlePlayerFall();
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
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("Você perdeu!");

                loseTextObject.SetActive(true);
                gameHasEnded = true;
                UpdateTimerText();  // Garante que o cronômetro mostre 00:00
            }
        }

        // Verifica se o jogador está no chão usando raycasting
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
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
            jump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Aplica uma força de impulso para o pulo
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("PickUp") && !gameHasEnded)
        {
            other.gameObject.SetActive(false);
            count++;

            SetCountText();
        }
    }

    // Lida com a queda do jogador
    void HandlePlayerFall(){
        if (currentLives > 0)  // Garante que não decremente vidas se já estiver em 0
        {
            currentLives--;
            UpdateLivesText();  // Atualiza a UI das vidas

            if (currentLives <= 0)
            {
                // O jogador perdeu após cair 3 vezes
                Debug.Log("Você perdeu todas as vidas!");
                loseTextObject.SetActive(true);
                transform.position = startPosition;
                gameHasEnded = true;  // Termina o jogo
                timerIsRunning = false;  // Para o cronômetro
            }
            else
            {
                // Retorna o jogador para a posição inicial
                transform.position = startPosition;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    // Atualiza o texto das vidas na interface
    void UpdateLivesText(){
        livesText.text = "Vidas: " + currentLives.ToString();
    }

    // Atualiza o texto do cronômetro na interface
    void UpdateTimerText(){
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        minutes = Mathf.Max(0, minutes);
        seconds = Mathf.Max(0, seconds);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
