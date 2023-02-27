using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_SelectionManager : MonoBehaviour
{
    public static TD_SelectionManager instance;

    public TD_Selectable CurrentSelected;
    public List<TD_Selectable> Selectables;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            if (Selectables == null) Selectables = new();
            DontDestroyOnLoad(instance);
        }
        else Destroy(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EventManager.OnTowerDeselect += DeselectAll;
        //EventManager.OnCancel += DeselectAll;
    }

    private void OnDisable()
    {
        EventManager.OnTowerDeselect -= DeselectAll;
    }

    public void SetSelected(TD_Selectable tD_Selectable)
    {
        DeselectCurrent();
        // TODO: handle building vs enemy
        if (CurrentSelected) CurrentSelected.OnBuildingDeselected();
        CurrentSelected = tD_Selectable;
    }

    public void DeselectCurrent()
    {
        if (CurrentSelected) CurrentSelected.OnBuildingDeselected();
    }

    public void DeselectAll()
    {
        CurrentSelected?.OnBuildingDeselected();
        CurrentSelected = null;
        //List<TD_Selectable> allSelected = Selectables.FindAll((selectable) => { return selectable.IsSelected; });
        foreach (var selectable in Selectables)
        {
            // TODO: handle building vs enemy
            if (selectable.IsSelected) selectable.OnBuildingDeselected();
        };
    }
}
