using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private bool isEnd;
    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6
    };
    private void Update()
    {
        if (!target) isEnd = true;
        /*
        if (isEnd)
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    target = GameObject.Find("Player"+ i).transform;//target
                    break;
                }
            }
        }
        */
        if (!target) return;
        transform.position = target.position + offset;
    }
}