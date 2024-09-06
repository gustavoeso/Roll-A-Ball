using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    private Vector2 baseRotation;
    private Vector2 rotation;

    // Start is called before the first frame update
    void Start(){
        offset = transform.position - player.transform.position;
        baseRotation = new Vector2(45, 0);

    }
    void OnLook(InputValue movementValue){
        rotation = movementValue.Get<Vector2>();
        print("OnLook -- rotation: " + rotation);
    }
    // Update is called once per frame
    void LateUpdate(){
        transform.position = player.transform.position + offset;
        float yRotation = rotation.y * 15;
        float xRotation = rotation.x * 25;
        var targetRotation = Quaternion.Euler(baseRotation.x - yRotation,baseRotation.y + xRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
    }
}
