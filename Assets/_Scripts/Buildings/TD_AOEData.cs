using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_AOEData")]
public class TD_AOEData : ScriptableObject
{
    #region Display
    /// <summary>
    /// The base object representing the AOE itself
    /// </summary>
    public GameObject aoePrefab;
    /// <summary>
    /// The G.O. that should be spawned on each "pulse" of the AOE 
    /// </summary>
    public GameObject effectPrefab;
    public Animator aoeAnimator;
    public ParticleSystem particleSystem;
    public string category;
    #endregion

    #region Behavior
    public float pulseDelay;
    /// <summary>
    /// This may be positive or negative for heal/dmg
    /// </summary>
    public float pulseEffectAmount;
    public float aoeRange;
    #endregion

    #region Interactions
    public bool RemoveOnContact = false;
    public float maxLifetime;
    #endregion
}