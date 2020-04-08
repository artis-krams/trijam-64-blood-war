using UnityEngine;

public class CastleController : MonoBehaviour
{
    public string TeamTag;
    public int MaxHealth = 100;
    public GameObject Spawn;
    public GameObject[] Minions;

    private float health;
    private float lastFrameTimestamp = 0;
    private float spawnSpeedInSeconds = 1;

    void Start()
    {
        health = MaxHealth;
        Instantiate(Minions[0], Spawn.transform.position, Spawn.transform.rotation);
    }

    void Update()
    {
        lastFrameTimestamp += Time.deltaTime;
        if (lastFrameTimestamp > spawnSpeedInSeconds)
        {
            lastFrameTimestamp = 0;
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
