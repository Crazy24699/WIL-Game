using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//if this is here, that means you have not taken the spewer ai off of its dev 
//exception and needs to be. 
//using TMPro;
//using Microsoft;
//using JetBrains;
//using OpenCover;

public class SpewerAi : BTBaseEnemy
{

    [SerializeField] private GameObject Dropplet;
    [SerializeField] private GameObject SpewPoint;

    protected float PlayerDistance;
    public float MaxAttackDist;

    public bool IsMoving = false;

    public EnemySoundManager EnemySoundManagerScript;

    private Animator EnemyAnimator;
    

    protected override void CustomStartup()
    {
        EnemySoundManagerScript = GetComponent<EnemySoundManager>();
        MaxFollowDistance = 20.25f;
        base.CustomStartup();
        CreateBehaviourTree();
        EnemyAnimator = GetComponent<Animator>();
        if (EnemyAnimator == null)
        {
            EnemyAnimator = transform.GetComponentInChildren<Animator>();
        }
        PatrolActive = true;


        WorldHandlerScript.AllEnemies.Add(this.gameObject);
        MaxHealth = 10;
        CurrentHealth = MaxHealth;
        ImmunityTime = 0.375f;
        
    }

    private void Start()
    {
        if(TutorialOverride)
        {
            MaxHealth = 10;
            CurrentHealth = MaxHealth;
            StartupRan = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //CurrentPosition = transform.position.RoundVector(2);
        //CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);


        if (!StartupRan) { return; }

        if (TutorialOverride )
        {

            HealthBar.value = CurrentHealth;
            if (CurrentHealth <= 0)
            {
                //Death();
                Destroy(gameObject);
            }
        }


        if (TutorialOverride) { return; }
        if (SeenPlayer)
        {
            PatrolActive = false;
        }


        if (!Alive)
        {
            Destroy(this.gameObject);
        }


        HandleForce();


        CurrentMoveSpeed = NavMeshRef.velocity.magnitude;
        CurrentMoveSpeed = (float)System.Math.Round(CurrentMoveSpeed, 2);
    }


    private void FixedUpdate()
    {
        if(TutorialOverride) { return; }

        if (StartupRan && Alive)
        {
            Debug.Log("This Is running");
            RootNode.RunLogicAndState();
            EnemyAnimator.SetFloat("CurrentSpeed", CurrentMoveSpeed);

            PlayerDistance = CurrentPlayerDistance;
            MaxAttackDist = MaxAttackDistance;
        }
    }

    public override void Attack()
    {
        if (WorldHandlerScript.EnemiesAttacking.Count < 2 && !OnAttackingList)
        {
            WorldHandlerScript.EnemiesAttacking.Add(this.gameObject);
            OnAttackingList = true;

        }

        if (!OnAttackingList || !CanAttack) { return; }
        Debug.Log("Wishing for\r\nWicked ways");

        LockForAttack();
        EnemyAnimator.SetTrigger("Attack");

    }

    public void DisableAttacking()
    {
        CanAttack = false;
        CanAttackPlayer = false;
    }

    public IEnumerator StartAttackCooldown()
    {

        yield return new WaitForSeconds(2.5f);
        CanAttack = true;
        CanAttackPlayer = true;
    }

    public void SpawnDropplet()
    {
        GameObject SpawnedDropplet = Instantiate(Dropplet, SpewPoint.transform.position, SpewPoint.transform.rotation);
        SpawnedDropplet.GetComponent<ProjectileBase>().LifeStartup(transform.forward, 100);
        //CanAttackPlayer = false;
    }

    public void CreateBehaviourTree()
    {
        BTPatrol PatrolNode = new BTPatrol(this.gameObject);
        BTPersuePlayer PersuePlayerNode = new BTPersuePlayer(this.gameObject);
        BTAttackSpewer AttackNode = new BTAttackSpewer(this.gameObject);

        BTNodeSequence PatrolSequence = new BTNodeSequence();
        BTNodeSequence PersuePlayerSequence = new BTNodeSequence();
        BTNodeSequence AttackSequence = new BTNodeSequence();

        PatrolSequence.SetSequenceValues(new List<BTNodeBase> { PatrolNode });
        PersuePlayerSequence.SetSequenceValues(new List<BTNodeBase> { PersuePlayerNode });
        AttackSequence.SetSequenceValues(new List<BTNodeBase> { AttackNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase> { PatrolSequence, PersuePlayerSequence, AttackSequence });
    }

}