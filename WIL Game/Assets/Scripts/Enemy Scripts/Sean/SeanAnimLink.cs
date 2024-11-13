using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeanAnimLink : MonoBehaviour
{
    [SerializeField] private SeanAI SeanScriptLink;
    [SerializeField] private GameObject CollisionBox;

    private void Start()
    {
        SeanScriptLink = transform.root.root.GetComponent<SeanAI>();
    }

    [SerializeField]
    private void ActivateImmune()
    {

    }

    [SerializeField]
    private void DeactivateImmune()
    {

    }

    [SerializeField]
    private void ActivateAttackBox()
    {
        Debug.Log("base");
        CollisionBox.SetActive(true);
    }

    [SerializeField]
    private void DeactivateAttackBox()
    {
        Debug.Log("switch");
        CollisionBox.SetActive(false);
    }

    [SerializeField]
    private void RetreatWaitTime()
    {
        
    }
}
