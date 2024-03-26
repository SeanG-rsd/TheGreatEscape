using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrFrog : MonoBehaviour
{
    [Header("---Movement---")]
    [SerializeField] private float movementSpeed;
    private float currentMovementSpeed;
    private bool canMove;

    [Header("---Attack---")]
    [SerializeField] private int damage;
    private int currentDamage;

    [SerializeField] private float attackCooldown;
    private float currentCooldown;
    private GameObject currentTarget;

    private List<GameObject> possibleTargets = new List<GameObject>();

    [Header("---Rage---")]
    [SerializeField] private float maxRageTime;
    private float currentRageTime;
    [SerializeField] private RectTransform rageRectMask;
    [SerializeField] private GameObject rageBar;
    private float rageRectOriginalSize;

    [Header("---Health---")]
    [SerializeField] private int maxHealth;
    private int health;
    [SerializeField] private RectTransform healthRectMask;
    private float healthRectOriginalSize;

    [Header("---Sound---")]
    [SerializeField] private AudioSource mrFrogSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip pillSound;

    [SerializeField] private Animator animator;

    public static Action OnPlayerDeath = delegate { };

    // Start is called before the first frame update
    private void Awake()
    {
        rageRectOriginalSize = rageRectMask.sizeDelta.x;
        healthRectOriginalSize = healthRectMask.sizeDelta.x;
        health = maxHealth;
        currentMovementSpeed = movementSpeed;
        currentDamage = damage;

        Policeman.OnEnemyDeath += HandleEnemyDeath;
        GameManager.OnStartGame += HandleStartGame;
    }

    private void OnDestroy()
    {
        Policeman.OnEnemyDeath -= HandleEnemyDeath;
        GameManager.OnStartGame -= HandleStartGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && possibleTargets.Count > 0 && currentCooldown <= 0)
            {
                GameObject target = possibleTargets[0];
                transform.position = target.transform.position;
                target.GetComponent<Policeman>().ChangeHealth(-currentDamage);
                currentCooldown = attackCooldown;
                mrFrogSource.PlayOneShot(attackSound);
                animator.SetTrigger("Attack");
            }

            if (currentRageTime > 0)
            {
                if (!rageBar.activeSelf) { rageBar.SetActive(true); }
                currentRageTime = Mathf.Clamp(currentRageTime - Time.deltaTime, 0, maxRageTime);
                rageRectMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rageRectOriginalSize * (currentRageTime / maxRageTime));
            }
            else
            {
                if (rageBar.activeSelf)
                {
                    rageBar.SetActive(false);
                    currentMovementSpeed = movementSpeed;
                    currentDamage = damage;
                }
            }

            if (health <= 0) // dead
            {
                OnPlayerDeath?.Invoke();
                mrFrogSource.PlayOneShot(deathSound);
                canMove = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);

            Vector3 movement = new(horizontal, vertical, 0);
            animator.SetFloat("Speed", movement.magnitude);
            movement.Normalize();
            movement *= currentMovementSpeed;


            transform.position += movement;
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        possibleTargets.Remove(enemy);
        if (possibleTargets.Count > 0 && currentTarget == enemy)
        {
            currentTarget = possibleTargets[0];
            currentTarget.GetComponent<Policeman>().SetAsTarget();
        }
        Destroy(enemy);
    }

    private void HandleStartGame()
    {
        canMove = true;
    }

    public void ChangeHealth(int value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);

        if (value < 0)
        {
            mrFrogSource.PlayOneShot(hurtSound);
        }

        healthRectMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthRectOriginalSize * (health / (float)maxHealth));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            possibleTargets.Add(other.gameObject);

            if (possibleTargets.Count == 1 && currentTarget == null)
            {
                other.gameObject.GetComponent<Policeman>().SetAsTarget();
                currentTarget = other.gameObject;
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Pill"))
        {
            if (currentRageTime + other.gameObject.GetComponent<Pill>().GetTime() > maxRageTime)
            {
                // special rage
            }

            mrFrogSource.PlayOneShot(pillSound);

            currentRageTime = Mathf.Clamp(currentRageTime + other.gameObject.GetComponent<Pill>().GetTime(), 0, maxRageTime);
            ChangeHealth(other.gameObject.GetComponent<Pill>().GetHealth());
            currentMovementSpeed = movementSpeed * other.gameObject.GetComponent<Pill>().GetSpeed();
            currentDamage = damage + other.gameObject.GetComponent<Pill>().GetDamage();
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {       
        if (collision.gameObject == currentTarget)
        {
            currentTarget.GetComponent<Policeman>().RemoveAsTarget();
            currentTarget = null;
        }
        
        possibleTargets.Remove(collision.gameObject);

        if (currentTarget == null && possibleTargets.Count > 0)
        {
            possibleTargets[0].GetComponent<Policeman>().SetAsTarget();
            currentTarget = possibleTargets[0];
        }
    }
}
