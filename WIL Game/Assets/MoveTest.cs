using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveTest : MonoBehaviour
{
    public Rigidbody RigidbodyRef;
    public InputActionReference PlayerActionMap;
    //private PlayerAttacks PlayerAttackScript;
    [SerializeField] private PlayerInput PlayerInputRef;
    [SerializeField] private PlayerInput DashInput;
    //[SerializeField] private PlayerInteraction PlayerInteractScript;
    //private WorldHandler WorldHandlerScript;

    public LayerMask GroundLayers;
    private RaycastHit SlopeHit;
    //[SerializeField] private AudioSource PlayerAudioSource;

    public float BaseMoveSpeed;
    private float SpeedMultiplier;
    public float CurrentSpeed;
    private float MaxSpeed;
    public float DetectionRadius = 0.35f;
    private float GroundDrag;

    private float MovingDelayTimer = 0.05f;
    [SerializeField] private float CurrentMoveDelayTime;

    private float MaxSlopeAngle = 40f;
    private float CurrentSlopeAngle;

    private float BaseNormalDrag;
    private float BaseAngleDrag;
    [SerializeField] private float TargetSpeed;
    private float OldTargetSpeed;

    [SerializeField] private float MovementSpeedChange = 10;

    //Dashing
    private float DashSetTime = 0.93f;
    [SerializeField] private float DashDelayTimer;
    [SerializeField] private float DashSpeed = 20;

    public Vector3 PlayerVelocity;
    [SerializeField] protected Vector3 InputDirection;
    [SerializeField] protected Vector3 MoveDirection;

    [Space(2)]
    [SerializeField] protected Vector2 DashDirection;
    [Space(2)]


    public Transform PlayerOrientation;
    //public Transform BaltoOrientation;
    //public Transform BaltoRef;

    [SerializeField] protected Transform Front_GroundCheckTransform;
    [SerializeField] protected Transform Back_GroundCheckTransform;
    [SerializeField] private Transform BaltoModel;
    public Transform BaltoOrientation;


    public bool Attacking = false;
    public bool AttackLocked = false;
    public bool CanMove = false;

    [SerializeField] private bool DashSet = false;
    [SerializeField] private bool PlayerDashing = false;
    private bool CanDash = false;
    [SerializeField] private bool KeepMomentum;

    [SerializeField] public bool Grounded;
    [SerializeField] private bool IsMoving;
    public bool StunLocked = false;
    public bool KnockbackLocked;

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

        //PlayerAttackScript = GetComponentInChildren<PlayerAttacks>();
        //PlayerInteractScript = gameObject.GetComponent<PlayerInteraction>();

        CurrentMoveDelayTime = MovingDelayTimer;

        if (GroundDrag == 0)
        {
            GroundDrag = 2;
        }

        RigidbodyRef.drag = GroundDrag;
        BaseNormalDrag = RigidbodyRef.drag;
        BaseAngleDrag = RigidbodyRef.angularDrag;

        TargetSpeed = BaseMoveSpeed * SpeedMultiplier;

        //PlayerAudioSource = this.GetComponent<AudioSource>();
        //WorldHandlerScript = FindObjectOfType<WorldHandler>();
    }

    private void FixedUpdate()
    {
        if (StunLocked) { return; }
        //Keep this in the fixed update method because if it isnt then it
        //causes the player to jitter and stutter

        MaxSpeed = BaseMoveSpeed * SpeedMultiplier;

        MovePlayer();
    }

    public void StopMoving()
    {
        InputDirection = Vector3.zero;
        MoveDirection = Vector3.zero;
        CurrentSpeed = 0;
        RigidbodyRef.velocity = Vector3.zero;
        //PlayerAnimations.SetBool("Is Moving", false);
        //PlayerAnimations.SetBool("Is Running", false);
    }

    private void Update()
    {
        if (StunLocked) { return; }
        //DashResetTimer();

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
            MovementSpeedChange = 50;
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
        Debug.Log("zuzu");
        //if (PlayerAttackScript.ChannelAttack) { return; }
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
