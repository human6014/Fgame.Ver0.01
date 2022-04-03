using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class MainCamera : MonoBehaviour
{
    Transform[] players;
    Transform target;
    Vector3 offset = new Vector3(0,4,-2);
    
    private bool isEnd;
    KeyCode[] keyCodes = {
        KeyCode.RightArrow,
        KeyCode.LeftArrow
    };
    public void SetTarget(Transform _target) => target = _target;
    private void Update()
    {
        if (!target) return;
        if (Input.GetKeyDown(keyCodes[0]))
        {
            
        }
        else if (Input.GetKeyDown(keyCodes[1]))
        {

        }
        transform.position = target.position + offset;
    }
}