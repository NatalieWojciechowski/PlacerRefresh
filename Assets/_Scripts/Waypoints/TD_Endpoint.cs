using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Endpoint : MonoBehaviour
{
    public Collider EndRegion;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if (!EndRegion) EndRegion = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        TD_Enemy enemy = collision.gameObject.GetComponent<TD_Enemy>();
        if (enemy)
        {
            EventManager.EnemyPassedCore(enemy.DmgToCore);
            Destroy(collision.gameObject);
        }
    }
    // In case they dont destroy? 
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("collision stay");
        TD_Enemy enemy = collision.gameObject.GetComponent<TD_Enemy>();
        if (enemy)
        {
            EventManager.EnemyPassedCore(enemy.DmgToCore);
            Destroy(collision.gameObject);
        }
    }
}
