using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody Rigidbody;
    public InputActionReference PlayerActionMap;
    private PlayerAttacks PlayerAttackScript;
    [SerializeField] private PlayerInput PlayerInputRef;
    [SerializeField] private PlayerInput DashInput;
    public LayerMask GroundLayers;
    private RaycastHit SlopeHit;


    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;
    public float Speed;
    public float DetectionRadius=0.35f;

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

    // Start is called before the first frame update
    void Start()
    {
        CanMove = true;
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;

        PlayerInputRef = new PlayerInput();
        PlayerInputRef.Enable();
        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed += Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled += Context => Sprint(1f);
        SpeedMultiplier = 1f;

        PlayerInputRef.BasePlayerMovement.DashReading.performed += Context => HandleDashDirection();
        PlayerInputRef.BasePlayerMovement.DashMovement.performed += Context => StartDash();

        PlayerAttackScript = GetComponentInChildren<PlayerAttacks>();


        CurrentMoveDelayTime = MovingDelayTimer;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.L))
        {
            Instantiate(HitParticle, transform.position, Quaternion.identity);
        }


        if (PlayerAnimations.GetBool("IsAttacking"))
        {
            Rigidbody.velocity = Vector3.zero;
            //Debug.Log("laced with poison");
            return;
        }

        //Keep this in the fixed update method because if it isnt then it
        //causes the player to jitter and stutter
        Speed = BaseMoveSpeed * SpeedMultiplier;

        MovePlayer();

    }

    private void Update()
    {
        TrackPlayerMovement();

        DashResetTimer();

        TrackBatloOrientation();
        ReduceDashVelocity();


        Vector3 MessuredVelocity = new Vector3(PlayerVelocity.x, 0, PlayerVelocity.z);
        if (MessuredVelocity.magnitude > Speed && !PlayerDashing)
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * Speed;
            Rigidbody.velocity = new Vector3(VelocityCap.x, Rigidbody.velocity.y
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
            Debug.Log("kickback;");
            Vector3 SetDashDirection = (PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x) * DashDistance;
            PlayerAttackScript.SetDodgeInfo((PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x).normalized);
            
            Rigidbody.AddForce(SetDashDirection * 25, ForceMode.Impulse);
            PlayerDashing = true;

            StartCoroutine(DashTime());
        }
    }

    private void ReduceDashVelocity()
    {
        if(PlayerDashing)
        {
            Rigidbody.velocity *= 0.975f;
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
                Debug.Log("all of my dreams");
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


    private void TrackPlayerMovement()
    {
        
        bool FrontGrounded = Physics.CheckSphere(Front_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        bool BackGrounded = Physics.CheckSphere(Back_GroundCheckTransform.position, DetectionRadius, GroundLayers);
        Grounded = FrontGrounded || BackGrounded;

        float VerticalGravity = Grounded ? -0.81f : -9.81f*2;
        Rigidbody.AddForce(new Vector3(0, VerticalGravity, 0), ForceMode.Acceleration);

        HandleAnimationStates();
        InputDirection = PlayerInputRef.BasePlayerMovement.Movement.ReadValue<Vector3>().normalized;
        InputDirection = InputDirection.RoundVector(0);

        PlayerVelocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y, Rigidbody.velocity.z);
        CurrentSpeed = Rigidbody.velocity.magnitude;
    }

    protected void MovePlayer()
    {
        //if (!Grounded) { return; }
        if(PlayerDashing) { return; }

        if (Attacking || AttackLocked)
        {
            CanMove = false;
            Rigidbody.velocity = Vector3.zero;
            return;
        }

        MoveDelay();
        if (InputDirection.magnitude <= 0.1f )
        {
            Rigidbody.velocity = new Vector3(0,Rigidbody.velocity.y,0);
            CanMove = false;
            return;
        }

        if (!CanMove) { return; }
        //Debug.Log("turn to rust");



        if (PlayerVelocity.magnitude > Speed) 
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * Speed;
            Rigidbody.velocity = new Vector3(VelocityCap.x, 0
                , VelocityCap.z);
        }


        if (CanMove)
        {

        }

        MoveDirection = PlayerOrientation.forward * InputDirection.z + PlayerOrientation.right * InputDirection.x;
        //Speed = BaseMoveSpeed * SpeedMultiplier;
        Rigidbody.AddForce(new Vector3(MoveDirection.x, 0, MoveDirection.z) * 10f * (Speed) , ForceMode.Force);

    }

    private void HandleAnimationStates()
    {
        Attacking = PlayerAnimations.GetBool("IsAttacking");

        if (PlayerAnimations.GetBool("IsAttacking"))
        {
            //Debug.Log("Its true");
            PlayerAnimations.SetBool("Is Moving", false);
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
