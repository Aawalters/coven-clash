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
    public int playerMoney = 200; // starting money
    //shop panel stuff
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text shopMoneyText;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button turretPurchaceButton0;
    [SerializeField] private Button turretPurchaceButton1;

    // upgrade panel stuff
    public GameObject upgradeSellPanel;
    [SerializeField] private TMP_Text turretUpgradeCostText;
    [SerializeField] private TMP_Text turretSellPriceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closePanelButton;
    private Turret selectedTurret;

    private GameObject turretToPlace = null;
    [SerializeField] private List<GameObject> turrets;

    // for SFX
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

        audioManager = AudioManager.Instance;
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
        Turret actualTurret = turretToPlace.GetComponent<Turret>();
        if (CanAfford(actualTurret.purchaseCost)) // TODO: change to turret to place cost 
        {
            audioManager.PlaySFX(audioManager.turretBuy);
            DeductMoney(actualTurret.purchaseCost);
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
        shopMoneyText.text = $"{playerMoney}g";
    }

    public void DeductMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyUI();
    }

    public bool CanAfford(int amount)
    {
        return playerMoney >= amount;
    }

    public void ShowUpgradeSellPanel(Turret turret)
    {
        if (upgradeSellPanel.activeSelf || turret == null || !turret.isSelectable) return;
        Debug.Log("attempting to show sell panel");
        // Set the selected turret
        selectedTurret = turret;

        turretUpgradeCostText.text = $"{turret.upgradeCost}g";
        turretSellPriceText.text = $"{turret.sellPrice}g";

        // Show the upgrade/sell panel
        upgradeSellPanel.SetActive(true);
        if (!upgradeSellPanel) Debug.Log("bro it's null");
        Debug.Log("shouldve been set to active");
        Debug.Log($"is the panel active in hierarchy? {upgradeSellPanel.activeInHierarchy}");

        //TODO: if the turret can't be upgraded, disable the button for upgrading

        // Position the panel next to the turret
        PositionPanelNextToTurret(turret);
    }

    private void PositionPanelNextToTurret(Turret turret)
    {
        // get the turret's world position
        Vector3 turretWorldPos = turret.transform.position;

        // convert the world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(turretWorldPos);

        // apply an smol offset
        float panelOffsetX = 100f;
        float panelOffsetY = 50f;

        // adjust the position of the panel
        screenPos.x += panelOffsetX;
        screenPos.y += panelOffsetY;

        // clamp the position to ensure the panel stays on screen!!
        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width - upgradeSellPanel.GetComponent<RectTransform>().rect.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height - upgradeSellPanel.GetComponent<RectTransform>().rect.height);

        // then set the position in screen space
        upgradeSellPanel.GetComponent<RectTransform>().position = screenPos;
    }

    public void CloseUpgradeSellPanel()
    {
        // close the panel using the close button
        audioManager.PlaySFX(audioManager.click);
        upgradeSellPanel.SetActive(false);
    }

    private void UpgradeTurret()
    {
        if (selectedTurret != null && CanAfford(selectedTurret.upgradeCost) 
            && selectedTurret.CanUpgrade())
        {
            selectedTurret.UpgradeTurret();
            audioManager.PlaySFX(audioManager.turretUpgrade);
            // deduct the upgrade cost
            DeductMoney(selectedTurret.upgradeCost);

            // update panel UI
            ShowUpgradeSellPanel(selectedTurret); 
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
            // sell turret & refund the player
            AddMoney(selectedTurret.sellPrice);
            audioManager.PlaySFX(audioManager.turretSell);
            // destroy the turret
            Destroy(selectedTurret.gameObject);

            // close the upgrade/sell panel, 
            // cus the turret doesn't exist anymore lol
            CloseUpgradeSellPanel();
        }
    }
}
