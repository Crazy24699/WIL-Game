using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolutionShard : MonoBehaviour
{
    private GameObject ShardBodyRef;
    private bool Uncounted = false;
    private void Start()
    {
        ShardBodyRef = transform.GetComponentInChildren<MeshRenderer>().gameObject;
        this.gameObject.name = ShardBodyRef.name + "" + Random.Range(-965, 956);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player") && Collision.gameObject.GetComponent<PlayerInteraction>() != null)
        {
            //Play anim
            //hide the shard body
            //Destroy shard after a moment

            Collision.GetComponent<PlayerInteraction>().HandleShardUpdate();
            Uncounted = true;
            StartCoroutine(ShardInteraction()); 
        }
    }

    private IEnumerator ShardInteraction()
    {

        ShardBodyRef.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.25f);
        Destroy(this.gameObject);
    }

}
