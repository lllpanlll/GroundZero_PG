using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Stat : MonoBehaviour
{
    T2.Manager t_mgr;
    public GameObject oPlayer, oAimPanel, oDpPp, oEmptyAP, oSkill;

    // EP
    public Slider sliderEP;
    Image[] imageEP;
    float fScaleEpHandle = 2;
    Text textEp;
    float timerEp = 0;
    // AP
    Slider sliderAP;
    Text textAp;
    Image imageEmptyAP, imageAim1, imageAim2;
    RectTransform trAim1, trAim2;
    float timerAp = 0;
    Quaternion qAimRot;
    // PP
    Slider sliderPP;
    Image imageHighlight;
    RectTransform trHighlight, trPpHandle;
    int iPrePp = 0;
    float fRotHighlight = 0;
    bool bHighlight = false;
    // DP
    Slider sliderDP;
    Image imageHP;   // DP UI에만
    Image imageRisk; // 화면전체

    float fAlphaEp = 1f, fAlphaPp = 0f, fAlphaAp = 1f;
    float fFadeoutSpeed = 0.5f; // 전체 페이드아웃 속력인데 방향 변수에 속도 들어가서 FadeInOut 검색 요망

    // Skills
    Slider[] sliderSkill;
    Image[] imageSkill1, imageSkill2, imageSkill3, imageSkill4;
    float[] timerSkill = new float[4];
    float[] fAlphaSkill = new float[4];

    public static UI_Stat instance = null;

    void Awake()
    {
        instance = this;

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
        imageRisk = transform.FindChild("Risk_Vignette").GetComponent<Image>();
        trHighlight = oDpPp.transform.FindChild("PpHighlight").GetComponent<RectTransform>();
        imageHighlight = trHighlight.GetComponent<Image>();
        trPpHandle = sliderPP.transform.FindChild("PpHandle").GetComponent<RectTransform>();
        sliderSkill = oSkill.GetComponentsInChildren<Slider>();
        imageSkill1 = sliderSkill[0].GetComponentsInChildren<Image>(); // 1번째 스킬칸
        imageSkill2 = sliderSkill[1].GetComponentsInChildren<Image>(); // 2번째 스킬칸
        imageSkill3 = sliderSkill[2].GetComponentsInChildren<Image>(); // 3번째 스킬칸
        imageSkill4 = sliderSkill[3].GetComponentsInChildren<Image>(); // 4번째 스킬칸
    }

    void Start()
    {
        sliderAP.maxValue = T2.Stat.MAX_AP;
        sliderEP.maxValue = T2.Stat.MAX_EP;
        sliderPP.maxValue = T2.Stat.MAX_PP;
        sliderDP.maxValue = T2.Stat.MAX_DP;
        sliderSkill[0].maxValue = T2.Skill.DimensionBall.GetInstance().coolTime;
        sliderSkill[1].maxValue = T2.Skill.SeventhFlow.GetInstance().coolTime;
        sliderSkill[3].maxValue = T2.Skill.SilverStream.GetInstance().coolTime;
        iPrePp = (int)sliderPP.maxValue;
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
        //ShortPoint();
    }

    public void CoolTime(T2.Skill.Skill skill)
    {
        //T2.Skill.DimensionBall skil = T2.Skill.DimensionBall.GetInstance();
        //UI_Stat.instance.CoolTime(skil);

        if (skill.Equals(T2.Skill.DimensionBall.GetInstance()))
        {
            StartCoroutine(CoolDown(T2.Skill.DimensionBall.GetInstance().coolTime, 0));
        }
        else if (skill.Equals(T2.Skill.SeventhFlow.GetInstance()))
        {
            StartCoroutine(CoolDown(T2.Skill.SeventhFlow.GetInstance().coolTime, 1));
        }
        else if (skill.Equals(T2.Skill.SilverStream.GetInstance()))
        {
            StartCoroutine(CoolDown(T2.Skill.SilverStream.GetInstance().coolTime, 3));
        }
    }

    public void CoolTimeForDimensionball(T2.Pref.DimensionBallPref pref)
    {
        
    }

    IEnumerator CoolDown(float _num, int _slider)
    {
        float f = _num;
        while (f >= 0)
        {
            f -= Time.deltaTime;
            sliderSkill[_slider].value = f;
            yield return new WaitForSeconds(0.01f);
        }
        yield break;
    }

    // 쿨타임때 회색으로
    //void ShortPoint()
    //{
    //    if (!t_mgr.PointCheck(T2.Skill.DimensionBall.GetInstance().PointType, T2.Skill.DimensionBall.GetInstance().iDecPoint))
    //    {
    //        imageSkill1[0].color = new Color(0.1f, 0.1f, 0.1f);
    //    }
    //    else
    //        imageSkill1[0].color = Color.white;

    //    if (!t_mgr.PointCheck(T2.Skill.SeventhFlow.GetInstance().PointType, T2.Skill.SeventhFlow.GetInstance().iDecPoint))
    //    {
    //        imageSkill2[0].color = new Color(0.1f, 0.1f, 0.1f);
    //    }
    //    else
    //        imageSkill2[0].color = Color.white;

    //    if (!t_mgr.PointCheck(T2.Skill.SilverStream.GetInstance().PointType, T2.Skill.SilverStream.GetInstance().iDecPoint))
    //    {
    //        imageSkill4[0].color = new Color(0.1f, 0.1f, 0.1f);
    //    }
    //    else
    //        imageSkill4[0].color = Color.white;
    //}

    void Skill()
    {
        if (T2.Skill.DimensionBall.GetInstance().bCoolTime)
        {
            imageSkill1[0].color = Color.blue;
        }
        else if (!T2.Skill.DimensionBall.GetInstance().bCoolTime)
        {
            imageSkill1[0].color = Color.white;
        }

        if (T2.Skill.SilverStream.GetInstance().bSilverStream)
        {
            imageSkill4[0].color = Color.blue;
        }
        else
        {
            imageSkill4[0].color = Color.white;
        }

        UiSkillEffect(imageSkill1, 0);
        UiSkillEffect(imageSkill2, 1);
        UiSkillEffect(imageSkill3, 2);
        UiSkillEffect(imageSkill4, 3);
    }

    // 스킬쿨타임 완료체크 (ex => UiSkillEffect(imageSkill2, 1)) = 2번째 스킬인 세븐쓰플로우를 뜻함 // 불편하다 바꾸고 싶다아.
    // _imageSkill[n]에서 2는 테두리 하이라이트, 3은 boom이펙트.
    void UiSkillEffect(Image[] _imageSkill, int _num)
    {
        if (sliderSkill[_num].value.Equals(0))
        {
            timerSkill[_num] += Time.deltaTime;
            if (timerSkill[_num] <= 1 && timerSkill[_num] >= 0.1f) // 1초 동안 스킬의 쿨타임이 완료 됐음을 알림.
            {
                _imageSkill[2].color = Color.white;
            }
            else if (timerSkill[_num] < 0.1f) // 스킬의 쿨타임이 완료 되면 잠깐동안 boom 이펙트.
            {
                fAlphaSkill[_num] = 1;
            }
            else
            {
                _imageSkill[2].color = Color.clear;
            }
        }
        else
        {
            timerSkill[_num] = 0;
            _imageSkill[2].color = Color.clear;
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
            imageEP[2].rectTransform.localScale = new Vector3(1,1,1);
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
        _f = Mathf.Clamp(_f, 0.5f, 1.5f);
        if (_f < 0.6) _f = 1.5f;

        return _f;
    }

    float FadeInOut(ref float _fKindOfStat, float _iDir)
    {
        _fKindOfStat += _iDir * fFadeoutSpeed * Time.deltaTime;
        _fKindOfStat = Mathf.Clamp01(_fKindOfStat);
        return _fKindOfStat;
    }

    void Hp()
    {
        if (sliderDP.value.Equals(0))
        {
            imageHP.enabled = true;
            imageRisk.enabled = true;
        }
        else
        {
            imageHP.enabled = false;
            imageRisk.enabled = false;
        }

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

        float _fPi = 180f - (1.8f * sliderPP.value);

        trPpHandle.transform.localRotation = Quaternion.Euler(
                0,
                0,
                _fPi);
    }
}
