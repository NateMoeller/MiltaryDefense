using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour {
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }

    }


    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    public int Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
        }
    }

    private void Start(){
        priceTxt.text = "<color=lime>$</color>" + price;
    }
}
