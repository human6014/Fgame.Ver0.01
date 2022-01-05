using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    int iNum, jNum;
    public AllTileMap allTileMap;
    private MeshRenderer meshRenderer;
    private void Start()
    {
        if (name == "Portal") return;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        iNum = int.Parse(transform.name.Substring(0, 1));
        jNum = int.Parse(transform.name.Substring(1));
    }
    public void PlayerEntry(GameObject player)
    {
        if (jNum == 0) player.transform.position = allTileMap.childPortal[iNum, jNum + 1].transform.position;//out->in
        else           player.transform.position = allTileMap.childPortal[iNum, jNum - 1].transform.position;//in->out
    }
}
