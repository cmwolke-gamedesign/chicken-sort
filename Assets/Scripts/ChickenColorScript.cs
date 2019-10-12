using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChickenColorScript : MonoBehaviour{

    static public Dictionary<string, Color> ChickenColors;
    static public List<string> ChickenColorsInUse; // keys for color dictionary go there.
    // Use this for initialization

    public static string ReturnRandomColor()
    {
        if (ChickenColors == null)
        {
            populateColors();
        }
        List<string> colors = System.Linq.Enumerable.ToList(ChickenColors.Keys);
        string newColor = colors[Random.Range(0, colors.Count)];
        return newColor;
    }

    public static string ReturnNewChickenColor()
    {
        if (ChickenColorsInUse == null)
        {
            ChickenColorsInUse = new List<string>();
        }

        if (ChickenColors == null)
        {
            populateColors();
        }

        string newColor = "";
        newColor = ChickenColorsInUse[Random.Range(0, ChickenColorsInUse.Count-1)];
        return newColor;
    }

    /** gets new color that's not in use and adds that color to the list of colors in use **/
    public static string GetUniqueRandomColor()
    {
        if (ChickenColorsInUse == null)
        {
            ChickenColorsInUse = new List<string>();
        }
        if (ChickenColors == null)
        {
            populateColors();
        }
        if (ChickenColorsInUse.Count != ChickenColors.Count) // check if all colors taken
        {
            Dictionary<string, Color> remainingValues = new Dictionary<string, Color>(ChickenColors);
            foreach (string key in ChickenColorsInUse)
            {
                remainingValues.Remove(key);
            }
            List<string> remainingColors = System.Linq.Enumerable.ToList(remainingValues.Keys);
            string newColor = remainingColors[Random.Range(0, remainingColors.Count)];
            ChickenColorsInUse.Add(newColor);
            return newColor;
        }
        else
        {
            return null;
        }
    }

    private static void populateColors()
    {
        ChickenColors = new Dictionary<string, Color>();
        ChickenColors.Add("pink", new Color32(234, 117, 186, 255));
        ChickenColors.Add("purple", new Color32(153, 38, 255, 255));
        ChickenColors.Add("yellow", new Color32(255, 248, 22, 255));
        ChickenColors.Add("orange", new Color32(255, 144, 0, 255));
        ChickenColors.Add("green", new Color32(0, 255, 8, 255));
        ChickenColors.Add("blue", new Color32(18, 92, 165, 255));
        ChickenColors.Add("red", new Color32(208, 41, 41, 255));
        ChickenColors.Add("grey", new Color32(144, 144, 144, 255));
        ChickenColors.Add("white", new Color32(255, 255, 255, 255));
    }
    public static void dePopulateColors()
    {
        ChickenColorsInUse = new List<string>();
    }
}
