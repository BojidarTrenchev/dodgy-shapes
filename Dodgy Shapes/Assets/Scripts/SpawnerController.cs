using UnityEngine;
using System.Collections.Generic;

public class SpawnerController : MonoBehaviour
{
    public float offset = -20f;
    public float initialSpawnY = 25f;
    public int countInitialObjects = 5;
    public float minDistanceY = 10f;
    public float maxDistanceY = 20f;

    private PlayerController playerController;
    private Transform playerTransform;

    public ObjectController[] allObjects;
    public ObjectController[] allCoins;
    private List<ObjectController> objectsWithRate;
    private List<ObjectController> objectsWithNoRate;

    private Rigidbody2D rb;
    private Transform transformParent;
    private Vector2 newObjPos;
    private Vector2 newCoinPos;
    private ObjectController newObject;
    private ObjectController coin;

    private int sumOfAllObjectsWithChance;
    private int chanceForObjectsWithoutRate;
    private float pointToSpawnX;
    private float additionalYDistance;

    //at the odd indeces are the min rates and at the even indeces are the max rates
    private List<string> objectsNotToSpawnName;

    private string playerName;
    public void Start()
    {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); 
        this.pointToSpawnX = Mathf.Abs(this.playerController.positionX);
        this.transformParent = this.GetComponent<Transform>().parent;
        this.playerTransform = this.playerController.GetComponent<Transform>();

        this.newObjPos = new Vector2(this.pointToSpawnX, initialSpawnY);

        this.objectsWithRate = new List<ObjectController>();
        this.objectsWithNoRate = new List<ObjectController>();
        this.objectsNotToSpawnName = new List<string>();

        playerName = playerController.GetComponentInChildren<SpriteRenderer>().sprite.name;

        DetermineRateValues(allObjects);
        DetermineRateValues(allCoins, false);

        if (PlayerInfo.info.mysteryTokens >= PlayerInfo.info.maxMysteryTokens 
            || PlayerInfo.info.lastUnlockedShapeIndex == PlayerInfo.info.allPlayerShapes.Length - 1)
        {
            DontSpawnObject("HelperMysteryToken");
        }


        InstantiateNewObjects(this.countInitialObjects);
    }

    public void FixedUpdate()
    {
        var currentPosition = rb.position;
        currentPosition.y = this.offset + this.playerTransform.position.y;
        rb.MovePosition(currentPosition);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        string otherTag = other.tag;
        if (otherTag == "CloseEnemiesDestroyer" || otherTag == "ParticlesCollider")
        {
            return;
        }

        if (!(otherTag == "Coin"))
        {
            InstantiateNewObjects(1);
        }

        Destroy(other.gameObject);
    }

    public void DontSpawnObject(string nameOfObject)
    {
        int objectsWithRateCount = objectsWithRate.Count;
        for (int i = 0; i < objectsWithRateCount; i++)
        {
            if (objectsWithRate[i].name == nameOfObject)
            {
                objectsWithRate[i].initialChance = objectsWithRate[i].chance;
                objectsNotToSpawnName.Add(nameOfObject);
                chanceForObjectsWithoutRate += objectsWithRate[i].chance;
                objectsWithRate[i].chance = 0;
                return;
            }
        }

        Debug.LogError("Wrong game object name! The specified name doesn't match with the names of the objects with rate");
    }

    public void RestoreObjectToSpawn(string nameOfObject)
    {
        if (!objectsNotToSpawnName.Contains(nameOfObject))
        {
            Debug.LogError("There are no objects to restore or the specified name is incorrect!");
        }
        else
        {
            int objectsWithRateCount = objectsWithRate.Count;
            for (int i = 0; i < objectsWithRateCount; i++)
            {
                if (objectsWithRate[i].name == nameOfObject)
                {
                    int index = -1;

                    index = objectsNotToSpawnName.IndexOf(nameOfObject);
                    objectsNotToSpawnName.RemoveAt(index);
                    objectsWithRate[i].chance = objectsWithRate[i].initialChance;
                    chanceForObjectsWithoutRate -= objectsWithRate[i].chance;
                    return;
                }
            }
        }
    }

    private void InstantiateNewObjects(int countOfObjects)
    {
        float yDistance = Random.Range(this.minDistanceY, this.maxDistanceY);
        int isNegative = Random.Range(0, 2) <= 0 ? -1 : 1;

        for (int i = 0; i < countOfObjects; i++)
        {
            ChooseObjToSpawn();

            ChooseCoinToSpawn();

            newObjPos.y += yDistance + this.additionalYDistance;
            newObjPos.x *= isNegative;

            yDistance = Random.Range(this.minDistanceY, this.maxDistanceY);
            isNegative = Random.Range(0, 2) <= 0 ? -1 : 1;
        }
    }

    private void ChooseCoinToSpawn()
    {
        int randomChance = Random.Range(0, 1001);
        int allCoinsLength = this.allCoins.Length;
        int top = 0;
        for (int i = 0; i < allCoinsLength; i++)
        {
            top += allCoins[i].chance;

            if (randomChance < top)
            {
                coin = this.allCoins[i];
                ObjectController newCoin = Instantiate(coin);
                newCoinPos = newObjPos;
                newCoinPos.x = -newObjPos.x;
                newCoinPos.y += Random.Range(-newCoin.additionalYDistance, newCoin.additionalYDistance);
                newCoin.mainTransform.position = newCoinPos;
                newCoin.mainTransform.parent = this.transformParent;
                break;
            }
        }
    }

    private void ChooseObjToSpawn()
    {      
        int randomChance = Random.Range(1, 1001); //101
        int objectsWithRateCount = this.objectsWithRate.Count;
        int top = 0;
        for (int i = 0; i < objectsWithRateCount + 1; i++)
        {
            if (i < objectsWithRateCount)
            {
                top += objectsWithRate[i].chance;

                if (randomChance < top)
                {
                    newObject = objectsWithRate[i];
                    break;
                }
            }
            else
            {
                top += chanceForObjectsWithoutRate;

                if (randomChance < top)
                {
                    int rand = Random.Range(0, objectsWithNoRate.Count);
                    newObject = objectsWithNoRate[rand];
                    break;
                }
            }
        }

        this.additionalYDistance = newObject.additionalYDistance;

        ObjectController newObj = Instantiate(newObject);
        newObj.mainTransform.position = newObjPos;
        newObj.mainTransform.parent = this.transformParent;
    }

    private void DetermineRateValues(ObjectController[] objects, bool addToRateArrays = true)
    {
        //this method should be used in Start!!! or when adding a new object to the allObjects array during runtime
        //this method is used to determine rate values for the objects to be spawn at a certain rate

        int allRatesSum = 0;
        for (int i = 0; i < objects.Length; i++)
        {
            //this check is used to remove from the arays the enemy with the same sprite as the player
            if (objects[i].CompareTag("Enemy"))
            {
                if (objects[i].GetComponentInChildren<SpriteRenderer>().sprite.name == playerName)
                {
                    continue;
                }
            }

            if (objects[i].hasARateChance)
            {
                if (objects[i].chance <= 0)
                {
                    objects[i].chance = objects[i].initialChance;
                }
                allRatesSum += objects[i].chance;

                if (allRatesSum >= 1000) //100
                {
                    Debug.LogError("The sum of the ChanceInPercent values of the objects which have them overflows 1000!!!");
                }

                if (addToRateArrays)
                {
                    this.objectsWithRate.Add(objects[i]);
                }
            }
            else
            {
                if (addToRateArrays)
                {
                    this.objectsWithNoRate.Add(objects[i]);
                }
            }
        }

        if (addToRateArrays)
        {
            chanceForObjectsWithoutRate = 1000 - allRatesSum;
        }

    }
}
