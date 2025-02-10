using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Lifeform : MonoBehaviour
{
    public int HP;
    public Character Character;
    public UnityEvent OnDeath;
    public GameObject DeathRagdoll;


    bool dead;
    public void Hurt()
    {
        Hurt(1);
    }

    public void Hurt(int damage)
    {
        if (dead)
            return;

        HP -= damage;

        if (HP <= 0)
            Death();

    }

    public void Stun(float stunDamage)
    {
        if (dead)
            return;

        if (Character != null)
            Character.Stun(stunDamage);
    }

    public virtual void Death()
    {
        if (dead)
            return;
        
        OnDeath?.Invoke();

        dead = true;

        if (DeathRagdoll != null)
        {
            GameObject go = Instantiate(DeathRagdoll);

            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;

            InitRagdoll initRagdoll = go.GetComponent<InitRagdoll>();
            initRagdoll.TryMatchBones(transform);

            GameManager.Instance.RemoveCharacter(Character);

            Destroy(gameObject);
        }
    }
}
