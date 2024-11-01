using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//if this is here, that means you have not taken the spewer ai off of its dev 
//exception and needs to be. 
using TMPro;
using Microsoft;
using JetBrains;
using OpenCover;

public class SpewerAi : BTBaseEnemy
{

    public bool SearchSequenceActive = false;
    public bool SpeedBoost;
    [SerializeField] private GameObject Dropplet;
    [SerializeField] private GameObject SpewPoint;

    public float PlayerDistance;
    public float MaxAttackDist;



    private Animator EnemyAnimator;
    

    protected override void CustomStartup()
    {
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

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan)
        {
            return;
        }

        HealthBar.value = CurrentHealth;
        if (CurrentHealth <= 0)
        {
            Death();
            Destroy(gameObject);
        }
        HandleForce();

        if (Input.GetKeyDown(KeyCode.J))
        {
            //Attacking = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            //CanAttackPlayer = true;
        }


        CurrentMoveSpeed = NavMeshRef.velocity.magnitude;
        CurrentMoveSpeed = (float)System.Math.Round(CurrentMoveSpeed, 2);
    }


    private void FixedUpdate()
    {
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
        if (!OnAttackingList) { return; }
        Debug.Log("Wishing for\r\nWicked ways");

        LockForAttack();
        EnemyAnimator.SetTrigger("Attack");

    }

    public void SpawnDropplet()
    {
        GameObject SpawnedDropplet = Instantiate(Dropplet, SpewPoint.transform.position, SpewPoint.transform.rotation);
        SpawnedDropplet.GetComponent<ProjectileBase>().LifeStartup(transform.forward, 100);
        CanAttackPlayer = false;
    }

    public void AttackCooldown()
    {
        StartCoroutine(AttackCooldown(2.5f));

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