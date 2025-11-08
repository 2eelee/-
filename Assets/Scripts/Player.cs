using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public float facingDirection = -1;
    private bool knockedBack = false;
    private bool canMove = true;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    public Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (knockedBack == false)
        {
            if (!canMove)
            {
                moveInput = Vector2.zero;
                animator.SetBool("isRunning", false);
                return;
            }
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(horizontal, vertical).normalized;
            bool isMoving = moveInput.magnitude > 0;
            animator.SetBool("isRunning", isMoving);
            if ((horizontal < 0 && transform.localScale.x < 0) || (horizontal>0 && transform.localScale.x>0))
            {
                FlipX();
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
        }
    }
    void FlipX()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public bool CanMove()
    {
        return canMove;
    }
    public bool KnockBack(Transform enemy, float force, float stunTime)
    {
        knockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        animator.SetBool("knockedBack", true);
        StartCoroutine(KnockBackCouter(stunTime));
        return knockedBack;      
    }
    IEnumerator KnockBackCouter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        knockedBack = false;
    }
}