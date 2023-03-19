using System;
using System.Collections;
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

    public Vector3 StartPosition { get; private set; }

    private float totalTravelNeeded;
    public AnimationCurve projectileArc;
    public float curveStartScale = 1f;
    public float curveFinalScale;

    enum ProjectileState
    {
        Loaded,
        Fire,
        Moving,
        Impact,
        Expire
    }
    private ProjectileState projectileState;

    private Coroutine stateTransition;
    [SerializeField]
    private GameObject impactEffects;
   
    [SerializeField] private AudioClip hitClip;

    protected virtual void Start()
    {
        mAnimator = GetComponent<Animator>();
        StartPosition = transform.position;
    }
    private void OnEnable()
    {

    }

    protected virtual void Update()
    {
        if (_spawnTime == 0 || projectileState == ProjectileState.Expire) return;
 
        //// Currently this should happen once the enemy dies
        //if (myTarget == null)
        //    Debug.LogError("NO target");
       
        if (Time.time - _spawnTime > _maxLifetime || !myTarget || transform.position.y < 0f) TryChangeState(ProjectileState.Expire);
        // Target may be destroyed
        if (myTarget) { 
            AdjustProjectileOnPath();
            if (mAnimator)
            {

                float graphValue = projectileArc.Evaluate((Time.time - _spawnTime) / (Mathf.Sqrt(Vector3.Distance(StartPosition, myTarget.transform.position))));
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
        Vector3 lerpPos = GetLerpPosition(StartPosition, myTarget.transform.position);
        transform.position = lerpPos;
        float angleDiff = Vector3.Dot(transform.forward, myTarget.transform.forward);
        if (Math.Abs(angleDiff) > 0.15f) transform.LookAt(myTarget.transform.position);
    }

    private Vector3 GetLerpPosition(Vector3 fromPos, Vector3 targetPos)
    {
        // TODO: Consider passing the total length from spawn time for the animator in "TravelTime" param
        float currentTimeOnPath = Time.time - _spawnTime;
        ////float aniYPos = curve. keys.GetValue( // .Evaluate(Time.time);
        return Vector3.Lerp(fromPos, targetPos, currentTimeOnPath / TotalTimeForPath(fromPos, targetPos));
    }

    private float TotalTimeForPath(Vector3 sPosition, Vector3 ePosition)
    {
        float pathLength = Vector3.Distance(sPosition, ePosition);
        return pathLength / projectileSpeed;
    }

    private void Expire()
    {
        Destroy(this.gameObject);
    }
    private void SafeTransition(ProjectileState nextState, float delay)
    {
        // Setup transition back to moving
        if (stateTransition != null) { StopCoroutine(stateTransition); stateTransition = null; }
        stateTransition = StartCoroutine(AllowStateTime(nextState, delay));
    }
    private IEnumerator AllowStateTime(ProjectileState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("Now allow state change");
        TryChangeState(nextState);
    }

    private void TryChangeState(ProjectileState toState = ProjectileState.Loaded)
    {
        if (mAnimator) mAnimator.SetInteger("animation", (int)toState);
        projectileState = toState;

        switch (toState)
        {
            case ProjectileState.Expire:
            this.GetComponent<Collider>().enabled = false;
            Expire();
            break;
            
            case ProjectileState.Impact:
            this.GetComponent<Collider>().enabled = false;
            if (mAnimator) mAnimator.PlayInFixedTime("Impact");
            if (impactEffects) impactEffects.SetActive(true);
            PlayImpactSound();
            SafeTransition(ProjectileState.Expire, 0.125f);
            break;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
            return;

        if (collision.gameObject == myTarget) {
            myTarget.GetComponent<TD_Enemy>().TakeDamage(projectileDamage);
        }
        TryChangeState(ProjectileState.Impact);
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
        TryChangeState(ProjectileState.Fire);
        SafeTransition(ProjectileState.Moving, 0.125f);
    }
    
    public void PlayImpactSound()
    {
        if (hitClip) TD_AudioManager.instance.PlayClip(hitClip, transform.position);
    }
}