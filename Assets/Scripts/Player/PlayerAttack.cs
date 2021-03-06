using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform bullet;


    CharacterStats stats;
    Animator animator;
    GameManager gameManager;
    GameObject bulletStartPosition;
    GameObject bulletsContainer;
    float delayTimer;
    Transform nearestEnemy;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        bulletStartPosition = GameObject.FindGameObjectWithTag("PlayerBulletSpawnPosition");
        bulletsContainer = GameObject.FindGameObjectWithTag("BulletsContainer");
        ResetTimer();
    }

    void FixedUpdate()
    {
        delayTimer -= Time.deltaTime;
        if (delayTimer < 0) StartAttack();
    }



    void StartAttack()
    {
        nearestEnemy = FindTheNearestEnemy();
        if (nearestEnemy && !animator.GetBool("walking"))
        {
            if (!nearestEnemy.GetComponent<HealthSystem>().isDead)
            {
                transform.LookAt(nearestEnemy);
                animator.SetTrigger("attacking");
            }
            else
            {
                animator.ResetTrigger("attacking");
            }
        }
    }

    public void Attack()
    {
        if (nearestEnemy && !animator.GetBool("walking"))
        {
            Transform newBuller = Instantiate(bullet, bulletStartPosition.transform.position, Quaternion.identity, bulletsContainer.transform);
            newBuller.GetComponent<BulletMovement>().SetData(nearestEnemy, stats.minDamage, stats.maxDamage);
            ResetTimer();
        }        
    }


    private void ResetTimer()
    {
        delayTimer = stats.delayBetweenAttacks;
    }

    Transform FindTheNearestEnemy()
    {
        if (gameManager.enemies.Count == 0) return null;
        GameObject nearestEnemy = gameManager.enemies[0];
        foreach (var enemy in gameManager.enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, nearestEnemy.transform.position))
            {
                if (!enemy.GetComponent<HealthSystem>().isDead)
                {
                    nearestEnemy = enemy;
                }
            }
        }
        return nearestEnemy.transform;
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
    }
}
