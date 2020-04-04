using UnityEngine;

public class CastleController : MonoBehaviour
{
    public string TeamTag;
    public int MaxHealth = 100;
    public Sprite DeathSprite;
    public AudioSource AudioData;
    public GameObject Spawn;
    public GameObject[] Minions;

    private float health;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        AudioData = GetComponent<AudioSource>();
        health = MaxHealth;
        Instantiate(Minions[0], Spawn.transform.position, Spawn.transform.rotation);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMinion(MinionType.melle);
        }
    }

    private void SpawnMinion(MinionType minionType)
    {
        Debug.Log("spawning " + Minions[0].name);
        Instantiate(Minions[0], Spawn.transform.position, Spawn.transform.rotation);
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            // todo: victroy condition
        }
    }
}
