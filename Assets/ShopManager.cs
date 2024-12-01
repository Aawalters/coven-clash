using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject shopMenu;
    public GameObject turretPrefab;
    public TurretPlacer TurretPlacer;
    public int playerMoney = 100; // Starting money
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button turretPurchaceButton0;
    [SerializeField] private Button turretPurchaceButton1;

    private GameObject turretToPlace = null;
    [SerializeField] private List<GameObject> turrets;

    void Start()
    {
        UpdateMoneyUI();
        shopMenu.SetActive(false);
        openButton.onClick.AddListener(() => OpenShop());
        closeButton.onClick.AddListener(() => CloseShop());
        turretPurchaceButton0.onClick.AddListener(() => SelectTurretToPlace(0));
        //turretPurchaceButton1.onClick.AddListener(() => SelectTurretToPlace(1)); //TODO: ADD
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
    }

    public void SelectTurretToPlace(int index)
    {
        turretToPlace = turrets[index];
        if (playerMoney >= 50) // TODO: change to turret to place cost 
        {
            
            CloseShop();
            //enter turret placing mode 
            TurretPlacer.EnterTurretPlacingMode(turretToPlace); // Enter turret placing mode
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void UpdateMoneyUI()
    {
        moneyText.text = $"{playerMoney}g";
    }

    public void DeductMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyUI();
    }
}
