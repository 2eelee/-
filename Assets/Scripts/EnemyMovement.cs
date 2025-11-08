using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private Animator anim;
    [SerializeField] private Transform player;

    public float movespeed;
    public float attackRange;
    public float attackCoolDown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float facingDirection = -1;
    private EnemyState enemyState, newState;
    private float attackCoolDownTimer;
    void Start()
    {
        ChangeState(EnemyState.Idle);
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        if (attackCoolDownTimer > 0)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * movespeed, Time.deltaTime * 5f);
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange && attackCoolDownTimer <= 0)
        {
            attackCoolDownTimer = attackCoolDown;
            ChangeState(EnemyState.Attacking);
            Attack();
        }
        else if ((direction.x < 0 && facingDirection == -1) || (direction.x > 0 && facingDirection == 1))
        {
            FlipX();
        }
    }
    void FlipX()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCoolDownTimer <= 0)
            {
                attackCoolDownTimer = attackCoolDown;
                ChangeState(EnemyState.Attacking);
                Attack();
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }

    }
    void ChangeState(EnemyState newState)
    {
        enemyState = newState;
    }
    void Attack()
    {
        GetComponent<EnemyCombat>().Attack();
    }
    private void OnGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
}
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}