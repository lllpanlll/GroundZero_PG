using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_DP_PP : MonoBehaviour
{

    Slider sliderDP;
    Slider sliderPP;
    public GameObject oPlayer;
    T2.Manager t_mgr;
    Image imageHP;
    RectTransform trHighlight;
    Image imageHighlight;
    bool bHighlight;
    int iPrePp;
    float fRotHighlight;
    float fTimer;
	public GameObject IMAGEBACK;

    // Use this for initialization
    void Start()
    {
        sliderDP = transform.FindChild("DP").GetComponent<Slider>();
        sliderPP = transform.FindChild("PP").GetComponent<Slider>();
        t_mgr = oPlayer.GetComponent<T2.Manager>();
        imageHP = transform.FindChild("DpDanger").GetComponent<Image>();
        trHighlight = transform.FindChild("PpHighlight").GetComponent<RectTransform>();
        imageHighlight = trHighlight.GetComponent<Image>();
		imageHighlight.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        sliderDP.value = t_mgr.GetDP();
        sliderPP.value = t_mgr.GetPP();

        if (sliderDP.value.Equals(0))
        {
            imageHP.enabled = true;
        }
        else
            imageHP.enabled = false;


        if (iPrePp < sliderPP.value)
        {
            bHighlight = true;
            switch ((int)sliderPP.value)
            {
                case 100:
                    fRotHighlight = 0;
                    break;
                case 75:
                    fRotHighlight = 45;
                    break;
                case 50:
                    fRotHighlight = 90;
                    break;
                case 25:
                    fRotHighlight = 135;
                    break;
                default:
                    bHighlight = false;
                    break;
            }
			trHighlight.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, IMAGEBACK.transform.eulerAngles.z + fRotHighlight);
            if (bHighlight)
            {
                imageHighlight.color = Color.white;
                fTimer = 0;
            }
        }
        fTimer += Time.deltaTime;
        if (fTimer >= 1)
        {
            imageHighlight.color = Color.clear;
        }

        iPrePp = (int)sliderPP.value;
    }
}
