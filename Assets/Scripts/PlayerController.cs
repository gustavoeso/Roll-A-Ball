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

    void Start(){
        rb = GetComponent <Rigidbody>();
        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue){
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText(){
        countText.text = "Count: "  + count.ToString();

        if(count >= 12){
            winTextObject.SetActive(true);
        }
    }

    private void FixedUpdate(){
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        // Detecta o pulo ao pressionar a tecla de espaço
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Aplica uma força de impulso para o pulo
        }
    }

    void OnCollisionStay(){
        // Verifica se a bola está no chão
        isGrounded = true;
    }

    void OnCollisionExit()
    {
        // Quando a bola sai do chão, ela não pode mais pular
        isGrounded = false;
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("PickUp")){
            other.gameObject.SetActive(false);
            count++;

            SetCountText();
            
        }
    }
}
