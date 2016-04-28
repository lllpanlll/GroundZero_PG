using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_AP : MonoBehaviour
{
    Slider sliderAP;
    public GameObject oPlayer;
    T2.Manager t_mgr;
    Image[] image;
    bool bChk;

    // Use this for initialization
    void Start()
    {
        sliderAP = transform.GetComponentInChildren<Slider>();
        t_mgr = oPlayer.GetComponent<T2.Manager>();
        image = gameObject.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        sliderAP.value = t_mgr.GetAP();
    }
}
