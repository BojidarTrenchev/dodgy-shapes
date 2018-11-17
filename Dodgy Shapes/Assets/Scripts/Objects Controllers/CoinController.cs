
public class CoinController : HelperObjectController
{
    public int sum;

    public override void Start()
    {
        base.Start();
        hasARateChance = true;
    }

    public void AddSum()
    {
        values.AddCoins(sum);
    }
}
