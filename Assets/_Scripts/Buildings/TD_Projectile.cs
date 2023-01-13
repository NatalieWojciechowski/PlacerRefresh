﻿using System;
using UnityEngine;

internal class TD_Projectile : MonoBehaviour
{
    public GameObject myParent;
    public GameObject myTarget;
    public float projectileSpeed = 2f;
    public float projectileDamage = 1f;
    private float _spawnTime = 0;
    private float _maxLifetime = 3f;

    private void Start()
    {

    }

    private void Update()
    {
        if (_spawnTime == 0) return;
 
        //// Currently this should happen once the enemy dies
        //if (myTarget == null)
        //    Debug.LogError("NO target");
       
        if (Time.time - _spawnTime > _maxLifetime) ExpireProjectile();
        // Target may be destroyed
        if (myTarget)
        {
            Vector3 lerpPos = GetLerpPosition(myParent.transform.position, myTarget.transform.position);
            transform.position = lerpPos;
            float angleDiff = Vector3.Dot(transform.forward, myTarget.transform.forward);
            if (Math.Abs(angleDiff) > 0.15f) transform.LookAt(myTarget.transform.position);
        }
        else ExpireProjectile();

    }

    private Vector3 GetLerpPosition(Vector3 parentPos, Vector3 targetPos)
    {
        float pathLength = Vector3.Distance(parentPos, targetPos);
        float totalTimeForPath = pathLength / projectileSpeed;
        float currentTimeOnPath = Time.time - _spawnTime;
        return Vector3.Lerp(parentPos, targetPos, currentTimeOnPath / totalTimeForPath);
    }

    private void ExpireProjectile()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == myTarget) {
            myTarget.GetComponent<TD_Enemy>().TakeDamage(projectileDamage);
            ExpireProjectile();
        }
    }

    internal void InitProjectile(TD_Building tD_Building, TD_Enemy buildingTarget)
    {
        _spawnTime = Time.time;
        myParent = tD_Building.gameObject;
        myTarget = buildingTarget.gameObject;
    }
}