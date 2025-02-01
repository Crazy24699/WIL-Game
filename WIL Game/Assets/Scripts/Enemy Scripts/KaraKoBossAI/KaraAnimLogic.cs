using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaraAnimLogic : MonoBehaviour
{

    [SerializeField] private KaraBossAI KaraLogic;

    [SerializeField] private GameObject SwipeHitbox;
    [SerializeField] private GameObject SlamHitbox;

    private bool SoundOverride = false;

    private void Start()
    {
        KaraLogic = transform.root.GetComponent<KaraBossAI>();
    }

    [SerializeField] private void CoalSpurt()
    {
        KaraLogic.CoalAttack.CoalBurst();
        PlayCoalShotSound();
    }

    [SerializeField] private void StartCoalCooldown()
    {
        StartCoroutine(KaraLogic.CoalAttack.AttackCooldown());
    }

    [SerializeField] private void StartHornCooldown()
    {
        StartCoroutine(KaraLogic.HornAttack.AttackCooldown());

    }

    [SerializeField] private void StartSlamCooldown()
    {
        StartCoroutine(KaraLogic.EarthAttack.AttackCooldown());

    }

    [SerializeField] private void PlayWalkSound()
    {

        if (SoundOverride) { return; }
        KaraLogic.BossSoundManage.PlaySound(BossSoundManager.SoundOptions.Moving);
    }

    [SerializeField]
    private void PlaySwipeSound()
    {
        if (SoundOverride) { return; }
        KaraLogic.BossSoundManage.PlaySound(BossSoundManager.SoundOptions.Attack1);
    }

    [SerializeField] private void PlaySlamSound()
    {
        if (SoundOverride) { return; }
        KaraLogic.BossSoundManage.PlaySound(BossSoundManager.SoundOptions.Attack2);
    }

    [SerializeField] 
    private void PlayCoalShotSound()
    {
        if (SoundOverride) { return; }
        KaraLogic.BossSoundManage.PlaySound(BossSoundManager.SoundOptions.Attack3);
    }

    [SerializeField]
    private void EnableHeadSwipe()
    {
        SwipeHitbox.SetActive(true);
    }

    [SerializeField]
    private void DisableHeadSwipe()
    {
        SwipeHitbox.SetActive(false);
    }

    [SerializeField]
    private void EnableEarthSlam()
    {
        SlamHitbox.SetActive(true);
    }

    [SerializeField]
    private void DisableEarthSlam()
    {
        SlamHitbox.SetActive(false);
    }

}
