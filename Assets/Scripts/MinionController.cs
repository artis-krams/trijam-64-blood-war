using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MinionController : MonoBehaviour
{
    public MinionType minionType;
    public float Range = 1;
    public float Damage = 10;
    public float AttackSpeed = 1;
    public float MovementSpeed = 1;
    public string TeamTag;
    public float MaxHealth = 100;
    public AudioClip AttackSound;
    public AudioClip DeathSound;
    public Animator Animator;

    private Slider HealthSlider;
    private bool isAttacking = false;
    private GameObject target;
    private float lastHitTimestamp;
    private float health;

    public AudioSource AudioData { get; private set; }

    void Start()
    {
        AudioData = GetComponent<AudioSource>();
        HealthSlider = gameObject.GetComponentInChildren<Slider>();
        HealthSlider.value = HealthSlider.maxValue = health = MaxHealth;

        Animator.SetFloat("MoveSpeed", MovementSpeed);
        Animator.SetFloat("AttackSpeed", AttackSpeed);
    }


    void Update()
    {
        lastHitTimestamp += Time.deltaTime;

        if (!isAttacking)
        {
            transform.Translate((TeamTag == "RedTeam" ? Vector3.right : Vector3.left) * Time.deltaTime * MovementSpeed);
        }
        else if (lastHitTimestamp > AttackSpeed)
        {
            AudioData.PlayOneShot(AttackSound);
            target.gameObject.SendMessage("ApplyDamage", Damage);
            lastHitTimestamp = 0;
        }
        else if (target == null || target.gameObject == null)
        {
            isAttacking = false;
            Debug.Log("not attackning");
        }

        Animator.SetBool("IsAttacking", isAttacking);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != TeamTag && collision.gameObject.tag != "Enviornment")
        {
            collision.gameObject.SendMessage("ApplyDamage", Damage);
            isAttacking = true;
            target = collision.gameObject;
        }
    }
    public void ApplyDamage(float damage)
    {
        health -= damage;
        HealthSlider.value = health;
        Debug.Log(HealthSlider.value + " | " + health);
        if (health < 0)
        {
            if (gameObject != null)
            {
                Animator.SetBool("Dying", true);
                AudioData.PlayOneShot(DeathSound);
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
