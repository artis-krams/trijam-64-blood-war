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
    public Material laserMaterial;

    private Slider HealthSlider;
    private bool isAttacking = false;
    private GameObject lockedTarget;
    private float lastHitTimestamp;
    private float health;
    private List<GameObject> targetQueue;

    public AudioSource AudioData { get; private set; }

    void Start()
    {
        targetQueue = new List<GameObject>();
        AudioData = GetComponent<AudioSource>();
        HealthSlider = gameObject.GetComponentInChildren<Slider>();
        if (HealthSlider != null)
        {
            HealthSlider.maxValue = health = MaxHealth;
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

    void OnPostRender()
    {
        if (lockedTarget == null || lockedTarget.transform == null)
        {
            return;
        }
        Debug.Log(lockedTarget);

        if (!laserMaterial)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        Debug.Log(laserMaterial);

        GL.PushMatrix();
        laserMaterial.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(gameObject.transform.position);
        GL.Vertex(new Vector3(lockedTarget.transform.position.x / Screen.width, lockedTarget.transform.position.y / Screen.height, 0));
        GL.End();

        GL.PopMatrix();
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
        }
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
