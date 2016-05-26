using UnityEngine;
using System.Collections;

public class UI_Minimap : MonoBehaviour
{
    public Transform trCamMain;
    //public Transform trMap;
    public Transform trPlayer;
    public Transform trIconMap;
    public Transform trIconPlayer;
    public int mapWidth = 300;
    public int mapHeight = 300;
    public int sceneWidth = 500;
    public int sceneHeight = 500;


    void Update()
    {
        float mX = GetMapPos(trPlayer.transform.position.x, mapWidth, sceneWidth);
        float mZ = GetMapPos(trPlayer.transform.position.z, mapHeight, sceneHeight);

        //trIconPlayer.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -trCamMain.eulerAngles.y);
        trIconPlayer.localRotation = Quaternion.Euler(0, 0, -trCamMain.eulerAngles.y);
        trIconMap.localPosition = new Vector3(-mX, -mZ, 0);
        //// map
        //float mX = GetMapPos(player.transform.position.x, mapWidth, sceneWidth);
        //float mZ = GetMapPos(player.transform.position.z, mapHeight, sceneHeight);
        ////float mapX = (mX * 0.5f) + (mapWidth * 0.5f);
        ////float mapZ = (-mZ * 0.5f) + (mapHeight * 0.5f);
        //GUI.Box(new Rect(-mX, mZ, 500, 500), "", mapIcon);

        //// player
        //float pX = GetMapPos(player.transform.position.x, mapWidth, sceneWidth);
        //float pZ = GetMapPos(player.transform.position.z, mapHeight, sceneHeight);
        ////float playerMapX = pX - iconHalfSize;
        ////float playerMapZ = ((pZ * -1) - iconHalfSize) + mapHeight;
        //float playerX = (pX * 0.5f) + (mapWidth * 0.5f);
        //float playerZ = (-pZ * 0.5f) + (mapHeight * 0.5f);
        //GUI.Box(new Rect(150, 150, iconSize, iconSize), "", playerIcon);
    }

    float GetMapPos(float pos, float mapSize, float sceneSize)
    {
        return pos * mapSize / sceneSize;
    }
}