using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Endpoint : MonoBehaviour
{

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
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
}
