using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTesting : MonoBehaviour
{
    public int CycleNum;
    public int CycleCount;

    public float AttackWaitTime;
    public float ElapsedTime;

    public bool CycleAttacks;

    public PlayerAttacks PlayerAttackScript;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAttackScript = GetComponent<PlayerAttacks>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CycleAttacks = true;
            ElapsedTime = AttackWaitTime;
            CycleCount = CycleNum;
        }
        if (CycleAttacks)
        {
            CycleAllAttacks();
        }
    }

    private void CycleAllAttacks()
    {
        ElapsedTime -=Time.deltaTime;
        if (ElapsedTime <= 0 && CycleCount > 0) 
        {
            int RandomAttack = Random.Range(0, 3);
            switch (RandomAttack)
            {
                case 0:
                    PlayerAttackScript.SlashAttack();
                    break;

                case 1:
                    PlayerAttackScript.BiteAttack(); 
                    break;

                case 2:
                    PlayerAttackScript.TailWhipAttack();
                    break;
            }
            ElapsedTime = AttackWaitTime;
            CycleCount--;
            return;

        }
    }

}
