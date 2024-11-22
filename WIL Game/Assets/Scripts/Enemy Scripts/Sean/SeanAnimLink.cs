using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeanAnimLink : MonoBehaviour
{
    [SerializeField] private SeanAI SeanScriptLink;
    [SerializeField] private GameObject CollisionBox;

    //private void Start()
    //{
    //    //SeanScriptLink = transform.root.root.GetComponent<SeanAI>();
    //}

    public void SetLink(SeanAI SeanLink)
    {
        SeanScriptLink = SeanLink;
    }

    [SerializeField]
    private void ActivateAttackBox()
    {
        Debug.Log("base");
        CollisionBox.SetActive(true);
    }


    [SerializeField]
    private void AttackTrue()
    {
        SeanScriptLink.IsAttacking = true;
    }
    [SerializeField]
    private void AttackFalse()
    {
        SeanScriptLink.IsAttacking = false;
    }



    [SerializeField]
    private void PlayImpact()
    {
        SeanScriptLink.ImpactSound();
    }

    [SerializeField]
    private void TriggerRetreat()
    {
        SeanScriptLink.RetreatWaitTime();
    }

    [SerializeField]
    private void DeactivateAttackBox()
    {
        Debug.Log("switch");
        CollisionBox.SetActive(false);
    }
}
