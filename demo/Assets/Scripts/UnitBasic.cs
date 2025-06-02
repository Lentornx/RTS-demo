using UnityEngine;
using UnityEngine.UI;
using Pathfinding;


[RequireComponent(typeof(Seeker))]
public class UnitBasic : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.blue;
    private Color defaultColor;

    public GameObject healthBarPrefab;
    private Image healthBarFill;
    private GameObject healthBarInstance;

    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.0f;
    public int attackDamage = 10;
    private float lastAttackTime;

    public string faction = "Player"; // or "Enemy"
    public int maxHealth = 50;
    private int currentHealth;

    private Animator animator;
    private Seeker seeker;
    private Path currentPath;
    private int currentWaypoint = 0;
    private float nextWaypointDistance = 0.01f;
    private bool pathPending = false;

    private Vector3 lastPosition;
    public bool IsMoving { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();

        if (healthBarPrefab)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 0.3f, Quaternion.identity, transform);
            healthBarFill = healthBarInstance.transform.Find("Background/Fill").GetComponent<Image>();
        }
    }

    void Update()
    {
        // attack
        TryAttackNearbyEnemy();

        UpdateAnimatorParameters();

        // movement
        if (currentPath == null || pathPending) return;
        if (currentWaypoint >= currentPath.vectorPath.Count) return;

        Vector3 direction = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.position += movement;

        if (Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void Stop()
    {
        currentPath = null;
        currentWaypoint = 0;
        pathPending = false;
    }

    void TryAttackNearbyEnemy()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            UnitBasic other = hit.GetComponent<UnitBasic>();
            if (other != null && other.faction != this.faction)
            {
                // attack
                int attackCount = 2; // Total number of attack animations
                int randomIndex = Random.Range(1, attackCount + 1);

                animator.SetInteger("attackIndex", randomIndex);
                animator.SetTrigger("attackTrigger");

                Stop(); // Stop when attacking
                other.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
                break;
            }
        }
    }

    void UpdateAnimatorParameters()
    {
        Vector3 movement = transform.position - lastPosition;
        IsMoving = movement.magnitude > 0.001f;
        animator.SetBool("isMoving", IsMoving);

        if (movement.x > 0)
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;

        lastPosition = transform.position;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");

        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;

        if (healthBarFill)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: Play animation, effects, notify other systems, etc.
        Debug.Log($"{gameObject.name} died.");

        Deselect();
        Destroy(gameObject);
    }

    public void Select()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = defaultColor;
    }

    public void MoveTo(Vector2 targetPosition)
    {
        if (seeker.IsDone())
        {
            pathPending = true;
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        pathPending = false;
        if (!p.error)
        {
            currentPath = p;
            currentWaypoint = 0;
        }
        else
        {
            Debug.LogError("Pathfinding error: " + p.errorLog);
        }
    }

    // For debug purposes: draw attack range in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
