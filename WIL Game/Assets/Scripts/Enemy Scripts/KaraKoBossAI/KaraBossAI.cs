using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KaraBossAI : BossBase
{

    #region Bools
    public bool PerformingAttack;
    public bool AttackChosen = false;
    private bool AttackActive;

    [HideInInspector] public bool CanMove = true;
    public bool CanPerformAction;
    [HideInInspector] public bool CustomLocationChosen = false;
    public bool CloseRange;
    public bool AllAttacksDown = false;
    private bool AttackResetRefreshed = false;


    #endregion

    #region Floats

    [Space(5)]
    public float PlayerDistance = 0.0f;
    public float StoppingDistance = 20;
    [SerializeField]private float ActionLockoutTime;
    [Space(2)]
    public float CloseRangeDistance;
    public float LongRangeDistance;
    public float AttackWaitTime;
    #endregion

    [Space(5)]
    public Transform ChosenLocation;
    private Transform PositionLock;

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
        CanPerformAction = true;
        if (PlayerRef == null)
        {
            PlayerRef = GameObject.Find("Player");
        }
        NavMeshRef=GetComponent<NavMeshAgent>();
        MaxHealth = 300;
        CurrentHealth = MaxHealth;

        AttackWaitTime = 15f;
        CreateBehaviourTree();
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
        if (AttackWaitTime > 0)
        {
            return;
        }
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
        //AttackChosen = false;

        //PerformingAttack = true;
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

        if (!CanPerformAction && ActionLockoutTime > 0 && PerformingAttack)
        {
            ActionLockoutTime -= Time.deltaTime;
        }
        else if (!CanPerformAction && ActionLockoutTime <= 0 && PerformingAttack)
        {
            ActionLockoutTime = 0;
            CanPerformAction = true;
            PerformingAttack = false;
            AttackChosen = false;
            CanMove = true;
        }

        if (AttackWaitTime > 0)
        {
            AttackWaitTime -= Time.deltaTime;
        }
        if (AttackWaitTime <= 0 && !AttackChosen)
        {
            AttackWaitTime = 0;
            AttackResetRefreshed = true;
            
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
        if(HornAttack.AttackCooldownActive && PerformingAttack && CanPerformAction)
        {
            Debug.Log("Slashed Thy Horns");
        }
        else if (!HornAttack.AttackCooldownActive)
        {
            
            ActionLockoutTime = HornAttack.LockoutTime;
            StartCoroutine(HornAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    private void CoalAttackMethod()
    {
        if (CoalAttack.AttackCooldownActive && PerformingAttack && !AttackActive)
        {
            Debug.Log("Feel the cancer rocks");

            CanMove = false;
            ChosenLocation = transform;
            SetDestination(PositionLock);
            StartCoroutine(CoalAttack.CoalBarrage());
            AttackActive = true;
        }
        else if (!CoalAttack.AttackCooldownActive)
        {
            ActionLockoutTime = CoalAttack.LockoutTime;
            StartCoroutine(CoalAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    private void EarthAttackMethod()
    {
        if(HornAttack.AttackCooldownActive && PerformingAttack && CanPerformAction)
        {
            Debug.Log("HAMMER DOWN MOTHER FU-");
        }
        if (!EarthAttack.AttackCooldownActive)
        {

            ActionLockoutTime = EarthAttack.LockoutTime;
            StartCoroutine(EarthAttack.AttackCooldown());
            PerformingAttack = true;
        }
    }

    public void CheckDistance()
    {
        PlayerDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
        if(PlayerDistance>StoppingDistance && PlayerDistance <= CloseRangeDistance)
        {
            Debug.Log("Check");
            CloseRange = true;
        }
        if (PlayerDistance <= LongRangeDistance && PlayerDistance > CloseRangeDistance) 
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

        [SerializeField] private GameObject CoalObject;
        [SerializeField] private GameObject[] MorterPoints;

        public IEnumerator CoalBarrage()
        {

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < MorterPoints.Length; j++)
                {
                    SpawnCoal(MorterPoints[j].gameObject, j + 0 + i);
                    yield return new WaitForSeconds(0.15f);
                }
                yield return new WaitForSeconds(1.25f);
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
