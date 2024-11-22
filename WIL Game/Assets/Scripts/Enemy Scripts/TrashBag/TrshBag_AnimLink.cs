using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrshBag_AnimLink : MonoBehaviour
{
    public TrashBag TrashBagScript;
    private void Start()
    {
        TrashBagScript = transform.root.root.GetComponent<TrashBag>();
        //Debug.Log(TrashBagScript.NavMeshRef.name);
    }


    [SerializeField]
    private void Attack()
    {
        TrashBagScript.FireProjectile();
    }

    [SerializeField] private void UpdateAnimState()
    {
        TrashBagScript.AttackAnimPlaying = false;
    }

    [SerializeField]
    private void StartImmuneTime()
    {

    }

    private void EndImmuneTime()
    {

    }

}
