using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Canvas : MonoBehaviour
{
    public Camera main_camera;
    void Start() => main_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    void LateUpdate() => transform.LookAt(transform.position + main_camera.transform.rotation * Vector3.back, main_camera.transform.rotation * Vector3.up);
}
