using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KaraBossAI : BossBase
{

    #region Bools
    public bool PerformingAttack;

    public BossSoundManager BossSoundManage;
    
    
    [HideInInspector] public bool CustomLocationChosen = false;
    public bool CloseRange;
    private bool AttackResetRefreshed = false;

    //New Bools
    public bool CanMove = true;
    public bool CanPerformAction;
    private bool AttackActive;
    public bool BeyondAllAttack;
    public bool BeyondCloseAttack;
    public bool AllAttacksDown = false;
    public bool CanAttack;
    public bool AttackChosen = false;


    #endregion

    #region Floats

    //New floats

    //The general lock out time for all actions
    private float GenActionLockoutTime = 2.5f;

    [Space(5)]
    public float PlayerDistance = 0.0f;
    public float StoppingDistance = 20;
    [SerializeField]private float ActionLockoutTime;
    [SerializeField]private float AttackLockoutTime;
    [Space(2)]
    public float CloseRangeDistance;
    public float LongRangeDistance;

    [SerializeField]private float MaxAttackDistance;

    public float AttackWaitTime;
    #endregion


    [Space(10)]
    public AttackOptions ChosenAttack;
    [Space(10)]

    #region Scripts
    public HornSwipAttackClass HornAttack;
    public CoalBarrangeAttackClass CoalAttack;
    public EarthShakerAttackClass EarthAttack;



    private Animator KaraAnimations;

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
        StartCoroutine(AttackLockoutDelay(5));
    }

    public override void BossStartup()
    {
        CanPerformAction = false;
        if (PlayerRef == null)
        {
            PlayerRef = GameObject.Find("Player");
        }
        NavMeshRef=GetComponent<NavMeshAgent>();
        MaxHealth = 30;
        CurrentHealth = MaxHealth;

        AttackChosen = false;
        PerformingAttack = false;
        CanAttack = true;

        PlayerRef = FindObjectOfType<PlayerInteraction>().gameObject;

        if (BossSoundManage == null)
        {
            BossSoundManage = this.GetComponent<BossSoundManager>();
        }

        Alive = true;
        ActionLockoutTime = 5;
        AttackWaitTime = 15f;
        KaraAnimations = transform.GetComponentInChildren<Animator>();
        CreateBehaviourTree();
        HealtbarStartup();
        CanMove = true;

        MaxAttackDistance = CoalAttack.AttackDistance;
        StartupRan = true;
    }

    private void CreateBehaviourTree()
    {
        BTKaraAttack AttackNode = new BTKaraAttack(this.gameObject);
        BTKaraChoice ChoiceNode = new BTKaraChoice(this.gameObject);
        BTKaraMove MoveNode = new BTKaraMove(gameObject);

        BTNodeSequence AttackSequence = new BTNodeSequence();
        BTNodeSequence MoveSequence = new BTNodeSequence();
        BTNodeSequence ChoiceSequence = new BTNodeSequence();

        AttackSequence.SetSequenceValues(new List<BTNodeBase> { AttackNode });
        MoveSequence.SetSequenceValues(new List<BTNodeBase> { MoveNode });
        ChoiceSequence.SetSequenceValues(new List<BTNodeBase> { ChoiceNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase>() { MoveSequence, AttackSequence , ChoiceSequence });

    }


    public override void HandleHealth(int HealthChange)
    {
        base.HandleHealth(HealthChange);
        BossSoundManage.PlaySound(BossSoundManager.SoundOptions.TakeDamage);
    }

    protected override void Die(int HealthCheck)
    {
        base.Die(HealthCheck);
        BossSoundManage.PlaySound(BossSoundManager.SoundOptions.Death);
    }


    private void FixedUpdate()
    {
        if (!StartupRan)
        {
            return;
        }

        if (Alive == false)
        {
            return;
        }

        if (AllAttacksDown)
        {
            CanPerformAction = false;
        }

        if (CurrentHealth <= 0)
        {
            Alive = false;
            Destroy(gameObject);
        }

        CheckDistance();
        //AttackChecker();
        RotateToTarget();
        //New code

        CanPerformAction = PlayerDistance <= MaxAttackDistance ? true : false;
        if (!CanPerformAction)
        {
            if (CanMove)
            {
                SetDestination(PlayerRef.transform);
                CanMove = false;
            }
        }

        

        if (CanPerformAction)
        {
            CanMove = true;
            RootNode.RunLogicAndState();
        }

        BeyondAllAttack = PlayerDistance > MaxAttackDistance;
    }

    #region New Code redone

    private void CoalAttackLogic()
    {
        AttackActive = CoalAttack.BarrageShots < CoalAttack.ShotsFired ? true : false;
    }

    public void FireCoal()
    {

        CoalAttackLogic();
        Debug.Log("fuck");
        KaraAnimations.SetTrigger("CoalAttack");

    }

    

    public void SlashHorn()
    {
        KaraAnimations.SetTrigger("HornAttack");
    }

    public void ShatterGround()
    {
        KaraAnimations.SetTrigger("EarthAttack");
    }

    public void AttackPerformanceSet(bool SetPerform, AttackOptions PerformingAttack)
    {
        switch (PerformingAttack)
        {
            case AttackOptions.HornSwipe:
                HornAttack.Performing = SetPerform;
                break;

            case AttackOptions.CoalBarrage:
                CoalAttack.Performing = SetPerform;
                break;

            case AttackOptions.EarthShaker:
                EarthAttack.Performing = SetPerform;
                break;

        }
    }

    private IEnumerator AttackLockoutDelay(float LockoutDelayTime)
    {
        CanAttack = false;
        yield return new WaitForSeconds(LockoutDelayTime);
        Debug.Log("Lockout done;");
        CanAttack = true;
        AttackChosen = false;
    }

    public void ActionLockoutLogic(bool LockoutAttack)
    {
        float LockoutTimeChosen = 0.0f;
        switch (LockoutAttack)
        {
            case true:
                LockoutTimeChosen = AttackLockoutTime;
                break;

            case false:
                LockoutTimeChosen = GenActionLockoutTime;
                break;
        }
        StartCoroutine(LockoutTimeLogic(LockoutTimeChosen,LockoutAttack));
        //Debug.Log("didnt see the grave");
    }

    private IEnumerator LockoutTimeLogic(float WaitTime, bool AttackLockout)
    {
        CanPerformAction = false;
        yield return new WaitForSeconds(GenActionLockoutTime);
        CanPerformAction = true;

        if (NavMeshRef.isStopped)
        {
            NavMeshRef.isStopped = false;
        }
    }


    public void RunChosenAttack()
    {
        if (!CanAttack)
        {
            return;
        }
        NavMeshRef.isStopped = true;

        CanMove = false;
        switch (ChosenAttack)
        {
            case AttackOptions.HornSwipe:
                AttackPerformanceSet(true, AttackOptions.HornSwipe);
                Debug.Log("Horn");
                ActionLockoutTime = HornAttack.LockoutTime;
                SlashHorn();
                break;

            case AttackOptions.CoalBarrage:
                AttackPerformanceSet(true, AttackOptions.CoalBarrage);
                Debug.Log("Coal");
                ActionLockoutTime = CoalAttack.LockoutTime;
                FireCoal();
                break;

            case AttackOptions.EarthShaker:
                AttackPerformanceSet(true, AttackOptions.EarthShaker);
                Debug.Log("Earth");
                ActionLockoutTime = EarthAttack.LockoutTime;
                ShatterGround();
                break;

        }
        ActionLockoutLogic(true);
        StartCoroutine(AttackLockoutDelay(ActionLockoutTime));

        ActionLockoutLogic(false);
        CanAttack = false;
        AttackPerformanceSet(false, ChosenAttack);
        //AttackChosen = false;

        //PerformingAttack = true;
    }
    #endregion



   
    public void SetDestination(Transform ObjectLocation)
    {

        if (!CanMove)
        {
            NavMeshRef.SetDestination(transform.position);
            return;
        }
        NavMeshRef.SetDestination(ObjectLocation.transform.position);
    }



    public void CheckDistance()
    {
        PlayerDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
    }


    #region Boss Attacks
    [System.Serializable]
    public class HornSwipAttackClass
    {
        public float AttackTimerDuration;

        public float AttackDistance;
        public float AttackCooldownTime;

        //How long the Action will last before Kara can perform another action
        public float LockoutTime;

        public bool AttackCooldownActive;
        public bool Performing = false;


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
        public float AttackTimerDuration;
        public float AttackDistance;
        public float AttackCooldownTime;
        public float LockoutTime;

        //How many times kara will fire a barrage of coal
        public int BarrageShots = 3;
        public int ShotsFired = 0;

        public bool AttackCooldownActive;
        public bool Performing = false;

        [SerializeField] private GameObject CoalObject;
        [SerializeField] private GameObject[] MorterPoints;

        public void CoalBurst()
        {
            for (int i = 0; i < MorterPoints.Length; i++)
            {
                SpawnCoal(MorterPoints[i].gameObject, i + 0 + i);
            }
        }

        private void SpawnCoal(GameObject FirePoint, int SpawnNumer)
        {
            GameObject SpawnedObject = Instantiate(CoalObject, FirePoint.transform.position, FirePoint.transform.rotation);
            SpawnedObject.GetComponent<ProjectileBase>().LifeStartup(FirePoint.transform.forward, 1500*6);
            SpawnedObject.name = "CoalShot" + SpawnNumer;
            
        }

        public IEnumerator AttackCooldown()
        {

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
        public bool Performing = false;

        public IEnumerator AttackCooldown()
        {
            AttackCooldownActive = true;
            yield return new WaitForSeconds(AttackCooldownTime);
            AttackCooldownActive = false;
        }
    }

    #endregion

}
