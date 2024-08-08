using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpewerAi : EnemyBase
{

    public bool SearchSequenceActive = false;
    public bool SpeedBoost;
    [SerializeField] private GameObject Dropplet;
    [SerializeField] private GameObject SpewPoint;

    private Animator EnemyAnimator;

    private void Start()
    {
        
        //int RandomWayPoint = Random.Range(0, WorldHandlerScript.AllSpires.Count);
        //WaypointPosition = WorldHandlerScript.AllSpires[RandomWayPoint][RandomWayPoint].ThisSpire.transform;
        StartCoroutine(BaseStartup());

        StartCoroutine(StartupDelay());
        EnemyAnimator = GetComponent<Animator>();
        if(EnemyAnimator == null)
        {
            EnemyAnimator = transform.GetComponentInChildren<Animator>();
        }
        PatrolActive = true;
        
    }
    protected override void CustomStartup()
    {
        WorldHandlerScript.AllEnemies.Add(this);
        MaxHealth = 10;
        
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
            Die();
            Destroy(gameObject);
        }
        HandleForce();

        if (Input.GetKeyDown(KeyCode.J))
        {
            IsAttacking = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            CanAttackPlayer = true;
        }


        CurrentMoveSpeed = NavMeshRef.velocity.magnitude;
        //Debug.Log(CurrentMoveSpeed);
        CurrentMoveSpeed = (float)System.Math.Round(CurrentMoveSpeed, 2);
        //Debug.Log(CurrentMoveSpeed); 
        //PlayAnimation();

        //SeenPlayer = PlayerEscaped;
        
    }


    private void FixedUpdate()
    {
        if (StartupRan && Alive)
        {
            RootNode.RunLogicAndState();
            EnemyAnimator.SetFloat("CurrentSpeed", CurrentMoveSpeed);
        }
    }

    private IEnumerator StartupDelay()
    {
        yield return new WaitForSeconds(0.5f);
        CreateBehaviourTree();
    }

    public override void Attack()
    {
        Debug.Log("Wishing lust for\r\nWicked ways");
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
        StartCoroutine(TempAttackCooldownLock());
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