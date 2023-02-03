using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour
{
    //public TD_Building ParentBuilding;
    private int maxLevel = 0;
    private int currentLevel = 0;
    public GameObject tierPrefab;
    [SerializeField] Transform tierContainer;
    
    private List<GameObject> indicatorTiers;

    [SerializeField] Color ActiveShade;
    [SerializeField] Color InactiveShade;

    TD_Building parentbuilding;

    // Start is called before the first frame update
    void Start()
    {
        if (indicatorTiers == null) indicatorTiers = new();
        if (ActiveShade == null) ActiveShade = Color.white;
        if (InactiveShade == null) InactiveShade = Color.grey;
    }

    private void OnEnable()
    {
        if (indicatorTiers == null) indicatorTiers = new();
        AssignChildTiers();
        RefreshLevels();
    }

    private void AssignChildTiers()
    {
        int childCount = tierContainer.childCount;
        indicatorTiers.Clear();
        for (int i = 0; i < childCount; i++)
        {
            indicatorTiers.Add(tierContainer.GetChild(i)?.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(-30, -20, 0);
        //transform.LookAt(Vector3.Cross(Camera.main.transform.position, Camera.main.transform.up));
    }

    public void InitIndicator(TD_Building tD_Building)
    {
        parentbuilding = tD_Building;
        if (!parentbuilding) return;
        AssignChildTiers();
        UpdateMax(parentbuilding.GetStats().MaxLevel);
        //PopulateIndicators();
        UpdateCurrent(parentbuilding.GetStats().Level);
    }

    private void PopulateIndicators()
    {
        //int tierIndex = 0;
        //indicatorTiers.RemoveAll( (tier) => { return tier == null; } );
        //int needAddCount = maxLevel - indicatorTiers.Count;
        //for (int i = 0; i < needAddCount; i++)
        //{
        //    AddIndicator();
        //}

        //foreach (GameObject tier in indicatorTiers)
        //{
        //    if (tier == null) indicatorTiers.add = AddIndicator();
        //    tierIndex++;
        //}
    }

    private void UpdateMax(int _maxLevel)
    {
        //if (maxLevel == _maxLevel) return;
        maxLevel = _maxLevel;
        int childCount = tierContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            indicatorTiers[i]?.SetActive(i < maxLevel);
        }
        //tierContainer.GetChild()


        //indicatorTiers.RemoveAll((tier) => { return tier == null; });


        //if (indicatorTiers.Count < _maxLevel)
        //{
        //    int countDiff = maxLevel - indicatorTiers.Count;
        //    for (int i = 0; i < countDiff; i++)
        //    {
        //        AddIndicator();
        //        // TODO: probably just want to have the max level already created with the objects and enable/disable the sprite renderers 
        //    }
        //}
        //else if (indicatorTiers.Count > _maxLevel) Debug.Log("More indicators than levels");
    }

    //private GameObject AddIndicator(bool addToList = true)
    //{
    //    Vector3 offset = new Vector3(0, 0.25f * indicatorTiers.Count, 0);
    //    GameObject newIndicator = Instantiate(tierPrefab, tierContainer);
    //    newIndicator.transform.Translate(offset);
    //    if (addToList && newIndicator != null) indicatorTiers.Add(newIndicator);
    //    return newIndicator;
    //}

    private void UpdateCurrent(int _currentLevel)
    {
        currentLevel = _currentLevel;
        if (indicatorTiers.Count < currentLevel) return;
        for (int i = 0; i < maxLevel; i++)
        {
            if (indicatorTiers[i] == null) break;
            if (currentLevel > i) indicatorTiers[i].GetComponent<Image>().color = ActiveShade;
            else indicatorTiers[i].GetComponent<Image>().color = InactiveShade;
            //indicators[i].gameObject?.SetActive(currentLevel > i);
        }
    }

    public void RefreshLevels()
    {
        if (parentbuilding == null) return;
        if (indicatorTiers == null || indicatorTiers[0] == null) InitIndicator(parentbuilding);
        BuildingData parentData = parentbuilding.GetStats();
        if (maxLevel != parentData.MaxLevel) UpdateMax(parentData.MaxLevel);
        if (currentLevel != parentData.Level) UpdateCurrent(parentData.Level);
        //transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(-30, -20, 0);
    }
}
