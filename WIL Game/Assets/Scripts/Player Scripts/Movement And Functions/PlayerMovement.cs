using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Unity Components")]

    #region Unity Components
    public InputActionReference PlayerActionMap;
    private PlayerInput PlayerInputRef;
    [SerializeField] private PlayerInput DashInput;
    public LayerMask GroundLayers;
    private RaycastHit SlopeHit;
    [SerializeField]private AudioSource PlayerAudioSource;

    public Rigidbody RigidbodyRef;
    public Animator PlayerAnimations;
    #endregion

    [Header("Scripts"), Space(1.25f)]

    #region Scripts
    private PlayerInteraction PlayerInteractScript;
    private WorldHandler WorldHandlerScript;
    private PlayerAttacks PlayerAttackScript;
    #endregion


    [Header("Floats"),Space(1.25f)]
    //Floats
    #region Floats
    //Basic movement 
    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;
    public float MaxSpeed;
    
    private float MovingDelayTimer = 0.0425f;
    [SerializeField] private float CurrentMoveDelayTime;
    [SerializeField] private float MovementSpeedChange = 10;

    //Speed smoothing
    [SerializeField] private float TargetSpeed;
    private float OldTargetSpeed;


    //Dashing
    private float DashSetTime = 0.93f;  //Setting the dash direction 
    [SerializeField] private float DashSpeed = 20;

    //Environment Detection
    private float MaxSlopeAngle = 40f;
    private float CurrentSlopeAngle;
    private float DetectionRadius = 0.45f;

    private float BaseNormalDrag;
    private float BaseAngleDrag;

    #endregion


    [Header("Vectors"), Space(1.25f)]

    #region Vectors
    public Vector3 PlayerVelocity;
    [SerializeField] protected Vector3 MoveDirection;
    [SerializeField] protected Vector3 InputDirection;
    [Space(2)]
    [SerializeField] protected Vector2 DashDirection;
    [SerializeField] protected Vector3 NewDashPosition;
    [SerializeField] protected Vector3 PreviousPosition;
    public Vector3 Gravity;
    [Space(2)]
    #endregion

    [Header("Transforms"), Space(1.25f)]
    
    #region Transforms
    public Transform PlayerOrientation;
    public Transform BaltoModel;

    [SerializeField] protected Transform Front_GroundCheckTransform;
    [SerializeField] protected Transform Back_GroundCheckTransform;
    #endregion

    [Header("Bools"), Space(1.25f)]

    #region
    public bool Attacking = false;
    public bool AttackLocked = false;
    public bool CanMove = false;
    [SerializeField]private bool DashSet = false;
    [SerializeField]private bool PlayerDashing = false;

    [SerializeField]public bool Grounded;
    public bool StunLocked = false;
    [SerializeField] private bool KeepMomentum;
    public bool KnockbackLocked;
    #endregion


    private string CurrentClipName;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        StunLocked = false;
        CanMove = true;
        RigidbodyRef = GetComponent<Rigidbody>();
        CurrentSpeed = 0;

        PlayerInputRef = new PlayerInput();
        PlayerInputRef.Enable();

        PlayerInputRef.BasePlayerMovement.Enable();

        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed += Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled += Context => Sprint(1f);
        SpeedMultiplier = 1f;

        PlayerInputRef.BasePlayerMovement.DashReading.performed += Context => HandleDashDirection();
        PlayerInputRef.BasePlayerMovement.DashMovement.performed += Context => StartDash();

        PlayerAttackScript = GetComponentInChildren<PlayerAttacks>();
        PlayerInteractScript = gameObject.GetComponent<PlayerInteraction>();

        CurrentMoveDelayTime = MovingDelayTimer;

        PlayerAudioSource = this.GetComponent<AudioSource>();
        WorldHandlerScript = FindObjectOfType<WorldHandler>();
    }

    private void FixedUpdate()
    {
        if (StunLocked || PlayerInteractScript.AtEnd) { return; }

        HandlePlayerMovement();
        if (PlayerAnimations.GetBool("IsAttacking") || AttackLocked || PlayerAttackScript.ChannelAttack)
        {
            CanMove = false;
            RigidbodyRef.velocity = Vector3.zero;
            MoveDirection = Vector3.zero;
            //Debug.Log("laced with poison");
            return;
        }

        //Keep this in the fixed update method because if it isnt then it
        //causes the player to jitter and stutter

        MaxSpeed = BaseMoveSpeed * SpeedMultiplier;

        MovePlayer();
        AlignToSurface();
    }

    public void StopMoving()
    {
        InputDirection = Vector3.zero;
        MoveDirection = Vector3.zero;
        CurrentSpeed = 0;
        RigidbodyRef.velocity = Vector3.zero;
        PlayerAnimations.SetBool("Is Moving", false);
        PlayerAnimations.SetBool("Is Running", false);
    }

    private void Update()
    {
        if (StunLocked) { return; }

        bool TargetSpeedChanged = TargetSpeed != OldTargetSpeed;

        if (!PlayerDashing)
        {
            TargetSpeed = BaseMoveSpeed;
        }

        if (TargetSpeedChanged)
        {
            Debug.Log("down");
            if (KeepMomentum)
            {
                StopCoroutine(SmoothMoveSpeed());
                StartCoroutine(SmoothMoveSpeed());
                Debug.Log("overdrive");
            }
            else
            {
                StopCoroutine(SmoothMoveSpeed());
                CurrentSpeed = TargetSpeed;
                Debug.Log("get inside my head");
            }
        }
        OldTargetSpeed = TargetSpeed;

        HandlePlayerMovement();
        ReduceDashVelocity();
        AlignToSurface();
        ControlSpeed();


        Vector3 MessuredVelocity = new Vector3(PlayerVelocity.x, 0, PlayerVelocity.z);
        if (MessuredVelocity.magnitude > MaxSpeed && !PlayerDashing)
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * MaxSpeed;
            RigidbodyRef.velocity = new Vector3(VelocityCap.x, RigidbodyRef.velocity.y
                , VelocityCap.z);
        }

    }

    private void ControlSpeed()
    {

        if (!Grounded && RigidbodyRef.useGravity)
        {
            RigidbodyRef.AddForce(Vector3.down * 6.25f, ForceMode.Force);
        }

        if (PlayerVelocity.magnitude > MaxSpeed)
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * CurrentSpeed;
            RigidbodyRef.velocity = new Vector3(VelocityCap.x, RigidbodyRef.velocity.y, VelocityCap.z);
        }

        if (InputDirection == Vector3.zero && !KnockbackLocked)
        {
            CurrentSpeed = 0;
            PlayerVelocity = Vector3.zero;
            MoveDirection = Vector3.zero;
        }
    }

    private void Sprint(float Multiplier)
    {
        SpeedMultiplier = Multiplier;
    }

    private void StartDash()
    {
        //if (PlayerAttackScript.ChannelAttack) { return; }
        if (DashDirection != Vector2.zero && !PlayerDashing)
        {
            TargetSpeed = DashSpeed;
            KeepMomentum = true;
            RigidbodyRef.useGravity = false;
            MovementSpeedChange = 80;
            Debug.Log("kickback;");
            Vector3 SetDashDirection = (PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x) * DashSpeed;

            RigidbodyRef.AddForce(SetDashDirection, ForceMode.Impulse);
            PlayerDashing = true;

            //Cinemachine was messing up and the jittering was because of that. You arent dumb
            //The fix was to change the update method and the blend update method to fixed update for both

            //The original was smart and late, respectively
            Invoke(nameof(DashReset), 0.45f);
        }
    }

    private IEnumerator SmoothMoveSpeed()
    {

        float SmoothTime = 0;
        float SpeedDifference = Mathf.Abs(TargetSpeed - CurrentSpeed);
        float StartSpeed = CurrentSpeed;

        float SpeedBoostValue = MovementSpeedChange;

        while (SmoothTime < SpeedDifference)
        {
            CurrentSpeed = Mathf.Lerp(StartSpeed, TargetSpeed, SmoothTime / SpeedDifference);
            SmoothTime += Time.deltaTime * SpeedBoostValue;
            Debug.Log("reee");

            yield return null;
        }

        CurrentSpeed = TargetSpeed;
        MovementSpeedChange = 1;
        KeepMomentum = false;

    }

    public void ApplyKnockback(Vector3 Direction)
    {
        Direction.y = 0.5f;
        Vector3 Force = Direction + (PlayerVelocity * -1) / 3;
        RigidbodyRef.AddForce(Force * 30, ForceMode.Impulse);
    }

    private void ReduceDashVelocity()
    {
        if (PlayerDashing)
        {
            RigidbodyRef.velocity *= 0.975f;
        }
    }

    private void OnDestroy()
    {
        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed -= Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled -= Context => Sprint(1f);

        PlayerInputRef.BasePlayerMovement.DashReading.performed -= Context => HandleDashDirection();
        PlayerInputRef.BasePlayerMovement.DashMovement.performed -= Context => StartDash();
        PlayerInputRef.Dispose();
        PlayerInputRef.Disable();
    }

    private void OnDisable()
    {
        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed -= Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled -= Context => Sprint(1f);

        PlayerInputRef.BasePlayerMovement.DashReading.performed -= Context => HandleDashDirection();
        PlayerInputRef.BasePlayerMovement.DashMovement.performed -= Context => StartDash();

        PlayerInputRef.Dispose();
        PlayerInputRef.Disable();

    }
    private void HandleDashDirection()
    {
        if (PlayerAttackScript.ChannelAttack) { return; }
        Debug.Log("zuzu");
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
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit HitInfo, 10, GroundLayers))
        {

            Vector3 SurfaceNormal = HitInfo.normal;
            CurrentSlopeAngle = Vector3.Angle(Vector3.up, HitInfo.normal);

            Quaternion TargetRotation = Quaternion.FromToRotation(BaltoModel.up, SurfaceNormal) * BaltoModel.rotation;
            TargetRotation = Quaternion.Euler(TargetRotation.eulerAngles.x, BaltoModel.eulerAngles.y, TargetRotation.eulerAngles.z);


            Vector3 CurrentRotation = Quaternion.Slerp(BaltoModel.rotation, TargetRotation, 20 * Time.deltaTime).eulerAngles;
            BaltoModel.rotation = Quaternion.Euler(CurrentRotation);

        }
    }
    private void HandlePlayerMovement()
    {

        Vector2 VelocityCondition = new Vector2(PlayerVelocity.x, PlayerVelocity.z);
        if (InputDirection == Vector3.zero && VelocityCondition != Vector2.zero)
        {
            MoveDirection = Vector3.zero;
            RigidbodyRef.velocity = new Vector3(0, RigidbodyRef.velocity.y, 0);

        }

        bool FrontGrounded = Physics.CheckSphere(Front_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        bool BackGrounded = Physics.CheckSphere(Back_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        Grounded = FrontGrounded || BackGrounded;


        //Updates the players current input direction
        InputDirection = PlayerInputRef.BasePlayerMovement.Movement.ReadValue<Vector3>().normalized;

        PlayerVelocity = new Vector3(RigidbodyRef.velocity.x, RigidbodyRef.velocity.y, RigidbodyRef.velocity.z);
        CurrentSpeed = RigidbodyRef.velocity.magnitude;
    }

    protected void MovePlayer()
    {
        //if (!Grounded) { return; }
        if (PlayerDashing) { return; }

        if (Attacking || AttackLocked)
        {
            CanMove = false;
            RigidbodyRef.velocity = Vector3.zero;
            return;
        }

        TargetSpeed = BaseMoveSpeed * SpeedMultiplier;

        MoveDelay();

        CurrentSpeed = BaseMoveSpeed * SpeedMultiplier;

        if (OnSlope())
        {
            RigidbodyRef.AddForce(GetMoveDirecOnSlope(MoveDirection) * CurrentSpeed * 10f, ForceMode.Force);
        }

        Debug.Log("Gods not home");
        PlayerVelocity.y = (Grounded) ? 0 : PlayerVelocity.y;


        MoveDirection = PlayerOrientation.forward * InputDirection.z + PlayerOrientation.right * InputDirection.x;
        RigidbodyRef.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z).normalized * 10f * (CurrentSpeed), ForceMode.Force);


        if (CanMove)
        {
            //Change to a turnary operator
            if (CurrentSpeed <= BaseMoveSpeed + 3)
            {
                //PlayerAnimations.SetBool("IsRunning", false);

            }
            else
            {
                //PlayerAnimations.SetBool("IsRunning", true);

            }

        }

    }


    protected void OldMovePlayer()
    {
        //if (!Grounded) { return; }
        //if (PlayerDashing) { return; }

        //if (Attacking || AttackLocked)
        //{
        //    CanMove = false;
        //    RigidbodyRef.velocity = Vector3.zero;
        //    return;
        //}

        //MoveDelay();
        //if (InputDirection.magnitude <= 0.1f)
        //{
        //    RigidbodyRef.velocity = new Vector3(0, RigidbodyRef.velocity.y, 0);
        //    CanMove = false;
        //    PlayerAudioSource.Stop();
        //    return;
        //}

        //if (PlayerAudioSource.clip == null || CurrentClipName != PlayerAudioSource.clip.name)
        //{
        //    PlayerAudioSource.clip = PlayerInteractScript.HandleAudioClip("Walking", true);
        //}
        //if (CurrentSpeed <= 5)
        //{

        //    PlayerAudioSource.Stop();
        //}
        //if (!CanMove) { return; }
        ////Debug.Log("turn to rust");
        //if (!PlayerAudioSource.isPlaying && CurrentSpeed > 4)
        //{
        //    PlayerAudioSource.Play();
        //    //Debug.Log(PlayerAudioSource.isPlaying);
        //}

        ////PlayerVelocity.y = (Grounded) ? 0 : PlayerVelocity.y;
        //Vector3 CustomVelocity = new Vector3(RigidbodyRef.velocity.x, RigidbodyRef.velocity.y, RigidbodyRef.velocity.z);
        //if (CustomVelocity.magnitude > MaxSpeed)
        //{

        //    //if there is an odd bug in the code with velocity not reseting correctly, look at this shit
        //    return;
        //    Vector3 VelocityCap = CustomVelocity.normalized * MaxSpeed;
        //    RigidbodyRef.velocity = new Vector3(VelocityCap.x, VelocityCap.y
        //        , VelocityCap.z);
        //}

        //MoveDirection = PlayerOrientation.forward * InputDirection.z + PlayerOrientation.right * InputDirection.x;
        //RigidbodyRef.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z) * 10f * (MaxSpeed), ForceMode.Force);


        //if (CanMove)
        //{

        //    if (CurrentSpeed <= BaseMoveSpeed + 3)
        //    {
        //        PlayerAnimations.SetBool("IsRunning", false);

        //    }
        //    else
        //    {
        //        PlayerAnimations.SetBool("IsRunning", true);

        //    }

        //}

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
            if (CurrentMoveDelayTime <= 0)
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

    private void DashReset()
    {
        PlayerDashing = false;


        RigidbodyRef.drag = BaseNormalDrag;
        RigidbodyRef.angularDrag = BaseAngleDrag;

        RigidbodyRef.useGravity = true;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, transform.lossyScale.y + 1.25f))
        {
            float Angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            return Angle < MaxSlopeAngle && Angle != 0;
        }
        return false;
    }

    private Vector3 GetMoveDirecOnSlope(Vector3 Direction)
    {
        return Vector3.ProjectOnPlane(MoveDirection, SlopeHit.normal).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Front_GroundCheckTransform.position, DetectionRadius);
        Gizmos.DrawWireSphere(Back_GroundCheckTransform.position, DetectionRadius);
    }

}
