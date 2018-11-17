using UnityEngine;
using System.IO;
using System;

//public enum MainColors
//{
//    Red = 0, ARed = 1, Pink = 2, APink = 3, Purple = 4, APurple = 5, DeepPurple = 6, ADeepPurple = 7,
//    Indigo = 8, AIndigo = 9, Blue = 10, ABlue = 11, LightBlue = 12, ALightBlue = 13, Cian = 14, ACian = 15,
//    Teal = 16, ATeal = 17, Green = 18, AGreen = 20,
//    LightGreen = 20, ALightGreen = 21, Lime = 22, ALime = 23,
//    Yellow = 24, AYellow = 25, Amber = 26, AAmber = 27, Orange = 28,
//    AOrange = 29, DeepOrange = 30, ADeepOrange = 31, Brown = 16, Grey = 17, BlueGrey = 19
//}

public enum ColorPalette
{
    Red = 0, Pink = 1, Purple = 2, DeepPurple = 3,
    Indigo = 4, Blue = 5, LightBlue = 6, Cian = 7,
    Teal = 8, Green = 9,
    LightGreen = 10, Lime = 11,
    Yellow = 12, Amber = 13, Orange = 14,
    DeepOrange = 15, Brown = 16, Grey = 17, BlueGrey = 18
}

public enum Saturation
{
    Main50 = 0, Main100 = 1, Main200 = 2, Main300 = 3, Main400 = 4, Main500 = 5, Main600 = 6, Main700 = 7,
    Main800 = 8, Main900 = 9, Accent100 = 10, Accent200 = 11, Accent400 = 12, Accent700 = 13
}

public enum PaletteType
{
    Main, Accent
}

public static class MaterialColors
{
    private static char[] rgb = {'r', 'g', 'b' };

    //WARNING!! This method is very slow so it should be used only on loading!
    public static Color[] GetColorPalette(ColorPalette color)
    {
        Color[] colors;
        if ((int)color > 15)
        {
            colors = new Color[10];
        }
        else
        {
            colors = new Color[14];
        }

        TextAsset textFile = Resources.Load("Material Colors") as TextAsset;

        using (StringReader read = new StringReader(textFile.text))
        {
            string line = string.Empty;
            int counter = 1;
            int arrayCounter = 0;
            int[] lineNumbers = GetLineNumbers(color);
            while (line != null)
            {
                line = read.ReadLine();
                if (counter > lineNumbers[0] && counter < lineNumbers[1])
                {
                    colors[arrayCounter] = ReadColor(line);
                    arrayCounter++;
                }
                else if (counter > lineNumbers[1] && counter <= lineNumbers[2])
                {
                    colors[arrayCounter] = ReadColor(line);
                    arrayCounter++;
                }
                else if (counter > lineNumbers[2])
                {
                    break;
                }
                counter++;
            }
        }
        Resources.UnloadAsset(textFile);

        //this is needed to turn the RGB values in numbers between 0 and 1
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].r /= 255;
            colors[i].g /= 255;
            colors[i].b /= 255;
        }
        return colors;
    }

    private static int[] GetLineNumbers(ColorPalette color)
    {
        int[] startMidEndLineNumbers = new int[3];
        int interval = 16; //the interval betweeen the different patterns in the text file

        if (color == ColorPalette.Brown || color == ColorPalette.Grey || color == ColorPalette.BlueGrey)
        {
            startMidEndLineNumbers[0] = 256 + ((int)color - 16) * 10 + ((int)color - 15);
            startMidEndLineNumbers[1] = startMidEndLineNumbers[0] + 11;
            startMidEndLineNumbers[2] = startMidEndLineNumbers[1];
        }
        else
        {
            startMidEndLineNumbers[0] = (int)color * interval + 1;
            startMidEndLineNumbers[1] = startMidEndLineNumbers[0] + 11;
            startMidEndLineNumbers[2] = startMidEndLineNumbers[1] + 4;
        }

        return startMidEndLineNumbers;
    }

    private static Color ReadColor(string line)
    {
        string[] rgbStr = line.Split(rgb, StringSplitOptions.RemoveEmptyEntries);
        Color color = new Color(int.Parse(rgbStr[0]), int.Parse(rgbStr[1]), int.Parse(rgbStr[2]));
        return color;
    }
}

