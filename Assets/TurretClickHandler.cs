using UnityEngine;

public class TurretClickHandler : MonoBehaviour
{
    [SerializeField] private Turret _parentTurret;
    [SerializeField] private ShopManager shopManager;

    void Start()
    {
        // Get reference to the parent Turret component
        _parentTurret = GetComponentInParent<Turret>();
        shopManager = FindObjectOfType<ShopManager>();
    }

    void OnMouseDown()
    {
        if (_parentTurret != null && shopManager != null)
        {
            Debug.Log("Turret clicked through click hitbox!");
            shopManager.ShowUpgradeSellPanel(_parentTurret); // pass this turret ref to the ShopManager
        }
    }
}
