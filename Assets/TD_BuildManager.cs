using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TD_BuildManager : MonoBehaviour
{
    enum BuildState
    {
        Idle,
        Blueprint,
        Placing,
        Bulldozing,
    }

    #region Connections
    public static TD_BuildManager current;
    public TD_BuildToolbar toolbarCtrl;
    public Physics2DRaycaster builderRaycaster;

    [SerializeField] private List<TD_Building> pieces;
    public List<TD_Building> Pieces { get => pieces; }
    #endregion

    #region Settings
    public LayerMask RaycastLayer = 1 << 0;
    public bool SnapBuild = true;
    [SerializeField] private Vector3 RaycastOffsetPosition;
    [SerializeField] private Vector3 buildOffset;
    #endregion

    #region Member
    private BuildState buildState;
    private Vector3 lastHitPos;
    private List<GameObject> BuiltBuildings;
    private GameObject previewObj;
    public Transform TowersParent;
    #endregion

    #region Lifecycle

    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
        if (BuiltBuildings == null) BuiltBuildings = new();
        if (!builderRaycaster) Camera.main.GetComponent<Physics2DRaycaster>();
        buildState = BuildState.Idle;
        if (!TowersParent) TowersParent = transform;
    }

    private void OnEnable()
    {
        PlayerControlsManager.PlayerCancel += OnPlayerCancel;
        PlayerControlsManager.PlayerAccept += OnPlayerAccept;
        //EventManager.OnTowerPlace += onBuildingPlaceAccept;
    }
    private void OnDisable()
    {
        PlayerControlsManager.PlayerCancel -= OnPlayerCancel;
        // TODO: Consider making an event from the build manager for the player controls to trigger the accept / cancel from there & broadcast
        PlayerControlsManager.PlayerAccept -= OnPlayerAccept;
        EventManager.OnTowerPlace -= onBuildingPlaceAccept;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("PIECES: " + pieces.ToString());

        //isValidBuildingPlacement();

        if (buildState == BuildState.Blueprint)
        {
            if (!UpdateLastHitIfValid()) {
                // force it to the last valid position
                previewObj.transform.position = lastHitPos;
            }
        }
        else if (buildState == BuildState.Placing)
        {

        }        
    }

    #endregion

    #region Placement Validation

    /// <summary>
    /// Building requires a buildable surface AND for no other building to be nearby / in collider.
    /// </summary>
    /// <returns></returns>
    private bool UpdateLastHitIfValid()
    {
        bool validPlacement = false;
        // "On Buildable Surface
        RaycastHit hit = builderRawRaycast();

        if (!hit.collider || !hit.collider.CompareTag("BuildableSurface") || HasObstruction(hit)) return false;

        validPlacement = CheckBuildPosition(hit);
        //if (hit.collider != null)
        //{
        //    //Hit something, print the tag of the object
        //    //Debug.Log("Hitting: " + hit.collider.tag);
        //    // TODO: Update preview rotation  
        //    //if (previewObj) previewObj.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
        //}

        ///

        //if (hit.point != null)
        //{
        //    validPlacement = !HasObstruction(hit);
        //    if (validPlacement) lastHitPos = hit.point;
        //}
        if (validPlacement) SetToHitOrCenter(hit);
        return validPlacement;
    }

    /// <summary>
    ///  Currently only checks for nearby towers; but we may wish to check for other items here.
    /// </summary>
    /// <param name="_hit"></param>
    /// <returns></returns>
    private bool HasObstruction(RaycastHit _hit)
    {
        bool hasNearbyTower = false;
        Vector3 direction = (_hit.point - Camera.main.transform.position).normalized;
        RaycastHit[] possibleTargets = Physics.RaycastAll(Camera.main.transform.position, direction, 400f);
        foreach (RaycastHit _target in possibleTargets)
        {
            TD_Building hitBuilding = _target.collider.gameObject.GetComponent<TD_Building>();
            if (hitBuilding && hitBuilding.IsRunning) hasNearbyTower = true;
        }
        return hasNearbyTower;
    }

    private void SetToHitOrCenter(RaycastHit _hit)
    {
        if (SnapBuild)
        {
            Vector3 _topCenterHit = _hit.collider.bounds.center + new Vector3(0, _hit.collider.bounds.extents.y, 0) + buildOffset;
            lastHitPos = _topCenterHit;
        } else
        {
            lastHitPos = _hit.point + buildOffset;
        }
        previewObj.transform.SetPositionAndRotation(lastHitPos, Quaternion.identity);
        ////Vector3 _topCenterHit = _hit.collider.ClosestPoint(_offsetHitPoint);
        //Vector3 _topCenterHit = _hit.collider.bounds.center + new Vector3(0, _hit.collider.bounds.extents.y, 0) + buildOffset;

        //previewObj.transform.SetPositionAndRotation(_topCenterHit, Quaternion.identity);
        //lastHitPos = _topCenterHit;

        // TODO: Sockets

        // Could create these as the tiles load potentially
        // Have collider disable once something placed nearby in it? 
        // Then only colliding sockets are buildable;
    }
    #endregion


    #region Placement Helpers
    /// <summary>
    /// Represents the direction the user is facing as the "builder" with their mouse.
    /// </summary>
    /// <returns></returns>
    private Ray builderRay()
    {
        Vector3 builderMouseToScreenPos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Camera.main.nearClipPlane) + RaycastOffsetPosition;
        Ray mouseWorldPosition = Camera.main.ScreenPointToRay(builderMouseToScreenPos);
        return mouseWorldPosition;
    }

    private RaycastHit builderRawRaycast()
    {
        RaycastHit hit = new RaycastHit();
        Ray bRay = builderRay();
        Physics.Raycast(bRay, out hit, 400f, RaycastLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(bRay.origin, bRay.direction * 400f, Color.red);
        return hit;
    }


    /// <summary>
    /// Will update last hit 
    /// </summary>
    /// <param name="_hit"></param>
    public bool CheckBuildPosition(RaycastHit _hit)
    {
        if (!_hit.collider || !previewObj || !_hit.collider.CompareTag("BuildableSurface")) return false;
        Vector3 direction = (_hit.point - Camera.main.transform.position).normalized;
        Debug.DrawRay(_hit.point, -direction, Color.yellow);
        // TODO: validate distance from last point, remove once out of range
        return _hit.point != null;
    }
    #endregion

    #region Player Actions
    public static GameObject StartPlacement(GameObject buildPrefab)
    {
        if (current.previewObj != null) Destroy(current.previewObj);
        current.previewObj = Instantiate(buildPrefab, current.transform);
        current.buildState = BuildState.Blueprint;
        current.previewObj.transform.position = current.lastHitPos;
        EventManager.current.TowerBlueprint(current.previewObj.GetComponent<TD_Building>());
        return current.previewObj;
    }

    private void FinishPlacement()
    {
        buildState = BuildState.Placing;
        previewObj.transform.SetParent(TowersParent);
        previewObj.transform.SetPositionAndRotation(lastHitPos, Quaternion.identity);
        previewObj.GetComponent<TD_Building>()?.OnPlacementConfirm(previewObj.transform);
        EventManager.current.TowerPlaced(previewObj.GetComponent<TD_Building>());
        BuiltBuildings.Add(previewObj);
        previewObj = null;
    }

    protected void OnPlayerCancel(object source, EventArgs eventArgs)
    {
        Debug.Log("OnPlayerCancel" + source);
        buildState = BuildState.Idle;
        Destroy(previewObj);
    }

    protected void OnPlayerAccept(object sender, EventArgs e)
    {
        if (previewObj)
            onBuildingPlaceAccept(previewObj?.GetComponent<TD_Building>());
    }

    private void onBuildingPlaceAccept(TD_Building tD_Building)
    {
        if (previewObj && tD_Building.BuildingUUID == previewObj.GetComponent<TD_Building>().BuildingUUID
            && UpdateLastHitIfValid())
            FinishPlacement();
    }
    #endregion

    #region Toolbar
    public void UpdateBuildToolbar()
    {
        toolbarCtrl?.ToolbarButtons(pieces);
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (previewObj && lastHitPos != null) Gizmos.DrawSphere(lastHitPos, 1f);
    }
    #endregion

}
