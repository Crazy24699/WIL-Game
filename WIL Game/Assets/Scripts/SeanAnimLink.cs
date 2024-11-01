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
        CollisionBox.SetActive(true);
    }

    [SerializeField]
    private void DeactivateAttackBox()
    {
        CollisionBox.SetActive(false);
    }

    [SerializeField]
    private void RetreatWaitTime()
    {
        
    }
}
