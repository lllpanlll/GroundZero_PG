using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenUI_DisanceObject : MonoBehaviour {

    public GameObject oTarget;
    Vector3 WorldToScreen;

    Transform trPlayer;
    Image imageUi;
    Text textUi;

    public void SetTaget(GameObject _target)
    {
        oTarget = _target; 
    }
    
    void Awake()
    {
        trPlayer = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Transform>();
        imageUi = transform.GetComponent<Image>();
        textUi = transform.GetComponentInChildren<Text>();
    }

    void Update()
    {
        WorldToScreen = Camera.main.WorldToScreenPoint(oTarget.transform.position);
        if (WorldToScreen.z > 0)
        {
            imageUi.enabled = true;
            textUi.enabled = true;
        }
        else
        {
            imageUi.enabled = false;
            textUi.enabled = false;
        }
        transform.position = new Vector3(WorldToScreen.x, WorldToScreen.y, transform.position.z);

        float fDist = Vector3.Distance(trPlayer.position, oTarget.transform.position);

        textUi.text = ((int)fDist).ToString() + "m";
    }
}
