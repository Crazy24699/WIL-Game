using System.Collections;
using UnityEngine;

public class BubbleProjectile : ProjectileBase
{

    [SerializeField]private float CurrentLifeTime = 0.0f;
    private bool StartedFadeEffect;

    [SerializeField]private Vector2 InitialBubbleSize;
    [SerializeField] private Vector2 FinalBubbleSize;

    [SerializeField] private Material BubbleMaterialRef;
    private Material BubbleMaterialClone;
    private Color MaterialColour;

    public AudioSource Clip;
    private bool ProjectileActive = false;

    protected override void CustomBehaviour()
    {
        Damage = 1;
        LifeTime = 8;
        CurrentLifeTime = LifeTime;

        BubbleMaterialClone = new Material(BubbleMaterialRef);
        BubbleMaterialClone.name = "BubbleMaterial clone";
        this.GetComponent<MeshRenderer>().material = BubbleMaterialClone;

        InitialBubbleSize = transform.localScale;
        FinalBubbleSize = Vector3.one * 2;

        CustomLifeTimer = true;
        MaterialColour = BubbleMaterialRef.color;

        StartCoroutine(ProjectileActivator());
        Clip.pitch = Random.Range(-0.95f, 1.15f);
        Clip.Play();
    }

    private void Start()
    {
        //CustomBehaviour();
    }

    //there are 2 ways to do this, one via animation and one via code. Just checking when the alpha of the
    //colour is completely gone, that is easier but i like code so im doing it this way
    private void Update()
    {
        if (CurrentLifeTime <= 0)
        {
            CurrentLifeTime = 0;
            return;
        }

        CurrentLifeTime -= Time.deltaTime;

        if (transform.lossyScale.x < FinalBubbleSize.x)
        {
            float SizeIncriment = 1 - CurrentLifeTime / LifeTime;
            float XScaleChange = Mathf.Lerp(InitialBubbleSize.x, FinalBubbleSize.x, SizeIncriment);

            Vector3 BubbleScaleChange = Vector3.one * XScaleChange;
            this.transform.localScale = BubbleScaleChange;
        }

        if (CurrentLifeTime <= LifeTime / 2)
        {
            //How to decriment a lerp value 
            float FadeValue = 1 - (CurrentLifeTime / (LifeTime / 2));
            

            //BubbleMaterialRef.color = MaterialColour;

            MaterialColour.a = Mathf.Lerp(1, 0, FadeValue);
            BubbleMaterialClone.color = MaterialColour;
            this.GetComponent<MeshRenderer>().material = BubbleMaterialClone;
            //Debug.Log("worl turns black     " + BubbleMaterialClone.color.a + "     " + this.GetComponent<MeshRenderer>().material.color.a);


        }

        if (CurrentLifeTime <= 0)
        {
            Destroy(gameObject);
        }


    }


    private void OnTriggerEnter(Collider Collision)
    {
        if (!ProjectileActive)
        {
            return;
        }

        if (Collision.CompareTag("Player"))
        {
            Debug.Log("The player");
            if (Collision.GetComponent<PlayerInteraction>() == null) 
            {
                PlayerInteraction Script=FindObjectOfType<PlayerInteraction>();
                Script.HandleHealth(-1);
                //Collision.transform.GetComponentInChildren<PlayerInteraction>().HandleHealth(-1);
            }
            
        }

        Destroy(gameObject);
    }

    private IEnumerator ProjectileActivator()
    {
        yield return new WaitForSeconds(0.5f);
        ProjectileActive = true;
    }

}
