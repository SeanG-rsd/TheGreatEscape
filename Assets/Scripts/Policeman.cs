using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Policeman : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject target;
    private bool canMove;

    private bool targetInRange;
    [SerializeField] private int damage;
    [SerializeField] private float timeBeforeCanShoot;
    private float currentTimeBeforeShoot;

    [SerializeField] private int maxHealth;
    private int health;
    [SerializeField] private RectTransform healthRectMask;
    private float healthRectOriginalSize;

    [SerializeField] private GameObject targetIndicator;

    [SerializeField] private Fade healthBarFade;

    [SerializeField] private bool isCar;

    [SerializeField] private GameObject pillPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float chanceOfDroppingPill;

    [SerializeField] private Animator carAnimator;
    [SerializeField] private Animator policemanAnimator;

    public static Action<GameObject> OnEnemyDeath = delegate { };
    void Start()
    {
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        health = maxHealth;
        healthRectOriginalSize = healthRectMask.sizeDelta.x;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (canMove)
        { 
            agent.SetDestination(target.transform.position);
            Vector2 angle = FindAngle();

            if (isCar)
            {
                carAnimator.SetFloat("Horizontal", angle.x);
                carAnimator.SetFloat("Vertical", angle.y);
            }
            else
            {
                policemanAnimator.SetFloat("Horizontal", angle.x);
                policemanAnimator.SetFloat("Vertical", angle.y);
            }
        }
    }

    private Vector2 FindAngle()
    {
        float changeX = target.transform.position.x - transform.position.x;
        float changeY = target.transform.position.y - transform.position.y;
        Vector2 angle = new (changeX, changeY);
        angle.Normalize();
        
        return angle;
    }

    private void Update()
    {
        if (health <= 0 && canMove)
        {
            float which = UnityEngine.Random.value;
            if (which <= chanceOfDroppingPill)
            {
                Instantiate(pillPrefab, transform.position, Quaternion.identity);
            }

            canMove = false;
            OnEnemyDeath?.Invoke(gameObject);
        }

        if (targetInRange)
        {
            currentTimeBeforeShoot -= Time.deltaTime;
            if (currentTimeBeforeShoot < 0)
            {
                Shoot();
                currentTimeBeforeShoot = timeBeforeCanShoot;
            }
        }
    }

    public void Activate(GameObject target)
    {
        this.target = target;
        canMove = true;
    }

    public void ChangeHealth(int value)
    {
        int last = health;
        health = Mathf.Clamp(health + value, 0, maxHealth);
        healthRectMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthRectOriginalSize * (health / (float)maxHealth));

        if (last > health)
        {
            healthBarFade.Activate(1.5f);
        }
    }

    public void SetAsTarget()
    {
        targetIndicator.SetActive(true);
    }

    public void RemoveAsTarget()
    {
        targetIndicator.SetActive(false);
    }

    private void Shoot()
    {
        target.GetComponent<MrFrog>().ChangeHealth(-damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Frog"))
        {
            currentTimeBeforeShoot = timeBeforeCanShoot;
            targetInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Frog"))
        {
            targetInRange = false;
        }
    }
}
