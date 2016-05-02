using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Stat : MonoBehaviour {
    // NOTE : EP에 핸들, 텍스트 추가
    // PP에 핸들 추가

    T2.Manager t_mgr;
    public GameObject oPlayer, oAim, oDpPp, oEmptyAP;
    public Slider sliderEP;
    Slider sliderAP, sliderDP, sliderPP;

    Image[] imageEP;
    Text textEp, textAp;

    Image imageHP, imageHighlight, imageEmptyAP;
    RectTransform trHighlight, trPpHandle;
    int iPrePp;
    float fRotHighlight;
    float timerEp, timerAp;
    bool bHighlight;

    float fAlphaEp = 1f, fAlphaPp = 0f, fAlphaAp = 1f;
    float fFadeoutSpeed = 0.5f; // 페이드아웃 속력

    // Use this for initialization
    void Start ()
    {
        t_mgr = oPlayer.GetComponent<T2.Manager>();
        sliderAP = oAim.GetComponentInChildren<Slider>();
        imageEmptyAP = oEmptyAP.GetComponent<Image>();
        textAp = oEmptyAP.transform.FindChild("Text").GetComponent<Text>();
        imageEP = sliderEP.GetComponentsInChildren<Image>();
        textEp = sliderEP.GetComponentInChildren<Text>();
        sliderDP = oDpPp.transform.FindChild("DP").GetComponent<Slider>();
        sliderPP = oDpPp.transform.FindChild("PP").GetComponent<Slider>();
        imageHP = oDpPp.transform.FindChild("DpDanger").GetComponent<Image>();
        trHighlight = oDpPp.transform.FindChild("PpHighlight").GetComponent<RectTransform>();
        imageHighlight = trHighlight.GetComponent<Image>();
        trPpHandle = sliderPP.transform.FindChild("PpHandle").GetComponent<RectTransform>();
        imageHighlight.color = Color.clear;
        iPrePp = 100;
    }
	
	// Update is called once per frame
	void Update ()
    {
        sliderAP.value = t_mgr.GetAP();
        sliderEP.value = t_mgr.GetEP();
        sliderPP.value = t_mgr.GetPP();
        sliderDP.value = t_mgr.GetDP();
        Ap();
        Ep();
        Hp();
        Pp();
    }

    void Ap()
    {
        if (((int)sliderAP.value).Equals(0))
        {
            oEmptyAP.SetActive(true);
        }
        else
        {
            oEmptyAP.SetActive(false);
        }

        // 페이드 처리
        //timerAp += Time.deltaTime;
        //if(timerAp > 1.5f)
        //{
        //    timerAp = 0;
        //    fAlphaAp = 1;
        //}
        //else if(timerAp > 0.5f)
        //{
        //    imageEmptyAP.color = new Color(1, 1, 1, FadeInOut(ref fAlphaAp, -3));
        //}

    }

    void Ep()
    {
        textEp.text = ((int)sliderEP.value).ToString() + "%";

        if (sliderEP.value.Equals(100))
        {
            timerEp += Time.deltaTime;
            if (timerEp > 1)
            {
                for (int i = 0; i < imageEP.Length; i++)
                {
                    imageEP[i].color = new Color(1, 1, 1, FadeInOut(ref fAlphaEp, - 1));
                }
                textEp.color = new Color(1, 1, 1, FadeInOut(ref fAlphaEp, - 1));
            }
        }
        else if (sliderEP.value < sliderEP.maxValue)
        {
            for (int i = 0; i < imageEP.Length; i++)
            {
                imageEP[i].color = Color.white;
            }
            textEp.color = Color.white;
            timerEp = 0;
            fAlphaEp = 1;
        }
    }

    float FadeInOut(ref float _fKindOfStat, float _iDir)
    {
        _fKindOfStat += _iDir * fFadeoutSpeed * Time.deltaTime;
        _fKindOfStat = Mathf.Clamp01(_fKindOfStat);
        return _fKindOfStat;
    }

    //IEnumerator InvisibleEPBar()
    //{
    //    yield return new WaitForSeconds(1f);
    //    if (sliderEP.value.Equals(sliderEP.maxValue))
    //        for (int i = 0; i < imageEP.Length; i++)
    //        {
    //            for (float f = 1f; f >= 0.1f; f -= 0.5f)
    //            {
    //                imageEP[i].color = new Color(1, 1, 1, f);
    //                yield return new WaitForSeconds(0.05f);
    //            }
    //            imageEP[i].color = Color.clear;
    //        }
    //}

    void Hp()
    {
        if (sliderDP.value.Equals(0))
        {
            imageHP.enabled = true;
        }
        else
            imageHP.enabled = false;

    }

    void Pp()
    {
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
            trHighlight.rotation = Quaternion.Euler(
                oDpPp.transform.eulerAngles.x,
                oDpPp.transform.eulerAngles.y,
                oDpPp.transform.eulerAngles.z + fRotHighlight);
            if (bHighlight)
            {
                imageHighlight.color = Color.white;
                fAlphaPp = 1;
            }
        }
        imageHighlight.color = new Color(1, 1, 1, FadeInOut(ref fAlphaPp, - 1.5f));

        iPrePp = (int)sliderPP.value;
        
        float fPi = 185 - (1.8f * sliderPP.value); // 185인 이유는 핸들이 제대로 못 덮기 때문이다
        trPpHandle.transform.rotation = Quaternion.Euler(
                oDpPp.transform.eulerAngles.x,
                oDpPp.transform.eulerAngles.y,
                (360 - oDpPp.transform.eulerAngles.z) + fPi);
    }
}
