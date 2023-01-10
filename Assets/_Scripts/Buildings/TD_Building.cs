using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Building : MonoBehaviour
{
    [SerializeField]
    TD_BuildingData _buildingData;
    
    public TD_Building(TD_BuildingData bData)
    {
        SetStats(bData);
    }

    public void SetStats(TD_BuildingData bData)
    {
        _buildingData = bData;
        PieceBehaviour pieceBehaviour = GetComponent<PieceBehaviour>();
        if (pieceBehaviour)
        {
            pieceBehaviour.Name = bData.displayName;
            pieceBehaviour.Icon = bData.icon;
            pieceBehaviour.Description = bData.description;
            pieceBehaviour.Category = bData.category;
        }
        // callbacks / Powerup animations/ etc
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_buildingData == null) _buildingData = GetComponent<TD_BuildingData>();

        Debug.Log("Building Data: " + _buildingData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
