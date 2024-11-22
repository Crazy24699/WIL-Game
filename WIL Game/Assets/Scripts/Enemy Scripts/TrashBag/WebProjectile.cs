using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

public class WebProjectile : ProjectileBase
{

    [SerializeField] private GameObject Webbing;

    private bool IsProjectile = true;
    private bool StartupRan = false;

    //[SerializeField] private MeshFilter StickingWeb_MF;
    //[SerializeField] private MeshFilter CurrentMeshFilter;
    
    //[SerializeField] private Collider WebMeshCollider;
    //[SerializeField] private Collider DroppletCollider;

    [SerializeField] private AudioSource WebAudio;

    private Vector3 StopPosition;

    protected override void CustomBehaviour()
    {
        //CurrentMeshFilter = GetComponent<MeshFilter>();
        //WebMeshCollider = GetComponent<MeshCollider>();
        //DroppletCollider = GetComponent<SphereCollider>();

        StartupRan = true;

        Damage = 3;

        WebAudio.pitch = Random.Range(-0.95f, 1.15f);
        WebAudio.Play();

    } 


    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if(IsProjectile)
            {
                Collision.transform.root.GetComponent<PlayerInteraction>().HandleHealth(-Damage);
                Destroy(this.gameObject);

                return;
            }
            //
        }

        if (Collision.CompareTag("Ground") )
        {
            //StopPosition = transform.position;
            //StopPosition.y += 1.25f;

            //RigidBodyRef.velocity = Vector3.one;
            //RigidBodyRef.isKinematic = true;
            //transform.position = StopPosition;

            //WebMeshCollider.enabled = true;
            //DroppletCollider.enabled = false;
            //CurrentMeshFilter.sharedMesh = StickingWeb_MF.sharedMesh;
            //transform.localScale = Vector3.one * 0.85f;

            //StartCoroutine(GroundLife());
            //IsProjectile = false;


            Destroy(this.gameObject);
        }
    }

    private IEnumerator GroundLife()
    {
        yield return new WaitForSeconds(10.25f);
        Destroy(this.gameObject);
    }

}
