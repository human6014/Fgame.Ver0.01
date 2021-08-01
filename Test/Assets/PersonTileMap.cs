using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTileMap : MonoBehaviour
{
    public void CopyTag()
    {
        Debug.Log("PersonTileMap CopyTag Ω√¿€");

        Transform[] tran = GetComponentsInChildren<Transform>();
        foreach (Transform t in tran)
        {
            t.gameObject.tag = this.tag;
        }

        Debug.Log("PersonTileMap CopyTag ≥°");
    }
}
