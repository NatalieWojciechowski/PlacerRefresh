using System;
using UnityEngine;

public class TD_Projectile : MonoBehaviour
{
    public GameObject myParent;
    public GameObject myTarget;
    public float projectileSpeed = 2f;
    public float projectileDamage = 1f;
    private float _spawnTime = 0;
    private float _maxLifetime = 10f;
    private Animator mAnimator;
    private float totalTravelNeeded;
    public AnimationCurve curve;
    public float curveStartScale = 1f;
    public float curveFinalScale;

    protected virtual void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (_spawnTime == 0) return;
 
        //// Currently this should happen once the enemy dies
        //if (myTarget == null)
        //    Debug.LogError("NO target");
       
        if (Time.time - _spawnTime > _maxLifetime || !myTarget || transform.position.y < 0f) ExpireProjectile();
        // Target may be destroyed
        if (myTarget) { 
            AdjustProjectileOnPath();
            if (mAnimator)
            {

                float graphValue = curve.Evaluate((Time.time - _spawnTime) / (Mathf.Sqrt(Vector3.Distance(myParent.transform.position, myTarget.transform.position))));
                //Debug.Log(aniYPos);
                //Vector3 aniPos = transform.position + new Vector3(0, aniYPos, 0);
                transform.Translate(new Vector3(0, graphValue, 0));
            }
            }
    }

    protected virtual void LateUpdate()
    {

    }

    private void AdjustProjectileOnPath()
    {
        Vector3 lerpPos = GetLerpPosition(myParent.transform.position, myTarget.transform.position);
        transform.position = lerpPos;
        float angleDiff = Vector3.Dot(transform.forward, myTarget.transform.forward);
        if (Math.Abs(angleDiff) > 0.15f) transform.LookAt(myTarget.transform.position);
    }

    private Vector3 GetLerpPosition(Vector3 parentPos, Vector3 targetPos)
    {
        // TODO: Consider passing the total length from spawn time for the animator in "TravelTime" param
        float currentTimeOnPath = Time.time - _spawnTime;
        ////float aniYPos = curve. keys.GetValue( // .Evaluate(Time.time);
        return Vector3.Lerp(parentPos, targetPos, currentTimeOnPath / TotalTimeForPath(parentPos, targetPos));
    }

    private float TotalTimeForPath(Vector3 sPosition, Vector3 ePosition)
    {
        float pathLength = Vector3.Distance(sPosition, ePosition);
        return pathLength / projectileSpeed;
    }

    private void ExpireProjectile()
    {
        Destroy(this.gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == myTarget) {
            myTarget.GetComponent<TD_Enemy>().TakeDamage(projectileDamage);
            if (mAnimator) mAnimator.PlayInFixedTime("Impact");
            ExpireProjectile();
        }
    }

    public virtual void InitProjectile(TD_Building tD_Building, TD_Enemy buildingTarget)
    {
        //curve = new AnimationCurve(new Keyframe(0,0), new Keyframe(0.5f, 1));
        //curve.preWrapMode = WrapMode.PingPong;
        //curve.postWrapMode = WrapMode.PingPong;
        _spawnTime = Time.time;
        myParent = tD_Building.gameObject;
        myTarget = buildingTarget.gameObject;
        mAnimator = GetComponent<Animator>();
        totalTravelNeeded = TotalTimeForPath(transform.position, myTarget.transform.position);

        // Initial Velocity
        Vector3 iVelocity = transform.forward * projectileSpeed;
        float totalDistance = Vector3.Distance(myTarget.transform.position, transform.position); 
        double timeOfFlight = (2 * Math.Sin(totalDistance)) / Physics.gravity.y;
        float launchAngle = 30f;

        //double range =  (iVelocity*iVelocity * Math.Sin(2 * totalDistance)) / Physics.gravity.y;
        //float range = iVelocity * Mathf.Cos(launchAngle);
        

        // If we are using fixed velocity, determine time.

        // if we are using fixed time, determine velocity.


        //Debug.Log($"{totalTravelNeeded} VS {timeOfFlight}");
        if (mAnimator)
        {
            curveFinalScale = totalTravelNeeded;
            mAnimator.SetFloat("TravelTime", totalTravelNeeded);
            mAnimator.Play("ProjectileArc");
            //object p = mAnimator.GetCurrentAnimatorClipInfo(0);
            //foreach (AnimatorClipInfo clip in p )
            //{
            //    UnityEditor.AnimationUtility
            //    clip.
            //}
        }
    }
}