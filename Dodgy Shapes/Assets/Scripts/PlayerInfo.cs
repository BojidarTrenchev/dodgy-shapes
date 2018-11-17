using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo info;

    public int highScore;
    public int allCoins;

    public int activePlayerShapeIndex;
    public int lastUnlockedShapeIndex;
    public bool unlockNextShape;
    public ColorPalette accentPalette;
    private ColorPalette mainPalette;

    public PlayerController[] allPlayerShapes;
    public Color[] allAcColors;
    private Color[] mainColors;
    private Color[] accentColors;
    public List<int> boughtShapes;
    public int[] shapesAcColors;
    public List<int> boughtColorsIndeces;

    public int mysteryTokens;
    public int maxMysteryTokens;

    public Saturation playerSaturation = Saturation.Accent200;
    
    private float basicSpeed = 15;
    private float basicDodgeSpeed = 2;
    private float basicSpeedUpCoefficient = 0.002f;
    private int basicHitPointsMax = 3;
    private int basicStartHitPoints = 2;
    private StringBuilder[] bonuses;

    private string dataPath;

    private const int countOfPalettes = 17; // without grey and blue grey
    private bool wasAcPaletteChanged;

    private DateTime lastDateTimeMoney;
    private DateTime lastDateTimeGift;
    private bool canGiveFreeGift;
    private bool canGiveFreeMoney;
    public int minutesBetweenFreeMoney = 30;
    public int minutesBetweenFreeGift = 3;
    private bool isFirstTimeGift;
    private bool isFirstTimeMoney;

    void Awake()
    {
        if (info == null)
        {
            DontDestroyOnLoad(this.gameObject);
            info = this;
        }
        else if (info != this)
        {
            Destroy(this.gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoad;
        //TODO: Use playerinfo for money instead of Values
        dataPath = Application.persistentDataPath + "/gameData.dat";
       //File.Delete(dataPath);
        LoadData();

        mainPalette = ChooseMainColorPalette(accentPalette);
        mainColors = MaterialColors.GetColorPalette(mainPalette);
        accentColors = MaterialColors.GetColorPalette(accentPalette);

        PlayerController activeShape = allPlayerShapes[activePlayerShapeIndex];
        basicSpeed = activeShape.speed;
        basicDodgeSpeed = activeShape.dodgeSpeed;
        basicSpeedUpCoefficient = activeShape.speedUpCoefficient;
        basicHitPointsMax = activeShape.hitPointsMaximum;
        basicStartHitPoints = activeShape.hitPoints;
    }

    public void Start()
    {
        bonuses = new StringBuilder[5];

        for (int i = 0; i < 5; i++)
        {
            bonuses[i] = new StringBuilder();
        }

        DOTween.Init();
    }
         

    public void ChooseActivePlayerShape(int indexInArray)
    {
        activePlayerShapeIndex = indexInArray;
        PlayerController activeShape = allPlayerShapes[activePlayerShapeIndex];
        basicSpeed = activeShape.speed;
        basicDodgeSpeed = activeShape.dodgeSpeed;
        basicSpeedUpCoefficient = activeShape.speedUpCoefficient;
        basicHitPointsMax = activeShape.hitPointsMaximum;
        basicStartHitPoints = activeShape.hitPoints;
    }

    public void ChangeAcColor(ColorPalette newPalette)
    {
        if (accentPalette != newPalette)
        {
            accentPalette = newPalette;
            wasAcPaletteChanged = true;
            mainPalette = ChooseMainColorPalette(newPalette);
        }
    }

    public Color GetMainColor(Saturation mainSaturation)
    {
        return mainColors[(int)mainSaturation];
    }

    public Color GetAcColorBySaturation(Saturation accentSaturation)
    {
        return accentColors[(int)accentSaturation];
    }

    public Color GetAcColor(int indexOfColor)
    {
        return allAcColors[indexOfColor];
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {        
        SaveData();
        if (wasAcPaletteChanged)
        {

            accentColors = MaterialColors.GetColorPalette(accentPalette);
            wasAcPaletteChanged = false;
        }
        mainPalette = ChooseMainColorPalette(accentPalette);
        mainColors = MaterialColors.GetColorPalette(mainPalette);
    }

    public string[] GetBonuses(int indexOfShape)
    {
        if (indexOfShape == activePlayerShapeIndex)
        {
            return null;
        }
        PlayerController shape = allPlayerShapes[indexOfShape];
        int numberOfBonuses = 0;
        // 5 is the max number of bonuses

        if (shape.hitPoints != basicStartHitPoints)
        {
            numberOfBonuses++;
            if (shape.hitPoints < basicStartHitPoints)
            {
                float difference = basicStartHitPoints - shape.hitPoints;
                bonuses[numberOfBonuses - 1].Append("- ");
                bonuses[numberOfBonuses - 1].Append(difference);
                bonuses[numberOfBonuses - 1].Append(" start hit points");
            }
            else
            {
                float difference = shape.hitPoints - basicStartHitPoints;
                bonuses[numberOfBonuses - 1].Append("+ ");
                bonuses[numberOfBonuses - 1].Append(difference);
                bonuses[numberOfBonuses - 1].Append(" start hit points");
            }
        }

        if (shape.hitPointsMaximum != basicHitPointsMax)
        {
            numberOfBonuses++;
            if (shape.hitPointsMaximum < basicHitPointsMax)
            {
                float difference = basicHitPointsMax - shape.hitPointsMaximum;
                bonuses[numberOfBonuses - 1].Append("- ");
                bonuses[numberOfBonuses - 1].Append(difference);
                bonuses[numberOfBonuses - 1].Append(" maximum hit points");
            }
            else
            {
                float difference = shape.hitPointsMaximum - basicHitPointsMax;
                bonuses[numberOfBonuses - 1].Append("+ ");
                bonuses[numberOfBonuses - 1].Append(difference);
                bonuses[numberOfBonuses - 1].Append(" maximum hit points");
            }
        }

        if (shape.speed != basicSpeed)
        {
            numberOfBonuses++;
            float percent = Mathf.Abs(basicSpeed - shape.speed) / basicSpeed * 100;
            if (shape.speed < basicSpeed)
            {
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% lower start speed");
            }
            else
            {
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% higher start speed");
            }
        }

        if (shape.speedUpCoefficient != basicSpeedUpCoefficient)
        {
            numberOfBonuses++;
            float percent = Mathf.Abs(basicSpeedUpCoefficient - shape.speedUpCoefficient) / basicSpeedUpCoefficient * 100;
            if (shape.speedUpCoefficient < basicSpeedUpCoefficient)
            {
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% lower acceleration");
            }
            else
            {
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% higher acceleration");
            }
        }

        if (shape.dodgeSpeed != basicDodgeSpeed)
        {
            numberOfBonuses++;
            float percent = Mathf.Abs(basicDodgeSpeed - shape.dodgeSpeed) / basicDodgeSpeed * 100;
            if (shape.dodgeSpeed < basicDodgeSpeed)
            {
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% lower dodge speed");
            }
            else
            {
                bonuses[numberOfBonuses - 1].Append("+");
                bonuses[numberOfBonuses - 1].Append((int)percent);
                bonuses[numberOfBonuses - 1].Append("% higher dodge speed");
            }
        }

        string[] allBonuses = new string[numberOfBonuses];
        for (int i = 0; i < numberOfBonuses; i++)
        {
            allBonuses[i] = bonuses[i].ToString();
            bonuses[i].Remove(0, bonuses[i].Length);
        }

        return allBonuses;
    }

    public static ColorPalette ChooseMainColorPalette(ColorPalette accentColorPalette)
    {
        int mainColor = 0;

        if ((int)accentColorPalette == 0)
        {
            mainColor = UnityEngine.Random.Range(3, countOfPalettes);
        }
        else if ((int)accentColorPalette == countOfPalettes - 1)
        {
            mainColor = UnityEngine.Random.Range(0, countOfPalettes - 2);
        }
        else
        {
            bool isBiggerThanAccentPalette;

            if ((int)accentColorPalette + 3 > countOfPalettes - 1)
            {
                isBiggerThanAccentPalette = false;
            }
            else if ((int)accentColorPalette - 3 < 0)
            {
                isBiggerThanAccentPalette = true;
            }
            else
            {
                isBiggerThanAccentPalette = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
            }

            if (isBiggerThanAccentPalette)
            {
                mainColor = UnityEngine.Random.Range((int)accentColorPalette + 3, countOfPalettes);
            }
            else
            {
                mainColor = UnityEngine.Random.Range(0, (int)accentColorPalette - 2);
            }
        }
        return (ColorPalette)mainColor;
    }

    public void SetNewLastDateMoney()
    {
        lastDateTimeMoney = DateTime.Now;
        isFirstTimeMoney = false;
    }

    public void SetNewLastDateGift()
    {
        lastDateTimeGift = DateTime.Now;
        isFirstTimeGift = false;
    }

    public bool CanGiveFreeMoney()
    {
        if ((DateTime.Now - lastDateTimeMoney).TotalMinutes >= minutesBetweenFreeMoney || isFirstTimeMoney)
        {
            canGiveFreeMoney = true;
        }
        else
        {
            canGiveFreeMoney = false;
        }

        return canGiveFreeMoney;
    }

    public bool CanGiveFreeGift()
    {
        if ((DateTime.Now - lastDateTimeGift).TotalMinutes >= minutesBetweenFreeGift || isFirstTimeGift)
        {
            canGiveFreeGift = true;
        }
        else
        {
            canGiveFreeGift = false;
        }

        return canGiveFreeGift;
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(dataPath);

        GameData data = new GameData();
        data.activePlayerShapeIndex = this.activePlayerShapeIndex;
        data.lastUnlockedShapeIndex = this.lastUnlockedShapeIndex;
        data.unlockNextShape = this.unlockNextShape;
        data.allCoins = this.allCoins;
        data.highScore = this.highScore;
        data.accentPalette = (int)accentPalette;
        data.boughtShapes = this.boughtShapes.ToArray();
        data.shapesAcColors = this.shapesAcColors;
        data.boughtColorsIndeces = this.boughtColorsIndeces.ToArray();
        data.lastDateTimeMoney = this.lastDateTimeMoney;
        data.lastDateTimeGift = this.lastDateTimeGift;
        data.isFirstTimeMoney= this.isFirstTimeMoney;
        data.isFirstTimeGift = this.isFirstTimeGift;
        data.mysteryTokens = this.mysteryTokens;
        data.maxMysteryTokens = this.maxMysteryTokens;
        data.isPlayingSound = MainAuidoManager.isPlayingSound;
        data.isPlayingMusic = MainAuidoManager.isPlayingMusic;
        data.canVibrate = Vibration.canVibrate;

        bf.Serialize(file, data);

        file.Close();
    }

    public void LoadData()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            this.activePlayerShapeIndex = data.activePlayerShapeIndex;
            this.lastUnlockedShapeIndex = data.lastUnlockedShapeIndex;
            this.unlockNextShape = data.unlockNextShape;
            this.highScore = data.highScore;
            this.allCoins = data.allCoins;
            accentPalette = (ColorPalette)data.accentPalette;
            this.boughtShapes = data.boughtShapes.ToList();

            if (this.lastUnlockedShapeIndex == 0)
            {
                this.lastUnlockedShapeIndex = 1;
            }

            if (this.boughtShapes.Count == 0)
            {
                this.boughtShapes.Add(0);
            }
            if (data.shapesAcColors == null || data.shapesAcColors.Length == 0)
            {
                this.shapesAcColors = new int[AllStoreShapesContainer.allStoreShapesLength];
                this.shapesAcColors[1] = 1;
            }
            else
            {
                this.shapesAcColors = data.shapesAcColors;
            }

            if (data.boughtColorsIndeces == null || data.boughtColorsIndeces.Length == 0)
            {
                this.boughtColorsIndeces = new List<int>();
                this.boughtColorsIndeces.Add(0);
                this.boughtColorsIndeces.Add(1);
            }
            else
            {
                this.boughtColorsIndeces = data.boughtColorsIndeces.ToList();
            }

            this.lastDateTimeMoney = data.lastDateTimeMoney;
            this.lastDateTimeGift = data.lastDateTimeGift;
            this.isFirstTimeGift = data.isFirstTimeGift;
            this.isFirstTimeMoney = data.isFirstTimeMoney;

            this.mysteryTokens = data.mysteryTokens;
            this.maxMysteryTokens = data.maxMysteryTokens;
            if (this.maxMysteryTokens <= 0)
            {
                this.maxMysteryTokens = 5;
            }

            MainAuidoManager.isPlayingSound = data.isPlayingSound;
            MainAuidoManager.isPlayingMusic = data.isPlayingMusic;
            Vibration.canVibrate = data.canVibrate;
        }
        else
        {
            this.lastUnlockedShapeIndex = 1;
                        
            this.boughtShapes = new List<int>();
            this.boughtShapes.Add(0);

            this.shapesAcColors = new int[AllStoreShapesContainer.allStoreShapesLength];
            this.shapesAcColors[1] = 1;

            this.boughtColorsIndeces = new List<int>();
            this.boughtColorsIndeces.Add(0);
            this.boughtColorsIndeces.Add(1);

            this.lastDateTimeMoney = DateTime.Now;
            this.lastDateTimeGift = DateTime.Now;
            this.isFirstTimeMoney = true;
            this.isFirstTimeGift = true;

            this.maxMysteryTokens = 15;

            MainAuidoManager.isPlayingSound = true;
            MainAuidoManager.isPlayingMusic = true;
            Vibration.canVibrate = true;
        }
    }
}

[Serializable]
class GameData
{
    public int activePlayerShapeIndex;
    public int accentPalette;
    public int lastUnlockedShapeIndex;
    public bool unlockNextShape;

    public int allCoins;
    public int highScore;

    public int[] boughtShapes;
    public int[] shapesAcColors;
    public int[] boughtColorsIndeces;

    public DateTime lastDateTimeMoney;
    public DateTime lastDateTimeGift;
    public bool isFirstTimeGift;
    public bool isFirstTimeMoney;

    public int mysteryTokens;
    public int maxMysteryTokens;

    public bool isPlayingSound = true;
    public bool isPlayingMusic = true;
    public bool canVibrate = true;
}
