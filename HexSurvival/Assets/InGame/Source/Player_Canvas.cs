using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Canvas : MonoBehaviour
{
    private Camera mainCamera;
    void Start() => mainCamera = Camera.main;
    void LateUpdate() => transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
}
