using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody RigidbodyRef;
    public InputActionReference PlayerActionMap;
    private PlayerAttacks PlayerAttackScript;
    [SerializeField] private PlayerInput PlayerInputRef;
    [SerializeField] private PlayerInput DashInput;
    [SerializeField] private PlayerInteraction PlayerInteractScript;
    private WorldHandler WorldHandlerScript;

    public LayerMask GroundLayers;
    private RaycastHit SlopeHit;
    [SerializeField]private AudioSource PlayerAudioSource;


    [SerializeField]private string CurrentClipName;

    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;
    public float Speed;
    public float DetectionRadius=0.35f;
    private float WalkSpeed;
    private float RunSpeed;

    protected float TurnSmoothingVel;
    public float TurnTime = 0.1f;

    private float MovingDelayTimer = 0.15f;
    [SerializeField] private float CurrentMoveDelayTime;

    private float DashSetTime = 0.93f;
    [SerializeField] private float DashDelayTimer;
    [SerializeField] private float DashDistance=20;
    private float MaxSlopeAngle = 35f;

    public Animator PlayerAnimations;

    public Vector3 PlayerVelocity;
    [SerializeField] protected Vector3 MoveDirection;
    [SerializeField] protected Vector3 InputDirection;
    [Space(2)]
    [SerializeField] protected Vector2 DashDirection;
    [SerializeField] protected Vector3 NewDashPosition;
    [SerializeField] protected Vector3 PreviousPosition;
    public Vector3 Gravity;
    [Space(2)]


    public Transform PlayerOrientation;
    public Transform BaltoOrientation;
    public Transform BaltoRef;
    public Transform MainCamera;
    [SerializeField] protected Transform Front_GroundCheckTransform;
    [SerializeField] protected Transform Back_GroundCheckTransform;

    public GameObject HitParticle;
    

    public bool Attacking = false;
    public bool AttackLocked = false;
    public bool CanMove = false;
    [SerializeField]private bool DashSet = false;
    [SerializeField]private bool PlayerDashing = false;
    private bool CanDash = false;
    [SerializeField]public bool Grounded;
    [SerializeField] private bool IsMoving;
    public bool StunLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        StunLocked = false;
        CanMove = true;
        RigidbodyRef = GetComponent<Rigidbody>();
        CurrentSpeed = 0;

        PlayerInputRef = new PlayerInput();
        PlayerInputRef.Enable();
        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed += Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled += Context => Sprint(1f);
        SpeedMultiplier = 1f;

        PlayerInputRef.BasePlayerMovement.DashReading.performed += Context => HandleDashDirection();
        PlayerInputRef.BasePlayerMovement.DashMovement.performed += Context => StartDash();

        PlayerAttackScript = GetComponentInChildren<PlayerAttacks>();
        PlayerInteractScript = gameObject.GetComponent<PlayerInteraction>();

        CurrentMoveDelayTime = MovingDelayTimer;
        WalkSpeed = BaseMoveSpeed * 1;
        RunSpeed = BaseMoveSpeed * 2.5f;
        PlayerAudioSource = this.GetComponent<AudioSource>();
        WorldHandlerScript = FindObjectOfType<WorldHandler>();
    }

    private void FixedUpdate()
    {
        if (StunLocked) { return; }

        //if (Input.GetKey(KeyCode.L))
        //{
        //    Instantiate(HitParticle, transform.position, Quaternion.identity);
        //}

        TrackPlayerMovement();
        if (PlayerAnimations.GetBool("IsAttacking"))
        {
            RigidbodyRef.velocity = Vector3.zero;
            //Debug.Log("laced with poison");
            return;
        }

        //Keep this in the fixed update method because if it isnt then it
        //causes the player to jitter and stutter

        Speed = BaseMoveSpeed * SpeedMultiplier;

        MovePlayer();
        AlignToSurface();
    }

    private void Update()
    {
        if (StunLocked) { return; }
        DashResetTimer();


        TrackBatloOrientation();
        ReduceDashVelocity();


        Vector3 MessuredVelocity = new Vector3(PlayerVelocity.x, 0, PlayerVelocity.z);
        if (MessuredVelocity.magnitude > Speed && !PlayerDashing)
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * Speed;
            RigidbodyRef.velocity = new Vector3(VelocityCap.x, RigidbodyRef.velocity.y
                , VelocityCap.z);
        }

    }

    private void TrackBatloOrientation()
    {
        BaltoOrientation.forward = BaltoRef.forward.normalized * -1;
    }

    private void Sprint(float Multiplier)
    {
        SpeedMultiplier = Multiplier;
    }

    private void StartDash()
    {
        if (DashDirection != Vector2.zero && !PlayerDashing)
        {
            PlayerAudioSource.Stop();
            PlayerAudioSource.clip = PlayerInteractScript.HandleAudioClip("Dashing", true);

            Debug.Log("kickback;");
            Vector3 SetDashDirection = (PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x) * DashDistance;
            PlayerAttackScript.SetDodgeInfo((PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x).normalized);
            
            RigidbodyRef.AddForce(SetDashDirection * 25, ForceMode.Impulse);
            PlayerDashing = true;

            StartCoroutine(DashTime());
        }
    }

    public void ApplyKnockback(Vector3 Direction)
    {
        Direction.y = 0.5f;
        Vector3 Force = Direction + (PlayerVelocity * -1)/3;
        RigidbodyRef.AddForce(Force * 30, ForceMode.Impulse);
    }

    private void ReduceDashVelocity()
    {
        if(PlayerDashing)
        {
            RigidbodyRef.velocity *= 0.975f;
        }
    }

    private void DashResetTimer()
    {
        DashSet = DashDirection != Vector2.zero;

        if (DashSet)
        {
            DashDelayTimer -= Time.deltaTime;
            if (DashDelayTimer <= 0)
            {
                DashDirection = Vector2.zero;
                DashDelayTimer = DashSetTime;
            }
        }
    }

    private void HandleDashDirection()
    {
        Vector2 NewDashDirection = PlayerInputRef.BasePlayerMovement.DashReading.ReadValue<Vector2>().normalized;

        if (DashDirection == Vector2.zero && NewDashDirection != Vector2.zero)
        {
            DashDirection = NormalizeDashDirection(NewDashDirection);
        }

        if (NewDashDirection != Vector2.zero && NewDashDirection != DashDirection)
        {
            DashDirection = NormalizeDashDirection(NewDashDirection);
        }
    }

    private Vector2 NormalizeDashDirection(Vector2 inputDirection)
    {
        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
        {
            return new Vector2(Mathf.Sign(inputDirection.x), 0); // Prioritize horizontal
        }
        else if (Mathf.Abs(inputDirection.x) < Mathf.Abs(inputDirection.y))
        {
            return new Vector2(0, Mathf.Sign(inputDirection.y)); // Prioritize vertical
        }
        else // If both are equal in magnitude
        {
            return new Vector2(Mathf.Sign(inputDirection.x), 0); // Default to horizontal
        }
    }


    private void AlignToSurface()
    {
        RaycastHit HitInfo;
        if(Physics.Raycast(transform.position, Vector3.down, out HitInfo, Mathf.Infinity, GroundLayers))
        {
            // Get the surface normal
            Vector3 surfaceNormal = HitInfo.normal;

            // Calculate the desired rotation to align with the surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

            // Decompose the current rotation into Euler angles
            Vector3 currentEulerAngles = transform.rotation.eulerAngles;

            // Decompose the target rotation into Euler angles
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Additively modify only the X and Z axes, keeping the Y axis unchanged
            float smoothedX = Mathf.LerpAngle(currentEulerAngles.x, targetEulerAngles.x, 100 * Time.deltaTime);
            float smoothedZ = Mathf.LerpAngle(currentEulerAngles.z, targetEulerAngles.z, 100 * Time.deltaTime);

            // Apply the new rotation
            transform.rotation = Quaternion.Euler(smoothedX, currentEulerAngles.y, smoothedZ);
        }
    }

    private void TrackPlayerMovement()
    {
        
        bool FrontGrounded = Physics.CheckSphere(Front_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        bool BackGrounded = Physics.CheckSphere(Back_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        Grounded = FrontGrounded || BackGrounded;

        float VerticalGravity = Grounded ? -9.81f : -27.81f*2;
        RigidbodyRef.AddForce(new Vector3(0, VerticalGravity, 0), ForceMode.Acceleration);

        HandleAnimationStates();
        InputDirection = PlayerInputRef.BasePlayerMovement.Movement.ReadValue<Vector3>().normalized;
        InputDirection = InputDirection.RoundVector(0);

        PlayerVelocity = new Vector3(RigidbodyRef.velocity.x, RigidbodyRef.velocity.y, RigidbodyRef.velocity.z);
        CurrentSpeed = RigidbodyRef.velocity.magnitude;
    }

    protected void MovePlayer()
    {
        //if (!Grounded) { return; }
        if(PlayerDashing) { return; }

        if (Attacking || AttackLocked)
        {
            CanMove = false;
            RigidbodyRef.velocity = Vector3.zero;
            return;
        }

        MoveDelay();
        if (InputDirection.magnitude <= 0.1f )
        {
            RigidbodyRef.velocity = new Vector3(0,RigidbodyRef.velocity.y,0);
            CanMove = false;
            PlayerAudioSource.Stop();
            return;
        }

        if (PlayerAudioSource.clip == null || CurrentClipName != PlayerAudioSource.clip.name )
        {
            PlayerAudioSource.clip = PlayerInteractScript.HandleAudioClip("Walking", true);
        }
        if (CurrentSpeed <= 5)
        {
            Debug.Log("Fuck");
            PlayerAudioSource.Stop();
        }
        if (!CanMove) { return; }
        //Debug.Log("turn to rust");
        if(!PlayerAudioSource.isPlaying && CurrentSpeed > 4)
        {
            PlayerAudioSource.Play();
            //Debug.Log(PlayerAudioSource.isPlaying);
        }

        PlayerVelocity.y = (Grounded) ? 0 : PlayerVelocity.y;
        Vector3 CustomVelocity = new Vector3(RigidbodyRef.velocity.x, RigidbodyRef.velocity.y, RigidbodyRef.velocity.z);
        if (CustomVelocity.magnitude > Speed) 
        {
            return;
            Vector3 VelocityCap = CustomVelocity.normalized * Speed;
            RigidbodyRef.velocity = new Vector3(VelocityCap.x, VelocityCap.y
                , VelocityCap.z);
        }

        MoveDirection = PlayerOrientation.forward * InputDirection.z + PlayerOrientation.right * InputDirection.x;
        RigidbodyRef.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z) * 10f * (Speed) , ForceMode.Force);


        if (CanMove)
        {

            if (CurrentSpeed <= WalkSpeed+3)
            {
                PlayerAnimations.SetBool("IsRunning", false);

            }
            else
            {
                PlayerAnimations.SetBool("IsRunning", true);

            }

        }

    }

    private void HandleAnimationStates()
    {
        Attacking = PlayerAnimations.GetBool("IsAttacking");


        if (PlayerAnimations.GetBool("IsAttacking"))
        {
            //Debug.Log("Its true");
            PlayerAnimations.SetBool("Is Moving", false);
            PlayerAnimations.SetBool("IsRunning", false);
            return;
        }
        switch (CurrentSpeed)
        {
            case <= 0.1f:
                PlayerAnimations.SetBool("Is Moving", false);
                break;

            case >= 1:
                PlayerAnimations.SetBool("Is Moving", true);
                break;
        }

    }

    private void MoveDelay()
    {
        if (InputDirection == Vector3.zero) { CanMove = false; CurrentMoveDelayTime = MovingDelayTimer; return; }
        if (!CanMove)
        {
            CurrentMoveDelayTime -= Time.deltaTime;
            if(CurrentMoveDelayTime <= 0)
            {
                CanMove = true;
                CurrentMoveDelayTime = MovingDelayTimer;
            }
        }
        if (InputDirection != Vector3.zero && CanMove)
        {
            CurrentMoveDelayTime = MovingDelayTimer;
        }
    }

    private IEnumerator DashTime()
    {
        yield return new WaitForSeconds(0.45f);
        PlayerDashing = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out SlopeHit, transform.lossyScale.y + 1.25f))
        {
            float Angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            return Angle < MaxSlopeAngle && Angle != 0;
        }
        return false;
    }

    private Vector3 GetMoveDirecOnSlope()
    {
        return Vector3.ProjectOnPlane(MoveDirection,SlopeHit.normal).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Front_GroundCheckTransform.position, DetectionRadius);
        Gizmos.DrawWireSphere(Back_GroundCheckTransform.position, DetectionRadius);
    }

}
