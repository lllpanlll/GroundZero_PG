using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Stat : MonoBehaviour
{
    T2.Manager t_mgr;
    public GameObject oPlayer, oAimPanel, oDpPp, oEmptyAP, oSkill;
    public Slider sliderEP;
    Slider sliderAP, sliderDP, sliderPP;

    Image[] imageEP;
    float fScaleEpHandle = 1;
    Text textEp, textAp;

    Image imageHP, imageHighlight, imageEmptyAP, imageAim1, imageAim2;
    RectTransform trHighlight, trPpHandle, trAim1, trAim2;
    int iPrePp;
    float fRotHighlight;
    float timerEp, timerAp;
    bool bHighlight;

    float fAlphaEp = 1f, fAlphaPp = 0f, fAlphaAp = 1f;
    float fFadeoutSpeed = 0.5f; // 전체 페이드아웃 속력

    Quaternion qAimRot;

    T2.Skill.SeventhFlow skillSeventhFlow;
    float coolTimerSeventhFlow;
    Slider[] sliderSkill;
    Image[] imageSkill1, imageSkill2, imageSkill3, imageSkill4;
    float[] timerSkill = new float[4];
    float[] fAlphaSkill = new float[4];

    void Awake()
    {
        t_mgr = oPlayer.GetComponent<T2.Manager>();
        sliderAP = oAimPanel.GetComponentInChildren<Slider>();
        imageEmptyAP = oEmptyAP.GetComponent<Image>();
        textAp = oEmptyAP.transform.FindChild("Text").GetComponent<Text>();
        trAim1 = oAimPanel.transform.FindChild("Aim1").GetComponent<RectTransform>();
        trAim2 = oAimPanel.transform.FindChild("Aim2").GetComponent<RectTransform>();
        imageAim1 = trAim1.GetComponent<Image>();
        imageAim2 = trAim2.GetComponent<Image>();
        imageEP = sliderEP.GetComponentsInChildren<Image>();
        textEp = sliderEP.GetComponentInChildren<Text>();
        sliderDP = oDpPp.transform.FindChild("DP").GetComponent<Slider>();
        sliderPP = oDpPp.transform.FindChild("PP").GetComponent<Slider>();
        imageHP = oDpPp.transform.FindChild("DpDanger").GetComponent<Image>();
        trHighlight = oDpPp.transform.FindChild("PpHighlight").GetComponent<RectTransform>();
        imageHighlight = trHighlight.GetComponent<Image>();
        trPpHandle = sliderPP.transform.FindChild("PpHandle").GetComponent<RectTransform>();
        sliderSkill = oSkill.GetComponentsInChildren<Slider>();
        imageSkill1 = sliderSkill[0].GetComponentsInChildren<Image>();
        skillSeventhFlow = oPlayer.GetComponent<T2.Skill.SeventhFlow>();
        imageSkill2 = sliderSkill[1].GetComponentsInChildren<Image>();
        imageSkill3 = sliderSkill[2].GetComponentsInChildren<Image>();
        imageSkill4 = sliderSkill[3].GetComponentsInChildren<Image>();
    }
    void Start()
    {
        sliderSkill[1].maxValue = skillSeventhFlow.coolTime;
        iPrePp = 100;
        for (int i = 0; i < 4; i++)
        {
            timerSkill[i] = 5;
            fAlphaSkill[i] = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        sliderAP.value = t_mgr.GetAP();
        sliderEP.value = t_mgr.GetEP();
        sliderPP.value = t_mgr.GetPP();
        sliderDP.value = t_mgr.GetDP();
        Ap();
        Ep();
        Hp();
        Pp();
        Skill();
    }

    void Skill()
    {
        if (skillSeventhFlow.bUsing)
        {
            coolTimerSeventhFlow = sliderSkill[1].maxValue;
            timerSkill[1] = 0;
        }
        SkillCooltimeComplite(imageSkill2, 1);
    }

    void SkillCooltimeComplite(Image[] _imageSkill, int _num)
    {
        sliderSkill[_num].value = SkillCooltime(ref coolTimerSeventhFlow);

        if (sliderSkill[_num].value.Equals(0))
        {
            timerSkill[_num] += Time.deltaTime;
            if (timerSkill[_num] <= 1 && timerSkill[_num] >= 0.01f)
            {
                _imageSkill[2].color = Color.white;
            }
            else if (timerSkill[_num] < 0.01f)
            {
                fAlphaSkill[_num] = 1;
            }
            else
            {
                _imageSkill[2].color = Color.clear;
            }
        }
        else if(sliderSkill[_num].value > sliderSkill[_num].maxValue - 1)
        {
            timerSkill[_num] = 10;
        }
        _imageSkill[3].color = new Color(1, 1, 1, FadeInOut(ref fAlphaSkill[_num], -10));
    }

    float SkillCooltime(ref float _time)
    {
        _time -= Time.deltaTime;
        return _time;
    }

    void Ap()
    {
        // 에임이 몬스터를 체크했을 때
        if (t_mgr.GetCheckAimForMonster())
        {
            imageAim1.color = Color.red;
            imageAim2.color = Color.red;
            qAimRot = Quaternion.Euler(oAimPanel.transform.eulerAngles.x, oAimPanel.transform.eulerAngles.y, oAimPanel.transform.eulerAngles.z + 45);
            trAim2.transform.rotation = Quaternion.Slerp(trAim2.transform.rotation, qAimRot, Time.deltaTime * 20);
            trAim2.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            trAim1.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        else
        {
            imageAim1.color = Color.white;
            imageAim2.color = Color.white;
            //trAim2.transform.rotation = Quaternion.Euler(oAimPanel.transform.eulerAngles.x, oAimPanel.transform.eulerAngles.y, oAimPanel.transform.eulerAngles.z);
            trAim2.transform.rotation = Quaternion.Slerp(
                trAim2.transform.rotation,
                Quaternion.Euler(oAimPanel.transform.eulerAngles.x, oAimPanel.transform.eulerAngles.y, oAimPanel.transform.eulerAngles.z),
                Time.deltaTime * 20);
            trAim2.transform.localScale = new Vector3(1, 1, 1);
            trAim1.transform.localScale = new Vector3(1, 1, 1);
        }

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
                    imageEP[i].color = new Color(1, 1, 1, FadeInOut(ref fAlphaEp, -1));
                }
                imageEP[2].rectTransform.localScale = new Vector3(1, 1, 1);
                textEp.color = new Color(1, 1, 1, FadeInOut(ref fAlphaEp, -1));
            }
        }
        else if (sliderEP.value < sliderEP.maxValue)
        {
            for (int i = 0; i < imageEP.Length; i++)
            {
                imageEP[i].color = Color.white;
            }
            imageEP[2].rectTransform.localScale = new Vector3(TransmuteImageScale(ref fScaleEpHandle), TransmuteImageScale(ref fScaleEpHandle), 1);
            textEp.color = Color.white;
            timerEp = 0;
            fAlphaEp = 1;
        }
    }

    float TransmuteImageScale(ref float _f)
    {
        _f -= Time.deltaTime;
        _f = Mathf.Clamp(_f, 0.5f, 1f);
        if (_f < 0.6) _f = 1;

        return _f;
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
        imageHighlight.color = new Color(1, 1, 1, FadeInOut(ref fAlphaPp, -1.5f));

        iPrePp = (int)sliderPP.value;

        float fPi = 185 - (1.8f * sliderPP.value); // 185인 이유는 핸들이 제대로 못 덮기 때문이다
        trPpHandle.transform.rotation = Quaternion.Euler(
                oDpPp.transform.eulerAngles.x,
                oDpPp.transform.eulerAngles.y,
                (360 - oDpPp.transform.eulerAngles.z) + fPi);
    }
}
