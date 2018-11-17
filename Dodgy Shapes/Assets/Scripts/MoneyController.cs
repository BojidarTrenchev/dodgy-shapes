using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    public Text money;
    public Text addedMoneyValue;
    public Animator moneyAnim;
    public int coinsToAdd;
    public AudioClip coinSound;

    private bool beginChanging;
    private int currentSum;
    private int addend;
    private int finalSum;

    public void Start()
    {
        money.text = PlayerInfo.info.allCoins.ToString();
    }

    public void ChangeMoney(int value)
    {
        moneyAnim.SetTrigger("Change");
        currentSum = PlayerInfo.info.allCoins;
        PlayerInfo.info.allCoins += value;
        if (value > 0)
        {
            addedMoneyValue.text = "+ " + value.ToString();
        }
        else
        {
            addedMoneyValue.text = value.ToString();
        }

        beginChanging = true;
        addend = (int)(value * Time.deltaTime);
        finalSum = PlayerInfo.info.allCoins;

    }

    public void Update()
    {
        if (beginChanging)
        {
            currentSum += addend;
            if (!MainAuidoManager.mainAudio.IsPlayingSound)
            {
                MainAuidoManager.mainAudio.PlaySound(coinSound);
            }

            if (addend < 0)
            {
                if (currentSum <= finalSum)
                {
                    money.text = finalSum.ToString();
                    beginChanging = false;
                }
                else
                {
                    money.text = currentSum.ToString();
                }
            }
            else if(addend == 0)
            {
                money.text = finalSum.ToString();
                beginChanging = false;
            }
            else
            {
                if (currentSum >= finalSum)
                {
                    money.text = finalSum.ToString();
                    beginChanging = false;
                }
                else
                {
                    money.text = currentSum.ToString();
                }
            }

        }
    }

    public static int RoundSumWithZeroes(int number, int zeroes)
    {
        int powedNumber = (int)Mathf.Pow(10, zeroes);
        if (number > powedNumber)
        {
            number /= powedNumber;
            number *= powedNumber;
        }

        return number;
    }
}
