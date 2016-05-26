using UnityEngine;
using System.Collections;

public class UI_IconObject : MonoBehaviour {

    public GameObject oIconObjPref;
    GameObject oUIMap;
    GameObject oPlayer;

    float oX, oZ, objX, objZ;

    GameObject o3dPannel;
    GameObject o3dUi;

    Camera camMain;
    Camera camUi;

    void Awake()
    {
        oPlayer = GameObject.FindGameObjectWithTag(Tags.Player);
        oUIMap = GameObject.Find("Center_UI"); // warning
        oIconObjPref = Instantiate(oIconObjPref) as GameObject;

        o3dPannel = GameObject.Find("3D_UI"); // warning
        o3dUi = Instantiate(oIconObjPref) as GameObject;

        camMain = Camera.main.GetComponent<Camera>();
        camUi = GameObject.Find("UI_Camera").GetComponent<Camera>();
    }

    void Start ()
    {
		oIconObjPref.transform.parent = oUIMap.transform;
		oIconObjPref.transform.localScale = new Vector3 (1, 1, 1);

        o3dUi.transform.parent = o3dPannel.transform;
        o3dUi.transform.localScale = new Vector3(2, 2, 1);
        o3dUi.transform.localPosition = new Vector3 (0, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        // obj
        oX = transform.position.x - oPlayer.transform.position.x;
        oZ = transform.position.z - oPlayer.transform.position.z;
        objX = (oX * 1.5f);
        objZ = (oZ * 1.5f);
        oIconObjPref.transform.localPosition = new Vector3(objX, objZ);
        //oIconObjPref.transform.rotation = Quaternion.identity;


        //    Vector3 screenPos = camera.WorldToScreenPoint(target.position);
        //    float x = screenPos.x;

        //    elementalText.transform.position = new Vector3(x, screenPos.y, elementalText.transform.position.z);


        Vector3 Main_screen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 Ui_World = camUi.ScreenToWorldPoint(Main_screen);
        Vector3 Ui_screen = camUi.WorldToScreenPoint(Ui_World);

        Vector3 newUi_world = camUi.ScreenToWorldPoint(o3dUi.transform.localPosition);
        Vector3 newMain_screen = camMain.WorldToScreenPoint(newUi_world);


        //if (Ui_World.z < 0)
        //    o3dUi.SetActive(false);
        //else
        //    o3dUi.SetActive(true);
        o3dUi.transform.localPosition = new Vector3(newMain_screen.x, newMain_screen.y, 0);
    }
}
