using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // Importa a biblioteca de UI
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;
    public float jumpForce = 5f;
    private bool isGrounded;
    [SerializeField] private float rayDistance = 0.6f;  // Aumenta o valor do raycast para garantir que detecte o chão
    public LayerMask groundLayer;  // Camada do chão para detectar o solo com o raycast
    private int count;
    private bool jump = false;
    private Vector3 startPosition;
    private int squares = 27; // Número total de peças

    // Variáveis para o cronômetro
    public float timeLimit = 180.0f;  // 3 minutos (180 segundos)
    private float timeRemaining;
    private bool timerIsRunning = true;
    private bool gameHasEnded = false;  // Flag para verificar se o jogo terminou

    // Barra de Tempo
    public Slider timeBar;  // Referência à barra de tempo (UI)

    // Barra de Progresso de Coleta
    public Slider progressBar;  // Referência à barra de progresso (UI)

    // Variáveis para o sistema de vidas
    public int maxLives = 5;  // Agora o jogador tem 5 vidas
    private int currentLives;
    public GameObject[] heartImages;  // Array para armazenar as imagens de coração
    // Invulnerabilidade após colisão
    private bool isInvulnerable = false;  // Controle de invulnerabilidade temporária
    public float invulnerabilityDuration = 1.0f;  // Tempo de invulnerabilidade após a colisão

    // Som de Morte
    public AudioSource deathSound;  // Referência ao som de morte

    // Som de Coleta de Moeda
    public AudioSource coinSound;  // Referência ao som de coleta

    void Start(){
        rb = GetComponent<Rigidbody>();
        startPosition = new Vector3(0, 0, 0); // Define a posição inicial (ponto 0,0,0)
        count = 0;

        // Inicializando o cronômetro
        timeRemaining = timeLimit;
        if (timeBar != null)
        {
            timeBar.maxValue = timeLimit;  // Define o valor máximo da barra de tempo
            timeBar.value = timeLimit;  // Inicia a barra cheia
        }

        // Inicializando a barra de progresso de coleta
        if (progressBar != null)
        {
            progressBar.maxValue = squares;  // Define o valor máximo da barra como o número total de peças
            progressBar.value = 0;  // Inicia a barra vazia
        }

        // Inicializando o sistema de vidas
        currentLives = maxLives;
        UpdateHeartsUI();  // Atualiza o estado das imagens de coração
    }

    void OnMove(InputValue movementValue){
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void CheckWinCondition(){
        // Verifica se o jogador coletou todas as peças
        if(count >= squares && !gameHasEnded && timeRemaining > 0){
            timerIsRunning = false;
            gameHasEnded = true;

            // Armazena o tempo restante usando PlayerPrefs
            PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
            PlayerPrefs.Save();

            SceneManager.LoadScene(3);  // Carrega a cena de vitória
        }

        // Atualiza a barra de progresso de coleta
        if (progressBar != null)
        {
            progressBar.value = count;  // Atualiza a barra com o número de itens coletados
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
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("Você perdeu!");

                gameHasEnded = true;
                SceneManager.LoadScene(2); // Carrega a cena de derrota
            }
        }

        // Verifica se o jogador está no chão usando raycasting
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);

        // Para ver visualmente o raycast
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.red);
    }

    // Atualiza a barra de tempo
    void UpdateTimerUI()
    {
        if (timeBar != null)
        {
            timeBar.value = timeRemaining;  // Atualiza o valor da barra com base no tempo restante
        }
    }

    void OnJump(){
        if (isGrounded)
        {
            jump = true;
        }
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

            // Toca o som de coleta
            if (coinSound != null)
            {
                coinSound.Play();
            }

            CheckWinCondition();
        }
    }

    // Lida com a queda do jogador ou colisão com o cubo vermelho
    public void HandlePlayerFall(){
        if (!isInvulnerable)  // Verifica se o jogador não está invulnerável
        {
            if (currentLives > 0)
            {
                currentLives--;
                UpdateHeartsUI();  // Atualiza as imagens de coração

                // Toca o som de morte ao perder uma vida
                if (deathSound != null)
                {
                    deathSound.Play();
                }

                if (currentLives <= 0)
                {
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

    // Atualiza as imagens de coração
    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].SetActive(true);  // Ativa a imagem se o jogador ainda tiver essa vida
            }
            else
            {
                heartImages[i].SetActive(false);  // Desativa a imagem se o jogador perdeu essa vida
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
}
