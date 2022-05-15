using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    CloudManager cloudManager;
    const float baseSpeed = 0.02f;
    float randSpeed;
    int randomY, randomZ;
    void Start()
    {
        randSpeed = baseSpeed * Random.Range(0.2f, 2);
        cloudManager = GetComponentInParent<CloudManager>();
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * randSpeed);
        if (transform.position.x >= cloudManager.GetFogEndPositionX() + 10)
        {
            randomY = cloudManager.GetRandomY();
            randomZ = cloudManager.GetRandomZ();

            transform.position = new Vector3(cloudManager.GetFogStartPositionX(), randomY, randomZ);

        }
    }
}
