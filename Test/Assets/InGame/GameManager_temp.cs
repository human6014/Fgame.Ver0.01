using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_temp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
}
