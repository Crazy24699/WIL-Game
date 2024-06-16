using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KaraBossAI : BossBase
{

    #region Bools
    [HideInInspector] public bool PerformingAttack;
    [HideInInspector] public bool CanMove = true;
    public bool ChosenDestination = false;
    public bool AttackChosen = false;
    public bool CanPerformAction;
    [HideInInspector] public bool CustomLocationChosen = false;
    [HideInInspector] public bool CloseRange;
    [HideInInspector] public bool OutOfRange;
    public bool AllAttacksDown = false;
    private bool StartupRan;
    private bool AttackResetRefreshed = false;
    #endregion

    #region Floats

    [Space(5)]
    public float PlayerDistance = 0.0f;
    [HideInInspector]public float StoppingDistance = 25;
    private float ActionLockoutTime;
    [Space(2)]
    public float CloseRangeDistance;
    public float LongRangeDistance;
    public float AttackWaitTime;
    #endregion

    [Space(5)]
    public Transform ChosenLocation;

    [Space(10)]
    public AttackOptions ChosenAttack;
    [Space(10)]

    #region Scripts
    public HornSwipAttackClass HornAttack;
    public CoalBarrangeAttackClass CoalAttack;
    public EarthShakerAttackClass EarthAttack;

    public NavMeshAgent NavMeshRef;

    #endregion
    public enum AttackOptions
    {
        HornSwipe,
        CoalBarrage,
        EarthShaker
    };

    private void Start()
    {
        BossStartup();
    }

    public override void BossStartup()
    {
        if (PlayerRef == null)
        {
            PlayerRef = GameObject.Find("Player");
        }
        NavMeshRef=GetComponent<NavMeshAgent>();
        MaxHealth = 300;
        CurrentHealth = MaxHealth;

        AttackWaitTime = 15f;
        StartupRan = true;
        CreateBehaviourTree();
    }

    private void CreateBehaviourTree()
    {
        //BTKaraAttack AttackNode = new BTKaraAttack(this.gameObject);
        BTKaraChoice ChoiceNode = new BTKaraChoice(this.gameObject);
        BTKaraMove MoveNode = new BTKaraMove(gameObject);

        BTNodeSequence AttackSequence = new BTNodeSequence();
        BTNodeSequence MoveSequence = new BTNodeSequence();
        BTNodeSequence ChoiceSequence = new BTNodeSequence();

        AttackSequence.SetSequenceValues(new List<BTNodeBase> { MoveNode });
        MoveSequence.SetSequenceValues(new List<BTNodeBase> { MoveNode });
        ChoiceSequence.SetSequenceValues(new List<BTNodeBase> { ChoiceNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase>() { AttackSequence, MoveSequence, ChoiceSequence });

    }

    private void FixedUpdate()
    {
        //if (!StartupRan)
        //{
        //    return;
        //}

        CheckDistance();
        AttackChecker();
        RootNode.RunLogicAndState();

    }

    private void Update()
    {
        //if (!StartupRan)
        //{
        //    return;
        //}

        ActiveActionCooldown();
    }

    public void RunChosenAttack()
    {
        CanMove = false;
        switch (ChosenAttack)
        {
            case AttackOptions.HornSwipe:
                HornAttackMethod();
                break;

            case AttackOptions.CoalBarrage:
                CoalAttackMethod();
                break;

            case AttackOptions.EarthShaker:
                EarthAttackMethod();
                break;

        }
        CanPerformAction = false;
        AttackChosen = true;

        PerformingAttack = true;
    }

    public void ResetAttackLockout(float AttackLockoutTime)
    {
        if (!AttackResetRefreshed)
        {
            return;
        }
        AttackWaitTime = AttackLockoutTime;
    }

    public void ActiveActionCooldown()
    {

        if (!CanPerformAction && ActionLockoutTime > 0)
        {
            ActionLockoutTime -= Time.deltaTime;
        }
        else if (!CanPerformAction && ActionLockoutTime <= 0)
        {
            ActionLockoutTime = 0;
            CanPerformAction = true;
        }

        if (AttackWaitTime > 0)
        {
            AttackWaitTime -= Time.deltaTime;
        }
        if (AttackWaitTime <= 0 && CanMove == true && !AttackChosen)
        {
            AttackWaitTime = 0;
            AttackResetRefreshed = true;
            CanMove = false;
        }

    }

    public void AttackChecker()
    {
        if (HornAttack.AttackCooldownActive && CoalAttack.AttackCooldownActive && EarthAttack.AttackCooldownActive)
        {
            AllAttacksDown = true;
            return;
        }

        AllAttacksDown = false;
    }

    public void SetDestination(Transform ObjectLocation)
    {

        if (!CanMove)
        {
            NavMeshRef.SetDestination(transform.position);
            return;
        }
        NavMeshRef.SetDestination(ObjectLocation.transform.position);
    }

    public bool CheckAttackRange(AttackOptions CheckingAttack)
    {
        bool InRange = false;
        float AttackDistanceRef = 0.0f;

        switch (CheckingAttack)
        {
            case AttackOptions.HornSwipe:
                AttackDistanceRef = HornAttack.AttackDistance;
                ActionLockoutTime = HornAttack.LockoutTime;
                break;

            case AttackOptions.CoalBarrage:
                AttackDistanceRef = CoalAttack.AttackDistance;
                ActionLockoutTime = CoalAttack.LockoutTime;
                break;

            case AttackOptions.EarthShaker:
                AttackDistanceRef = EarthAttack.AttackDistance;
                ActionLockoutTime = EarthAttack.LockoutTime;
                break;
                
        }

        InRange = (AttackDistanceRef <= PlayerDistance) ? true : false;
        return InRange;
    }

    public void HornAttackMethod()
    {

        if (!HornAttack.AttackCooldownActive)
        {
            Debug.Log("Slashed Thy Horns");
            StartCoroutine(HornAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    private void CoalAttackMethod()
    {
        if (!CoalAttack.AttackCooldownActive)
        {
            Debug.Log("Feel the cancer rocks");
            StartCoroutine(CoalAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    private void EarthAttackMethod()
    {
        if (!EarthAttack.AttackCooldownActive)
        {
            Debug.Log("HAMMER DOWN MOTHER FU-");
            StartCoroutine(EarthAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    public void CheckDistance()
    {
        PlayerDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
        if(PlayerDistance>StoppingDistance && PlayerDistance <= CloseRangeDistance)
        {
            CloseRange = true;
        }
        if(PlayerDistance<=LongRangeDistance && PlayerDistance > CloseRangeDistance)
        {
            CloseRange = false;
        }
    }


    #region Boss Attacks
    [System.Serializable]
    public class HornSwipAttackClass
    {
        public float AttackDistance;
        public float AttackCooldownTime;

        //How long the Action will last before Kara can perform another action
        public float LockoutTime;

        public bool AttackCooldownActive;


        public IEnumerator AttackCooldown()
        {
            AttackCooldownActive = true;
            yield return new WaitForSeconds(AttackCooldownTime);
            AttackCooldownActive = false;
        }
    }

    [System.Serializable]
    public class CoalBarrangeAttackClass
    {
        public float AttackDistance;
        public float AttackCooldownTime;
        public float LockoutTime;

        public bool AttackCooldownActive;


        public IEnumerator AttackCooldown()
        {
            AttackCooldownActive = true;
            yield return new WaitForSeconds(AttackCooldownTime);
            AttackCooldownActive = false;
        }
    }

    [System.Serializable]
    public class EarthShakerAttackClass
    {
        public float AttackDistance;
        public float AttackCooldownTime;
        public float LockoutTime;

        public bool AttackCooldownActive;


        public IEnumerator AttackCooldown()
        {
            AttackCooldownActive = true;
            yield return new WaitForSeconds(AttackCooldownTime);
            AttackCooldownActive = false;
        }
    }

    #endregion

}
