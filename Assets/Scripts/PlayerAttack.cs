using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, IAction
{
    [SerializeField]
    float attackCooldownTimeInSeconds = 0.5f;
    WaitForSeconds waitAttackCooldown;

    [SerializeField]
    Vector2 attackOffSet;
    [SerializeField]
    float attackRange;
    Vector2 attackPos;

    [SerializeField]
    LayerMask whatIsDamageable;
    [SerializeField]
    int damage = 1;

    private void Start()
    {
        waitAttackCooldown = new WaitForSeconds(attackCooldownTimeInSeconds);
    }

    private void Update()
    {

    }

    IEnumerator AttackCooldown()
    {
        yield return waitAttackCooldown;
    }

    public void PerformAction(bool flip)
    {
        if (flip)
        {
            attackPos = new Vector2(-attackOffSet.x, attackOffSet.y) + (Vector2)transform.position;
        }
        else
        {
            attackPos = attackOffSet + (Vector2)transform.position;
        }

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos, attackRange, whatIsDamageable);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Health>().TakeDamage(damage);
        }
        Debug.Log("Atacou!");
    }

    public void PerformAnimation()
    {
        Debug.Log("Nada por enquanto");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }

}
