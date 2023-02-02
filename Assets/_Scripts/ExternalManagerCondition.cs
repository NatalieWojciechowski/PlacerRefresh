using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalManagerCondition : ConditionBehaviour
{
    public static bool ShowGizmos = true;

    public override bool CheckForPlacement()
    {
        bool canAfford = false;
        if (Piece != null) return false;
        TD_Building tBuilding = Piece.GetComponent<TD_Building>();
        if (tBuilding && TD_GameManager.current.CanAfford(tBuilding.GetStats().PurchaseCost))
            canAfford = true;
        return canAfford;
    }

    private void OnDrawGizmosSelected()
    {
        if (!ShowGizmos) return;

        if (Piece == null) return;

        Gizmos.matrix = Piece.transform.localToWorldMatrix;
        Gizmos.color = Color.cyan / 2f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
