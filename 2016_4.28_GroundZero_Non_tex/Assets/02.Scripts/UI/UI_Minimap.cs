using UnityEngine;
using System.Collections;

public class UI_Minimap : MonoBehaviour
{
    public Transform trCamMain;
    //public Transform trMap;
    public Transform trPlayer;
    public Transform trIconMap;
    public int mapWidth = 300;
    public int mapHeight = 300;
    public int sceneWidth = 500;
    public int sceneHeight = 500;


    void Update()
    {
        float mX = GetMapPos(trPlayer.transform.position.x, mapWidth, sceneWidth);
        float mZ = GetMapPos(trPlayer.transform.position.z, mapHeight, sceneHeight);

        trIconMap.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -trCamMain.eulerAngles.y);
    }

    float GetMapPos(float pos, float mapSize, float sceneSize)
    {
        return pos * mapSize / sceneSize;
    }
}