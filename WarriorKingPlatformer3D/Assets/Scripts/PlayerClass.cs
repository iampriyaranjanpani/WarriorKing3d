using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClass : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    
    private PlayerInput playerInput;
    //input actions
    private InputAction movement;
    private InputAction jump;
    private InputAction attack;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping;

    private Animator animator;
    private int moveID;
    private int jumpID;
    private int attackID1;
    private int attackID2;
    private int attackID3;
    private int isAttackingID;
    private int playerDeadID;
    private int levelWinID;

    //Combo Attack
    [SerializeField] GameObject sword;
    private bool isAttacking = false;
    private int comboCount = 0;
    [SerializeField] private float comboTimeWindow = 0.5f;
    private float comboTimer;


    //Audio
    [SerializeField] AudioSource jumpAudioSource;
    [SerializeField] AudioSource swordAudioSource;
    [SerializeField] AudioClip[] swordAudioClips;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        movement = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        attack = playerInput.actions["SwordAttack"];
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        moveID = Animator.StringToHash("isMoving");
        jumpID = Animator.StringToHash("isJumping");
        attackID1 = Animator.StringToHash("Attack1");
        attackID2 = Animator.StringToHash("Attack2");
        attackID3 = Animator.StringToHash("Attack3");
        isAttackingID = Animator.StringToHash("isAttacking");
        playerDeadID = Animator.StringToHash("isPlayerDead");
        levelWinID = Animator.StringToHash("isLevelWin");
    }
    private void OnEnable()
    {
        movement.started += OnMoveStarted;
        movement.canceled += OnMoveCanceled;
        jump.started += OnJumpStarted;
        attack.started += StartAttack;
    }

    private void OnDisable()
    {
        movement.started -= OnMoveStarted;
        movement.canceled -= OnMoveCanceled;
        jump.started -= OnJumpStarted;
        attack.canceled -= StartAttack;
    }
    private void FixedUpdate()
    {
        //movement
        rb.velocity = new Vector3(moveInput.x * moveSpeed * Time.deltaTime , rb.velocity.y, 0f);        
    }
    private void Update()
    {
        // Handle combo timing
        if (isAttacking)
        {
            comboTimer += Time.deltaTime;

            // Check if combo window has passed
            if (comboTimer >= comboTimeWindow)
            {
                comboCount = 0;
                isAttacking = false;
            }
        }
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Movement Called");
        animator.SetBool(moveID, true);
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.x < 0f) // Checking if the player is moving left (input value < 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // Rotate the player -90 degrees around the y-axis
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f); // Rotate the player 90 degrees around the y-axis
        }
    }

    public void OnMoveStartedLeft()
    {
        Debug.Log("Movement Left Called");
        animator.SetBool(moveID, true);
        moveInput = new Vector2(-1f, 0f);
        transform.rotation = Quaternion.Euler(0f, -90f, 0f); // Rotate the player -90 degrees around the y-axis
    }

    public void OnMoveStartedRight()
    {
        Debug.Log("Movement Right Called");
        animator.SetBool(moveID, true);
        moveInput = new Vector2(1f, 0f);
        transform.rotation = Quaternion.Euler(0f, 90f, 0f); // Rotate the player 90 degrees around the y-axis
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Movement Cancelled");
        animator.SetBool(moveID, false);
        moveInput = context.ReadValue<Vector2>();
        //rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }
    public void OnMoveCanceledBt()
    {
        Debug.Log("Movement Cancelled");
        animator.SetBool(moveID, false);
        moveInput = Vector2.zero;
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            jumpAudioSource.Play();
            animator.SetBool(jumpID, true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void OnJumpStartedBt()
    {
        if (isGrounded)
        {
            jumpAudioSource.Play();
            animator.SetBool(jumpID, true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool(jumpID, false);
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    private void StartAttack(InputAction.CallbackContext context)
    {
        StartAttackBt();
    }
    public void StartAttackBt()
    {
        sword.tag = "Sword";
        int randomIndex = Random.Range(0, 2);
        AudioClip swordAudio = swordAudioClips[randomIndex];
        swordAudioSource.PlayOneShot(swordAudio);
        // Perform different actions based on the combo count
        switch (comboCount)
        {
            case 0:
                Debug.Log("Case1 registered=-=");
                // Play first attack animation
                animator.SetTrigger(attackID1);
                break;
            case 1:
                Debug.Log("Case2 registered=-=");
                // Play second attack animation
                animator.SetTrigger(attackID2);
                break;
            case 2:
                Debug.Log("Case3 registered=-=");
                // Play third attack animation
                animator.SetTrigger(attackID3);
                break;
            default:
                // No more attacks, reset combo
                comboCount = 0;
                break;
        }

        comboCount++;
        comboTimer = 0f;
        // Set attacking flag
        isAttacking = true;
        animator.SetBool(isAttackingID, true);
    }

    public void ComboAttackFinished()
    {
        sword.tag = "Untagged";
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.fullPathHash == attackID1 || currentState.fullPathHash == attackID2 || currentState.fullPathHash == attackID3)
        {
            // An attack animation is already playing, so we return and prevent starting a new attack
        Debug.Log("ComboAttackFInished-=-=-");
            return;
        }
        // Attack animation event called when the attack animation finishes
        animator.SetBool(isAttackingID,false);
        isAttacking = false;
        comboCount = 0;
    }
    public void PlayerDeathCalled()
    {
        OnDisable();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetTrigger(playerDeadID);
    }
    public void GameOverCalled()
    {
        gameManager.GameOver();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            LevelComplete();
        }
    }
    private void LevelComplete()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetTrigger(levelWinID);
        gameManager.LevelWin();
    }
}
