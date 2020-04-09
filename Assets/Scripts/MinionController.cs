using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MinionController : MonoBehaviour
{
    public MinionType minionType;
    public float RangeMultiplier = 1;
    public float Damage = 10;
    public float AttackSpeed = 1;
    public float MovementSpeed = 1;
    public string TeamTag;
    public float MaxHealth = 100;
    public AudioClip AttackSound;
    public AudioClip DeathSound;
    public Animator Animator;
    public AudioSource AudioData { get; private set; }
    public GameObject laserOrigin;

    private Slider HealthSlider;
    private bool isAttacking = false;
    private GameObject lockedTarget;
    private float lastHitTimestamp;
    private float health;
    private List<GameObject> targetQueue;
    private LineRenderer line;
    private Vector3 laserOriginVector;

    void Start()
    {
        targetQueue = new List<GameObject>();
        AudioData = GetComponent<AudioSource>();
        line = GetComponent<LineRenderer>();
        if (laserOrigin == null)
        {
            laserOriginVector = new Vector3(transform.position.x, transform.position.y, -1);
        }
        else if (line != null)
        {
            laserOriginVector = laserOrigin.transform.position;
        }

        HealthSlider = gameObject.GetComponentInChildren<Slider>();
        if (HealthSlider != null)
        {
            HealthSlider.value = HealthSlider.maxValue = health = MaxHealth;
            Debug.Log(health);
        }

        if (Animator != null)
        {
            Animator.SetFloat("MoveSpeed", MovementSpeed);
            Animator.SetFloat("AttackSpeed", AttackSpeed);
        }
    }
    void Update()
    {
        lastHitTimestamp += Time.deltaTime;

        if (!isAttacking && minionType != MinionType.ranged)
        {
            transform.Translate((TeamTag == "RedTeam" ? Vector3.right : Vector3.left) * Time.deltaTime * MovementSpeed);
        }
        else if (lockedTarget == null || lockedTarget.gameObject == null)
        {
            var newTarget = targetQueue.FirstOrDefault(t => t != null);
            if (newTarget != null)
            {
                lockedTarget = newTarget;
            }
            else
            {
                isAttacking = false;
            }
        }
        else if (lastHitTimestamp > AttackSpeed)
        {
            if (AttackSound != null)
                AudioData.PlayOneShot(AttackSound);

            lockedTarget.gameObject.SendMessage("ApplyDamage", Damage);
            lastHitTimestamp = 0;
        }

        if (Animator != null)
        {
            Animator.SetBool("IsAttacking", isAttacking);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (minionType == MinionType.melle)
        {
            Attack(collision.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (minionType == MinionType.ranged)
        {
            Attack(other.gameObject);
            StartCoroutine("FireLaser");
        }
    }
    IEnumerator FireLaser()
    {

        line.enabled = true;

        while (isAttacking)
        {
            line.SetPosition(0, laserOriginVector);
            line.SetPosition(1, lockedTarget.transform.position);

            yield return null;
        }

        line.enabled = false;
    }

    private void Attack(GameObject target)
    {
        if (target == null || target.tag == TeamTag || target.tag == "Enviornment")
        {
            return;
        }

        if (lockedTarget == null)
        {
            lockedTarget = target;
        }
        else
        {
            targetQueue.Add(target);
        }

        target.SendMessage("ApplyDamage", Damage);
        isAttacking = true;
    }

    public void ApplyDamage(float damage)
    {
        if (HealthSlider == null)
        {
            return;
        }

        health -= damage;
        HealthSlider.value = health;

        if (health < 0)
        {
            if (gameObject != null && Animator != null)
            {
                Animator.SetBool("Dying", true);
                if (DeathSound != null)
                {
                    AudioData.PlayOneShot(DeathSound);
                }
                Destroy(gameObject, 0.3f);
            }
        }
    }
}

public enum MinionType
{
    melle = 0,
    ranged = 1
}
