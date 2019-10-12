using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public static SpawnableArea spawnArea = new SpawnableArea(-8f, 3.7f, 8f, -3.7f);
    private static int spawnedChickensCounter = 0;

    /** SLIGHTLY RANDOMIZED COOP POSITIONS **/
    public static CoopPos[] active_coopset;
    public static CoopPos[] coops1 = {new CoopPos(7.5f, -3.2f, 0), new CoopPos(-7.5f, 3f, 105), new CoopPos(-7.5f, -3.4f, 180),
        new CoopPos(7.5f, 3f, 80), new CoopPos(-1.7f, 3f, 70)};
    public static CoopPos[] coops2 = {new CoopPos(-7.5f, -3f, 260), new CoopPos(-7.5f, 3f, 150), new CoopPos(7.5f, 3f, 30),
        new CoopPos(7.5f, -3.4f, 0), new CoopPos(0f, 3, 90)};
    public static CoopPos[] coops3 = {new CoopPos(-7.5f, 3f, 105), new CoopPos(0f, 3, 55), new CoopPos(-7.5f, -3.4f, 180),
        new CoopPos(3f, 3f, 90), new CoopPos(7.5f, -3.2f, 340)};
    

    /** handles spawning a new coop at activecoopset -index's- position. the coop has a random color that gets added to the current active colorset **/
    public static GameObject SpawnCoop(int index)
    {
        if (active_coopset == null) ChooseRandomActiveCoopset();
        string coopColor = ChickenColorScript.GetUniqueRandomColor();
        if (coopColor == null) return null; // all colors have been taken. 

        Color c_coopColor;
        ChickenColorScript.ChickenColors.TryGetValue(coopColor, out c_coopColor);
        GameObject coop = Instantiate(Resources.Load<GameObject>("Prefabs/Coop"), new Vector3(active_coopset[index].x, active_coopset[index].y), Quaternion.Euler(0, 0, active_coopset[index].z_rot)) as GameObject;
        coop.GetComponent<CoopLogic>().color = coopColor;
        coop.GetComponent<SpriteRenderer>().color = c_coopColor;

        return coop;
    }

    public static GameObject SpawnChicken(Vector3? pos = null)
    {
        spawnedChickensCounter++;

        if (pos == null) pos = NewPosition();

        string chickenColor = ChickenColorScript.ReturnNewChickenColor();
        if (chickenColor == null) return null;
        Color c_chickenColor;
        ChickenColorScript.ChickenColors.TryGetValue(chickenColor, out c_chickenColor);

        GameObject chicken = Instantiate(Resources.Load<GameObject>("Prefabs/Chicken"), pos.Value, Quaternion.identity) as GameObject;
        chicken.GetComponent<SpriteRenderer>().color = c_chickenColor;
        chicken.GetComponent<Chicken>().color = chickenColor;
        chicken.GetComponentInChildren<LineRenderer>().startColor = c_chickenColor;

        return chicken;
    }

    public static GameObject SpawnChick(Vector3 pos)
    {
        GameObject chick = Instantiate(Resources.Load<GameObject>("Prefabs/Chick"), pos, Quaternion.identity) as GameObject;
        return chick;
    }

        public static void ChooseRandomActiveCoopset()
    {
        int tmp = Random.Range(0, 3);
        switch (tmp)
        {
            case 0:
                active_coopset = coops1;
                break;
            case 1:
                active_coopset = coops2;
                break;
            case 2:
                active_coopset = coops3;
                break;
        }
    }

    private static Vector3 NewPosition()
    {
        bool safePos = false;
        float spawnX, spawnY;
        Vector3 testPos = new Vector3();
        while (!safePos)
        {
            spawnX = Random.Range(spawnArea.x1, spawnArea.x2);
            spawnY = Random.Range(spawnArea.y1, spawnArea.y2);
            testPos = new Vector3(spawnX, spawnY, 0);

            safePos = CheckPosition(testPos);
        }
        return testPos;
    }

    // Returns true if OverlapBox of chicken size has no collisions.
    private static bool CheckPosition(Vector3 position)
    {
        Collider2D[] colls = Physics2D.OverlapBoxAll(position, new Vector2(1.5f, 1.5f), LayerMask.GetMask("Default"));
        if (colls.Length == 0)
        {
            return true;
        }
        else
        {
            print("Colliding with something");
            return false;
        }
    }
}

public class SpawnableArea
{
    public float x1, x2, y1, y2;

    public SpawnableArea(float _x1, float _y1, float _x2, float _y2)
    {
        this.x1 = _x1;
        this.x2 = _x2;
        this.y1 = _y1;
        this.y2 = _y2;
    }
}

public struct CoopPos
{
    public float x, y;
    public int z_rot;

    public CoopPos(float new_x, float new_y, int new_z_rot)
    {
        x = new_x;
        y = new_y;
        z_rot = new_z_rot;
    }
}