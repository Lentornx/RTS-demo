using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Collections;


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

    // logic parameters
    private UnitBasic enemy_target = null;
    public bool aiEnabled;
    public float detectionRange = 4.5f;
    public float spawnDesiredProximity = 3.0f;
    private Vector2 spawnPoint;
    private double patience = 0;


    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        MoveTo(transform.position); // to avoid unit stacking upon spawn

        // logic
        if (aiEnabled)
        {
            StartCoroutine(HandleAILogic());
            spawnPoint = transform.position;
            //spriteRenderer.color = Color.red;
        }


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
        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            ResolveOverlap();
            return;
        }

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
        ResolveOverlap();
    }

    void ResolveOverlap()
    {
        float checkRadius = 0.1f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius);

        foreach (var hit in hits)
        {
            if (hit.gameObject != this.gameObject && hit.GetComponent<UnitBasic>() != null)
            {
                // nudge unit away
                Vector3 awayDir = (transform.position - hit.transform.position).normalized;
                Vector3 offset = awayDir * 0.05f;
                Vector3 newPos = transform.position + offset;

                MoveTo(newPos);
                //transform.position = newPos;
                return;
            }
        }
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
                Stop();
                break;
            }
            else if (other == null) // later zlacze te dwie rzeczy as it should be, this but a temporary fix
            {
                healthSystem building = hit.GetComponent<healthSystem>();
                if (building != null && building.faction != this.faction)
                  {  
                    // attack
                    int attackCount = 2; // Total number of attack animations
                    int randomIndex = Random.Range(1, attackCount + 1);

                    animator.SetInteger("attackIndex", randomIndex);
                    animator.SetTrigger("attackTrigger");

                    Stop(); // Stop when attacking
                    building.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                    Stop();
                    break;
                }
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

    // on completion of calculating the path, before movement begins
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

    IEnumerator HandleAILogic()
    {
        while (true)
        {
            if (Time.time % 90 > 80)
                spawnPoint = new Vector2(30, -50);
            if (enemy_target == null)
            {
                Idle();
                FindEnemy();
            }
            else
            {
                if (Vector2.Distance(enemy_target.transform.position, transform.position) > detectionRange)
                {
                    MoveTo(transform.position);
                    enemy_target = null;
                }
                else if (Vector2.Distance(enemy_target.transform.position, transform.position) > attackRange)
                {
                    ChaseEnemy();
                }
                else
                {
                    MoveTo(transform.position); // basically stand and fight
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    void Idle()
    {
        if (Vector2.Distance(spawnPoint, transform.position) > spawnDesiredProximity)
            MoveTo(spawnPoint);
        else
            IdleMovement();
    }

    void IdleMovement(float radius = 1.5f)
    {
        if (Time.time >= patience)
        {
            Vector2 originalPosition = transform.position;
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector2 targetPosition = originalPosition + offset;

            MoveTo(targetPosition);

            float cooldown = Random.Range(1.5f, 3f);
            patience = Time.time + cooldown;
        }
    }

    void FindEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            UnitBasic other = hit.GetComponent<UnitBasic>();
            if (other != null && other.faction != this.faction)
            {
                enemy_target = other;
                Debug.Log("found enemy");
            }
        }
    }
    private void ChaseEnemy()
    {
        if (enemy_target != null)
        {
            MoveTo(enemy_target.transform.position);
        }
    }

    // For debug purposes: draw attack range in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
