using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;

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
    private int squares = 25;

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

    // Invulnerabilidade após colisão
    private bool isInvulnerable = false;  // Controle de invulnerabilidade temporária
    public float invulnerabilityDuration = 1.0f;  // Tempo de invulnerabilidade após a colisão

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

        if(count >= squares && !gameHasEnded && timeRemaining > 0){
            winTextObject.SetActive(true);
            timerIsRunning = false;
            gameHasEnded = true;

            // Armazena o tempo restante usando PlayerPrefs
            PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
            PlayerPrefs.Save();

            SceneManager.LoadScene(3);  // Carrega a cena de vitória
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
                SceneManager.LoadScene(2); // Carrega a cena de derrota
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

    // Lida com a queda do jogador ou colisão com o cubo vermelho
    public void HandlePlayerFall(){
        if (!isInvulnerable)  // Verifica se o jogador não está invulnerável
        {
            if (currentLives > 0)
            {
                currentLives--;
                UpdateLivesText();  // Atualiza a UI das vidas

                if (currentLives <= 0)
                {
                    loseTextObject.SetActive(true);
                    transform.position = startPosition;
                    gameHasEnded = true;  // Termina o jogo
                    timerIsRunning = false;  // Para o cronômetro

                    // Carrega a cena de derrota
                    SceneManager.LoadScene(2);  // Certifique-se de que a cena de derrota está no índice 2 do Build Settings
                }
                else
                {
                    // Retorna o jogador para a posição inicial
                    transform.position = startPosition;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // Iniciar período de invulnerabilidade
                StartCoroutine(InvulnerabilityCoroutine());
            }
        }
    }

    // Coroutine para controlar o período de invulnerabilidade
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;  // Define o jogador como invulnerável
        yield return new WaitForSeconds(invulnerabilityDuration);  // Aguarda o tempo de invulnerabilidade
        isInvulnerable = false;  // Jogador volta a ser vulnerável
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
