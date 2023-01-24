using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TD_BuildManager : MonoBehaviour
{
    public static TD_BuildManager current;
    public TD_BuildToolbar toolbarCtrl;

    [SerializeField]
    private List<TD_Building> pieces;

    public Physics2DRaycaster builderRaycaster;
    private Vector3 lastHitPos;

    public List<TD_Building> Pieces { get => pieces; }
    public LayerMask RaycastLayer = 1 << 0;

    private GameObject previewObj;
    public Transform TowersParent;

    enum BuildState
    {
        Idle,
        Blueprint,
        Placing,
        Bulldozing,
    }

    private BuildState buildState;
    [SerializeField] private Vector3 RaycastOffsetPosition;
    [SerializeField] private Vector3 buildOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
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
            if (!isValidBuildingPlacement()) {
                previewObj.transform.position = lastHitPos + buildOffset;
            }
        }
        else if (buildState == BuildState.Placing)
        {

        }        
    }

    private bool isValidBuildingPlacement()
    {
        bool validPlacement = false;
        RaycastHit hit = builderRaycastTarget();
        if (hit.point != null && hit.collider && hit.collider.CompareTag("BuildableSurface"))
        {
            validPlacement = true;
            lastHitPos = hit.point;
            Debug.Log("New Point: " + hit.point.ToString());
        }
        return validPlacement;
    }

    private RaycastHit builderRaycastTarget()
    {
        RaycastHit hit = new RaycastHit();
        Ray bRay = builderRay();
        Physics.Raycast(bRay, out hit, 400f, RaycastLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(bRay.origin, bRay.direction * 400f, Color.red);
        HandleBuildRayHit(hit);
        if (hit.collider != null)
        {
            //Hit something, print the tag of the object
            //Debug.Log("Hitting: " + hit.collider.tag);
            // TODO: Update preview rotation  
            //if (previewObj) previewObj.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
        }
        return hit;
    }

    private Ray builderRay()
    {
        Vector3 builderMouseToScreenPos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition;
        //Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(builderMouseToScreenPos);
        Debug.Log(builderMouseToScreenPos + Input.mousePosition);
        Ray mouseWorldPosition = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(mouseWorldPosition.ToString());
        return mouseWorldPosition;
        //Ray testRay = new(Camera.main.gameObject.transform.position, mouseWorldPosition.origin);

        //Debug.DrawRay(testRay.origin, Camera.main.transform.forward, Color.grey);

        //Debug.DrawRay(testRay.origin, testRay.direction * 100f, Color.green);
        ////return new Ray(Camera.main.gameObject.transform.position, Camera.main.ScreenToWorldPoint(builderMouseToScreenPos));
        ////return Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition);
        ////return Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition);
        ////return Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition);
        ////return Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition);
        //Vector3 cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -10.0f));
        ////Ray adjustedRay = Camera.main.ScreenPointToRay(mouseWorldPosition + RaycastOffsetPosition);
        //Vector3 offsetMouse = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f) + RaycastOffsetPosition;
        //Ray adjustedRay = Camera.main.ScreenPointToRay(offsetMouse) ;
        //adjustedRay.origin = cameraCenter;
        //Debug.DrawRay(adjustedRay.origin, adjustedRay.direction *100f, Color.blue);
        //return adjustedRay;
    }

    private Ray ScreenCenterRay()
    {
        return Camera.main.ScreenPointToRay(new Vector2(Screen.height / 2, Screen.width / 2));
    }

    public void HandleBuildRayHit(RaycastHit _hit)
    {
        if (!_hit.collider || !previewObj || !_hit.collider.CompareTag("BuildableSurface")) return;
        // See if we already have another piece here? 
        Vector3 direction = (_hit.point - Camera.main.transform.position).normalized;
        Debug.DrawRay(_hit.point, -direction, Color.yellow);
        AllTargetsOnRay(_hit);
 
        // TODO: validate distance from last point, remove once out of range
        UpdatePreviewObjPos(_hit);
    }

    private void AllTargetsOnRay(RaycastHit _hit)
    {
        Vector3 direction = (_hit.point - Camera.main.transform.position).normalized;
        RaycastHit secondHit = new();
        //Grab objects in direction of clicked object (including the one clicked)
        RaycastHit[] possibleTargets = Physics.RaycastAll(Camera.main.transform.position, direction, 400f);
        //foreach (RaycastHit _target in possibleTargets)
        //{
        //    if (_target.collider?.gameObject.CompareTag("Tower") 
        //}
        Debug.Log($"There are {possibleTargets.Length} possible targets ahead");
        Debug.Log($"You hit {_hit.transform.name}");
        //Set destination, and set to move
    }

    private void UpdatePreviewObjPos(RaycastHit _hit)
    {
        //if (CenterSnap)
        Vector3 _offsetHitPoint = _hit.point + buildOffset;


        // Get contact point with collider => _hit
        Vector3 _placeUpward = Vector3.Cross(_offsetHitPoint, _hit.collider.gameObject.transform.position);
        Debug.DrawLine(_placeUpward, _placeUpward * 1f);

        Vector3 _topCenterHit = _hit.collider.ClosestPoint(_offsetHitPoint);
        //Debug.Log(_hit.collider.transform.forward);
        Debug.Log(_topCenterHit);
        previewObj.transform.SetPositionAndRotation(_topCenterHit, Quaternion.identity);
        lastHitPos = _topCenterHit;
    }

    public static GameObject StartPlacement(GameObject buildPrefab)
    {
        if (current.previewObj != null) Destroy(current.previewObj);

        current.previewObj = Instantiate(buildPrefab, current.transform);
        current.buildState = BuildState.Blueprint;
        EventManager.current.TowerBlueprint(current.previewObj.GetComponent<TD_Building>());
        return current.previewObj;
    }

    private void FinishPlacement()
    {
        buildState = BuildState.Placing;
        previewObj.transform.SetPositionAndRotation(lastHitPos, Quaternion.identity);
        previewObj.transform.SetParent(TowersParent);
        previewObj.GetComponent<TD_Building>()?.OnPlacementConfirm(previewObj.transform);
        EventManager.current.TowerPlaced(previewObj.GetComponent<TD_Building>());
        previewObj = null;
    }

    public void UpdateBuildToolbar()
    {
        toolbarCtrl?.ToolbarButtons(pieces);
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
            && isValidBuildingPlacement())
            FinishPlacement();
    }

    private void OnDrawGizmos()
    {
        if (previewObj && lastHitPos != null) Gizmos.DrawSphere(lastHitPos, 1f);
    }
}
