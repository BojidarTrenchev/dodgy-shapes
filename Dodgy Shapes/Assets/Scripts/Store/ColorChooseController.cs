using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorChooseController : MonoBehaviour
{
    public int index;
    public int price = 1000;

    private StoreManager store;
    private Text priceText;
    public void Start()
    {
        store = GameObject.FindGameObjectWithTag("Store").GetComponent<StoreManager>();
        priceText = GetComponentInChildren<Text>();

        if (store.isColorBought(index))
        {
            priceText.gameObject.SetActive(false);
        }
        else
        {
            priceText.text = price.ToString();
        }
    }

    public void OnClick()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        bool isBought = store.BuyOrActivateColor(index, price);
        if (isBought)
        {
            priceText.DOFade(0, 0.3f);
        }
    }
}
