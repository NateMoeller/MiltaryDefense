using UnityEngine;
using System.Collections;

public class TowerData : MonoBehaviour {

    public GameObject bullet;
    public float fireRate;
    public int sellPrice;
    public int upgradePrice;
    public string upgradeName;
    [SerializeField]
    private Sprite upgradeSprite;
    public int level;
    public string type;

    // TODO: upgrade the attributes of the towers to make them better!
    public void UpgradeTower() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = upgradeSprite;
        level++;

        if(type == "MiniGun") {
            UpgradeMiniGun();
        }
        else if(type == "ElectricGun") {
            UpgradeElectricGun();
        }
        else if(type == "Flamethrower") {
            UpgradeFlameThrower();
        }
    }



    private void UpgradeMiniGun() {
        sellPrice = 15;
        fireRate = .15f;
    }

    private void UpgradeElectricGun() {
        sellPrice = 25;
        fireRate = .35f;
    }

    private void UpgradeFlameThrower() {
        sellPrice = 35;
        fireRate = .15f;
    }

    private void UpgradeRockets() {
        sellPrice = 25;
        fireRate = 1.5f;
    }

}
