using UnityEngine;
using System.Collections;

public class testMap : MonoBehaviour
{
    public Transform map;
    public Transform player;
    public Transform enemy;
    public GUIStyle miniMap;
    public GUIStyle mapIcon;
    public GUIStyle playerIcon;
    public GUIStyle enemyIcon;
    public int mapOffSetX = 100;
    public int mapOffSetY = 100;
    public int mapWidth = 300;
    public int mapHeight = 300;
    public int sceneWidth = 500;
    public int sceneHeight = 500;
    public int iconSize = 10;
    private int iconHalfSize;

    void Update()
    {
        iconHalfSize = iconSize / 2;
    }

    float GetMapPos(float pos, float mapSize, float sceneSize)
    {
        return pos * mapSize / sceneSize;
    }

    void OnGUI()
    {
        //GUI.BeginGroup(new Rect(mapOffSetX, mapOffSetY, mapWidth, mapHeight), miniMap);
        
        // map
        float mX = GetMapPos(player.transform.position.x, mapWidth, sceneWidth);
        float mZ = GetMapPos(player.transform.position.z, mapHeight, sceneHeight);
        //float mapX = (mX * 0.5f) + (mapWidth * 0.5f);
        //float mapZ = (-mZ * 0.5f) + (mapHeight * 0.5f);
        GUI.Box(new Rect(-mX, mZ, 500, 500), "", mapIcon);

        // player
        float pX = GetMapPos(player.transform.position.x, mapWidth, sceneWidth);
        float pZ = GetMapPos(player.transform.position.z, mapHeight, sceneHeight);
        //float playerMapX = pX - iconHalfSize;
        //float playerMapZ = ((pZ * -1) - iconHalfSize) + mapHeight;
        float playerX = (pX * 0.5f) + (mapWidth * 0.5f);
        float playerZ = (-pZ * 0.5f) + (mapHeight * 0.5f);
        GUI.Box(new Rect(150, 150, iconSize, iconSize), "", playerIcon);

        // enemy
        float sX = GetMapPos(enemy.transform.position.x - player.transform.position.x, mapWidth, mapWidth);
        float sZ = GetMapPos(enemy.transform.position.z - player.transform.position.z, mapHeight, mapHeight);
        float enemyX = (sX * 0.5f) + (mapWidth * 0.5f);
        float enemyZ = (-sZ * 0.5f) + (mapHeight * 0.5f);
        GUI.Box(new Rect(enemyX, enemyZ, iconSize, iconSize), "", enemyIcon);

        //GUI.EndGroup();
    }
}