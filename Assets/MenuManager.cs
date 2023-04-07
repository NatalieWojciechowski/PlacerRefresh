using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public class MenuItem
    {
        public Button MenuButton;
        public GameObject menuPanel;

        public void TogglePanel()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }
    }

    [SerializeField]
    public List<MenuItem> menuItems;

    // Start is called before the first frame update
    void Start()
    {
        menuItems = new();
    }

    private void OnEnable()
    {
        if (menuItems == null) menuItems = new();
        foreach (MenuItem menuItem in menuItems)
        {
            menuItem.MenuButton.onClick.AddListener(menuItem.TogglePanel);
        }
    }

    private void OnDisable()
    {
        foreach (MenuItem menuItem in menuItems)
        {
            menuItem.MenuButton.onClick.RemoveListener(menuItem.TogglePanel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
