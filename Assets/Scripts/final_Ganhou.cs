using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class final_Ganhou : MonoBehaviour
{
    public TextMeshProUGUI victoryText;  // Referência ao campo de texto para exibir a mensagem

    public void Restart(){
        SceneManager.LoadScene(0);
    }

    void Start(){
        // Recupera o tempo restante salvo
        float timeRemaining = PlayerPrefs.GetFloat("TimeRemaining", 0f);

        // Converte o tempo restante para minutos e segundos
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Exibe o tempo restante na tela de vitória
        victoryText.text = "Você venceu! Tempo restante: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
