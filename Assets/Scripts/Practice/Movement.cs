using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpHeigt = 10f;
    public float verticalVelocity;
    public float gravity = -9f;
    public bool isMoveable = true;

    private CharacterController characterController;

    [Header("OnGround")]
    [Tooltip("캐릭터의 땅 접촉 여부 확인")]
    public bool isGrounded = true;

    [Header("GroundCheck")]
    [Tooltip("오브젝트 땅 지정")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.35f;
    [SerializeField] private float groundCheckDistance = 0.15f;

    InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    Vector3 move;

    void Update()
    {
        
        MoveInputSet();    
    }
    void MoveInputSet()
    {
        //moveInput이 x축과 y축의 이동을 +1, -1로 받아낸다.
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        //move의 횡이동이 moveInput의 x축을, move의 앞뒤 이동이 moveInput의 y축 입력을 받는다.
        move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // verticalVelocity는 지속 중력 증가, 0이하로 떨어지면 -2 고정, 캐릭터의 y축은 verticalVelocity의 영향 
        verticalVelocity += gravity;

        if (verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        move.y = verticalVelocity;


    }
    void LateUpdate()
    {
        characterController.Move(move * Time.deltaTime);
    }
  
}
