using UnityEngine;
using System.Collections;

public class ChickenSortEvents : MonoBehaviour
{

    public delegate void PauseEvent();
    public static event PauseEvent onPause;
    public static event PauseEvent onUnpause;

    public static void PauseGame()
    {
        onPause(); 
    }

    public static void UnpauseGame()
    {
        onUnpause();
    }

    public delegate void GameEvent();
    public static event GameEvent onMultiplierReset;

    public delegate void GameEventObject(GameObject go = null);
    public static event GameEventObject onChickenCaptured;
    public static event GameEventObject onChickenSpawn;
    public static event GameEventObject onLayEgg; 
    public static event GameEventObject onHatchEgg;
    public static event GameEventObject onSpawnNewChicken;
    public static event GameEventObject onChickGrowup;
    public static event GameEventObject onEggCollect;
    

    public static void MultiplierReset()
    {
        onMultiplierReset();
    }

    public static void ChickGrowsUp(GameObject chick)
    {
        onChickGrowup(chick);
    }

    public static void SpawnNewChicken()
    {
        onSpawnNewChicken(null);
    }

    public static void CaptureChicken(GameObject chicken)
    {
        onChickenCaptured(chicken); 
    }

    public static void SpawnChicken()
    {
        onChickenSpawn();
    }

    public static void LayEgg(GameObject chickenLayingEgg)
    {
        onLayEgg(chickenLayingEgg);
    }
    public static void CollectEgg(GameObject egg)
    {
        onEggCollect(egg);
    }
    public static void HatchEgg(GameObject egg)
    {
        onHatchEgg(egg);
    }

    public delegate void LevelEvent(int newLevel);
    public static event LevelEvent onLevelChange; 

    public static void LevelChange(int newLevel)
    {
        print(newLevel + " is new level");
        onLevelChange(newLevel);
    }
}
