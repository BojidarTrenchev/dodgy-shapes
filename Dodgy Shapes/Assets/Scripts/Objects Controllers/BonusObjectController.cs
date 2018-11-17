using UnityEngine;
using System.Collections;

public class BonusObjectController : HelperObjectController
{
    public int multiplier; //if this is 0 then the additional value is added to the score
    public int additionalValueToCoins; // if this is 0 then the score is multiplied
    [Range(0, 1)]
    public float additionPercentValue;

    public AudioClip bonusSound;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (multiplier == 0)
            {
                values.AddCoins(additionalValueToCoins);
            }
            else
            {
                values.MultiplyCoins(multiplier);
            }

            if (additionPercentValue != 0)
            {
                values.AddCoins((int)((additionPercentValue) * Values.currentCoins));
            }

            MainAuidoManager.mainAudio.PlaySound(bonusSound, 0.4f);
        }
    }
}
