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

    private void Start()
    {
        StartCoroutine(BaseStartup());

        StartCoroutine(StartupDelay());

    }

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan)
        {
            return;
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
        

        RootNode.RunLogicAndState();
    }

    private IEnumerator StartupDelay()
    {
        yield return new WaitForSeconds(0.5f);
        CreateBehaviourTree();
    }

    private void FixedUpdate()
    {
        //HandlePatrol();
    }

    public override void Attack()
    {
        //Play animation of attack
        LockForAttack();

        GameObject SpawnedDropplet = Instantiate(Dropplet, SpewPoint.transform.position, SpewPoint.transform.rotation);
        SpawnedDropplet.GetComponent<ProjectileBase>().LifeStartup(transform.forward, 100);
        CanAttackPlayer = false;
        StartCoroutine(AttackCooldown());
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