using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_EnemyBuff : MonoBehaviour
{
    enum EnemyBuffType
    {
        Stats,
        Effect,
        Spawn,
    }

    EnemyBuffType BuffType = EnemyBuffType.Stats;

    Coroutine corEffects;
    Coroutine corCycleCheck;
    TD_Enemy buffedEnemy;
    private float startTime;
    public float activeTime = 0f;
    public float maxDuration = 1f;

    public GameObject BuffSpawnPrefab;
    public string DisplayName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyBuff(TD_Enemy tD_Enemy)
    {
        buffedEnemy = tD_Enemy;
        if (corEffects != null) StopCoroutine(corEffects);
        // In case the enemy has been destroyed, else apply effects
        if (buffedEnemy != null) corEffects = StartCoroutine(BuffEffects(maxDuration));
    }

    IEnumerator BuffEffects(float delay)
    {
        // Create the effects when the buff is first Started;
        startTime = Time.time;
        Debug.Log("Buff STARTING");
        yield return new WaitForSeconds(delay);
        Debug.Log("Buff ENDING");
        GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TD_Enemy enemyHit = collision.gameObject.GetComponent<TD_Enemy>();
        if (enemyHit) ApplyBuff(enemyHit);
    }
}
