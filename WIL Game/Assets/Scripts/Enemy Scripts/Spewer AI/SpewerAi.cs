using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpewerAi : EnemyBase
{

    public bool SearchSequenceActive = false;
    public bool SpeedBoost;


    private void Start()
    {
        StartCoroutine(BaseStartup());

        CreateBehaviourTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan)
        {
            return;
        }
        HandleForce();

        RootNode.RunLogicAndState();
    }

    private void FixedUpdate()
    {
        //HandlePatrol();
    }

    public void CreateBehaviourTree()
    {
        BTPatrol PatrolNode = new BTPatrol(this.gameObject);
        BTPersuePlayer PersuePlayerNode = new BTPersuePlayer(this.gameObject);

        BTNodeSequence PatrolSequence = new BTNodeSequence();
        BTNodeSequence PersuePlayerSequence = new BTNodeSequence();

        PatrolSequence.SetSequenceValues(new List<BTNodeBase> { PatrolNode });
        PersuePlayerSequence.SetSequenceValues(new List<BTNodeBase> { PersuePlayerNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase> { PatrolSequence, PersuePlayerSequence });

    }



}