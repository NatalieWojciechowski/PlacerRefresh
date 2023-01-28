using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_AOEData")]
public class TD_AOEData : ScriptableObject
{
    #region Display
    public GameObject spawnPrefab;
    public Animator aoeAnimator;
    public ParticleSystem particleSystem;
    public string category;
    #endregion

    #region Behavior
    public float pulseDelay;
    public float aoeRange;
    #endregion

    #region Interactions
    public bool RemoveOnContact = false;
    public float maxLifetime;
    #endregion
}