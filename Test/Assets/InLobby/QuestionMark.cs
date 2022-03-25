using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPanel : MonoBehaviour
{
    bool isActive;
    public void OnClicked() => gameObject.SetActive(isActive = !isActive);
}
