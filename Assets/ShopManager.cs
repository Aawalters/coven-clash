using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopMenu;
    public GameObject turretPrefab;
    public GridManager gridManager;
    public int playerMoney = 100; // Starting money
    public Text moneyText;

    private GameObject turretToPlace = null;

    void Start()
    {
        UpdateMoneyUI();
        shopMenu.SetActive(false);
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
    }

    public void SelectTurretToPlace(GameObject turret)
    {
        if (playerMoney >= 50) // Example cost of turret
        {
            turretToPlace = turretPrefab;
            CloseShop();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void UpdateMoneyUI()
    {
        //moneyText.text = $"{playerMoney}g";
    }

    public void DeductMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyUI();
    }
}
