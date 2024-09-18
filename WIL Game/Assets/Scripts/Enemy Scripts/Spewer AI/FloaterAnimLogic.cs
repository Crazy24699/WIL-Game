using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterAnimLogic : MonoBehaviour
{
    [SerializeField]private SpewerAi SpewerAIScript;

    private void Start()
    {
        Debug.Log(transform.parent.name);
        SpewerAIScript = transform.parent.transform.parent.GetComponentInChildren<SpewerAi>();
    }

    [SerializeField]
    private void SpewDrop()
    {
        SpewerAIScript.SpawnDropplet();
    }
    

}
