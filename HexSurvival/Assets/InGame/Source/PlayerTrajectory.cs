using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrajectory : MonoBehaviour
{
    private const int maxCount = 18;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] new Transform transform;
    
    public void RenderOn() => lineRenderer.enabled = true;
    
    public void RenderOff() => lineRenderer.enabled = false;

    public void DrawTrajectory(float chargingTime)
    {
        RenderOn();
        int index = 0;
        float power = chargingTime * 1.25f + 0.5f;
        float maxDist = maxCount * Time.deltaTime;
        Vector3 velocityVector = transform.forward * power + Vector3.up * (power / 2);
        lineRenderer.positionCount = maxCount;

        Vector3 currentPosition = transform.position + transform.forward / 4 + Vector3.up / 3;
        for (float t = 0.0f; t < maxDist; t += Time.deltaTime)
        {
            lineRenderer.SetPosition(index, currentPosition);
            currentPosition += velocityVector * Time.deltaTime;
            velocityVector += Physics.gravity * Time.deltaTime;
            index++;
        }
    }
}
