using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody Rigidbody;
    public InputActionReference PlayerActionMap;
    [SerializeField] private PlayerInput PlayerInputRef;
    [SerializeField] private PlayerInput DashInput;

    public Transform MainCamera; 

    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;
    public float Speed;

    protected float TurnSmoothingVel;
    public float TurnTime = 0.1f;

    private float MovingDelayTimer = 0.15f;
    [SerializeField] private float CurrentMoveDelayTime;

    private float DashSetTime = 0.3f;
    [SerializeField] private float DashDelayTimer;
    [SerializeField] private float DashDistance=20;

    public Animator PlayerAnimations;

    public Vector3 PlayerVelocity;
    [SerializeField] protected Vector3 MoveDirection;
    [SerializeField] protected Vector3 InputDirection;

    [SerializeField] protected Vector2 DashDirection;
    [SerializeField] protected Vector3 NewDashPosition;
    [SerializeField] protected Vector3 PreviousPosition;

    public Transform PlayerOrientation;
    public Transform BaltoOrientation;
    public Transform BaltoRef;
    public GameObject HitParticle;

    public bool AttackLocked = false;
    public bool CanMove = false;
    private bool DashSet = false;
    private bool PlayerDashing = false;
    private bool CanDash = false;

    // Start is called before the first frame update
    void Start()
    {
        CanMove = false;
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;

        PlayerInputRef = new PlayerInput();
        PlayerInputRef.Enable();
        PlayerInputRef.BasePlayerMovement.MovementModifiers.performed += Context => Sprint(2.5f);
        PlayerInputRef.BasePlayerMovement.MovementModifiers.canceled += Context => Sprint(1f);
        SpeedMultiplier = 1;

        PlayerInputRef.BasePlayerMovement.DashMovement.performed += Context => StartDash();
        PlayerInputRef.BasePlayerMovement.DashReading.performed += Context => HandleDashDirection();

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
    }

    private void Update()
    {

        Speed = BaseMoveSpeed * SpeedMultiplier;
        MovePlayer();
        DashResetTimer();
        if(PlayerDashing)
        {
            HandleDashing();
        }
        TrackBatloOrientation();
    }

    private void TrackBatloOrientation()
    {
        BaltoOrientation.forward = BaltoRef.forward.normalized *-1;
    }

    private void Sprint(float Multiplier)
    {
        SpeedMultiplier = Multiplier;
    }

    private void HandleDashing()
    {
        if (!PlayerDashing) { return; }
        Vector3 CurrentPosition = transform.position.RoundVector(2);
        if(CurrentPosition==NewDashPosition) 
        {
            PlayerDashing = false;

        }
        CurrentPosition = Vector3.MoveTowards(CurrentPosition, NewDashPosition, 120 * Time.deltaTime);
        transform.position = CurrentPosition;

    }

    private void StartDash()
    {
        if (DashDirection != Vector2.zero && !PlayerDashing)
        {
            Debug.Log("kickback;");
            PlayerDashing = true;
            Vector3 ForwardDirection = transform.forward;

            // Normalize the DashDirection and apply it to forward direction
            Vector3 SetDashDirection = (PlayerOrientation.forward * DashDirection.y + PlayerOrientation.right * DashDirection.x) * DashDistance;
            SetDashDirection = SetDashDirection.RoundVector(2);
            //Vector3 SetDashDirection = (new Vector3(DashDirection.y, 0, 0)) * DashDistance;

            NewDashPosition = (this.transform.position.RoundVector(2) + (SetDashDirection));
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
        //Debug.Log(DashingDirection);
        Vector2 NewDashDirection = PlayerInputRef.BasePlayerMovement.DashReading.ReadValue<Vector2>().normalized;
        if (DashDirection==Vector2.zero)
        {
            DashDirection = PlayerInputRef.BasePlayerMovement.DashReading.ReadValue<Vector2>().normalized;
        }
        if (DashDirection != Vector2.zero && NewDashDirection != DashDirection && NewDashDirection != Vector2.zero) 
        {
            //DashingDirection = Vector2.zero;
            DashDirection = NewDashDirection;
        }
    }

    protected void MovePlayer()
    {
        HandleAnimationStates();
        InputDirection = PlayerInputRef.BasePlayerMovement.Movement.ReadValue<Vector3>().normalized;
        InputDirection = InputDirection.RoundVector(0);

        PlayerVelocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y, Rigidbody.velocity.z);
        CurrentSpeed = Rigidbody.velocity.magnitude;

        MoveDelay();
        if (InputDirection.magnitude <= 0.1f)
        {
            Rigidbody.velocity = Vector3.zero;
            CanMove = false;
            return;
        }

        if (!CanMove) { return; }
        Debug.Log("turn to rust");



        if (PlayerVelocity.magnitude > Speed) 
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * Speed;
            Rigidbody.velocity = new Vector3(VelocityCap.x, 0
                , VelocityCap.z);
        }

        MoveDirection = PlayerOrientation.forward * InputDirection.z + PlayerOrientation.right * InputDirection.x;
        //Speed = BaseMoveSpeed * SpeedMultiplier;
        Rigidbody.AddForce(new Vector3(MoveDirection.x, MoveDirection.y, MoveDirection.z) * 10f * (Speed) , ForceMode.Force);

    }

    private void HandleAnimationStates()
    {
        AttackLocked = PlayerAnimations.GetBool("IsAttacking");

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

}
