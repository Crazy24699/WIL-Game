using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaraAnimLogic : MonoBehaviour
{

    [SerializeField] private KaraBossAI KaraLogic;

    private void Start()
    {
        KaraLogic = transform.root.GetComponent<KaraBossAI>();
    }

    [SerializeField] private void CoalSpurt()
    {
        KaraLogic.CoalAttack.CoalBurst();
    }

    [SerializeField]private void StartCoalCooldown()
    {
        StartCoroutine(KaraLogic.CoalAttack.AttackCooldown());
    }

    [SerializeField]private void StartHornCooldown()
    {
        StartCoroutine(KaraLogic.HornAttack.AttackCooldown());

    }

    [SerializeField]private void StartSlamCooldown()
    {
        StartCoroutine(KaraLogic.EarthAttack.AttackCooldown());

    }


}
