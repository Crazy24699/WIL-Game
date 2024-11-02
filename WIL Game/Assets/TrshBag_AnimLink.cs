using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrshBag_AnimLink : MonoBehaviour
{
    private TrashBag TrashBagScript;
    private void Start()
    {
        TrashBagScript = transform.root.root.GetComponent<TrashBag>();
        Debug.Log(TrashBagScript.NavMeshRef.name);
    }
    [SerializeField]
    private void Attack()
    {
        TrashBagScript.FireProjectile();
    }
}
