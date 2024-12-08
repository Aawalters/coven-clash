using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public GameObject shopMenu;
    public GameObject turretPrefab;
    public TurretPlacer TurretPlacer;
    public int playerMoney = 100; // Starting money
    //shop panel stuff
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button turretPurchaceButton0;
    [SerializeField] private Button turretPurchaceButton1;

    //upgrade panel stuff
    public GameObject upgradeSellPanel; // The panel to show upgrade/sell options
    [SerializeField] private TMP_Text turretUpgradeCostText;
    [SerializeField] private TMP_Text turretSellPriceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closePanelButton;
    private Turret selectedTurret;

    private GameObject turretToPlace = null;
    [SerializeField] private List<GameObject> turrets;
    AudioManager audioManager;

    void Start()
    {
        UpdateMoneyUI();
        shopMenu.SetActive(false);
        openButton.onClick.AddListener(() => OpenShop());
        closeButton.onClick.AddListener(() => CloseShop());
        turretPurchaceButton0.onClick.AddListener(() => SelectTurretToPlace(0));
        //turretPurchaceButton1.onClick.AddListener(() => SelectTurretToPlace(1)); //TODO: ADD
    
        upgradeSellPanel.SetActive(false);
        upgradeButton.onClick.AddListener(() => UpgradeTurret());
        sellButton.onClick.AddListener(() => SellTurret());
        closePanelButton.onClick.AddListener(() => CloseUpgradeSellPanel());

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void OpenShop()
    {
        audioManager.PlaySFX(audioManager.click);
        shopMenu.SetActive(true);
    }

    public void CloseShop()
    {
        audioManager.PlaySFX(audioManager.click);
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
        audioManager.PlaySFX(audioManager.turretBuy);
        UpdateMoneyUI();
    }

    public void ShowUpgradeSellPanel(Turret turret)
    {
        Debug.Log("attempting to show sell panel");
        // Set the selected turret
        selectedTurret = turret;

        turretUpgradeCostText.text = $"{turret.upgradeCost}g";
        turretSellPriceText.text = $"{turret.sellPrice}g";

        // Show the upgrade/sell panel
        upgradeSellPanel.SetActive(true);
        if (!upgradeSellPanel) Debug.Log("bro it's null");
        Debug.Log("shouldve been set to active");
        Debug.Log($"Is the panel active in hierarchy? {upgradeSellPanel.activeInHierarchy}");

        // Position the panel next to the turret
        PositionPanelNextToTurret(turret);
    }

    private void PositionPanelNextToTurret(Turret turret)
    {
        // Get the turret's world position
        Vector3 turretWorldPos = turret.transform.position;

        // Convert the world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(turretWorldPos);

        // Apply an offset (customize as needed)
        float panelOffsetX = 100f;
        float panelOffsetY = 50f;

        // Adjust the position of the panel
        screenPos.x += panelOffsetX;
        screenPos.y += panelOffsetY;

        // Clamp the position to ensure the panel stays on screen
        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width - upgradeSellPanel.GetComponent<RectTransform>().rect.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height - upgradeSellPanel.GetComponent<RectTransform>().rect.height);

        // Set the position in screen space
        upgradeSellPanel.GetComponent<RectTransform>().position = screenPos;
    }

    public void CloseUpgradeSellPanel()
    {
        // Close the panel if clicked outside or after an action
        audioManager.PlaySFX(audioManager.click);
        upgradeSellPanel.SetActive(false);
    }

    // Implement the upgrade functionality
    private void UpgradeTurret()
    {
        if (selectedTurret != null)//&& CanAfford(selectedTurret.upgradeCost)) and CAN UPGRADE
        {
            selectedTurret.UpgradeTurret();
            audioManager.PlaySFX(audioManager.turretUpgrade);
            // Deduct the upgrade cost
            DeductMoney(selectedTurret.upgradeCost);

            // Update panel UI
            ShowUpgradeSellPanel(selectedTurret); // Re-update with new stats
        }
        else
        {
            Debug.Log("Not enough money or turret doesn't exist.");
        }
    }

    // Implement the sell functionality
    private void SellTurret()
    {
        if (selectedTurret != null)
        {
            // Sell turret and refund the player
            //AddMoney(selectedTurret.sellPrice);
            audioManager.PlaySFX(audioManager.turretSell);
            // Destroy the turret
            Destroy(selectedTurret.gameObject);

            // Close the upgrade/sell panel
            CloseUpgradeSellPanel();
        }
    }
}
