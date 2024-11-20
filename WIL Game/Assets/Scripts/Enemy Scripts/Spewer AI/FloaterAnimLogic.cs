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
        SpewerAIScript.IsMoving = false;
        SpewerAIScript.SpawnDropplet();
        PlaySpitSound();
    }

    [SerializeField]
    private void StopMoving()
    {
        SpewerAIScript.IsMoving = false;
    }

    private void DisableAttack()
    {
        SpewerAIScript.CanAttackPlayer = false;
    }

    private void StartAttackCooldown()
    {
        StartCoroutine(SpewerAIScript.StartAttackCooldown());
    }

    [SerializeField]
    private void PlaySpitSound()
    {
        SpewerAIScript.EnemySoundManagerScript.PlaySound(EnemySoundManager.SoundOptions.Attack);
        SpewerAIScript.IsMoving = false;
    }

    [SerializeField]
    private void PlayMoveSound()
    {
        if(SpewerAIScript.IsMoving) { return; }
        SpewerAIScript.IsMoving = true;
        Debug.Log("Play");
        SpewerAIScript.EnemySoundManagerScript.PlaySound(EnemySoundManager.SoundOptions.Moving);
    }

}
