using UnityEngine;
using System.Collections;

public class UI_Manager : MonoBehaviour {

    static UI_Manager instance;
    public static UI_Manager Instance
    {
        get{
            if (instance.Equals(null))
            {
                instance = FindObjectOfType<UI_Manager>();
#if UNITY_EDITOR
                if (FindObjectsOfType<UI_Manager>().Length > 1)
                    Debug.LogError("여러개임.");
#endif
            }
            return instance;
        }
    }

    T2.Manager t2Mgr;
    M_AICore mAicore;
    public GameObject oStart_UI; // 플레이 씬 첫 UI // 나중에 문제가 생길 수 있을지도.
    public GameObject oEnd_UI;

    
    void Awake() // 나중에 문제가 생길 수 있을지도. warning
    {
        t2Mgr = FindObjectOfType<T2.Manager>();
        mAicore = FindObjectOfType<M_AICore>();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Application.LoadLevel(0); // OnLevelWasLoaded 테스트용으로 써봄. 

        // 나중에 OnLevelWasLoaded() 쪽으로 옮겨줘야함.
        StartCoroutine(EffectUi(oStart_UI, 8));
        StartCoroutine(CharacterStop(4));
    }

    void Update()
    {
        // 나중에 SendMessage 같은걸로 '몬스터가 죽었을 때' 처리하도록 바꿔야함.
    }

    void OnLevelWasLoaded(int _level)
    {
        if (_level.Equals(0))
        {
            print("OnLevelWasLoaded(0)");
        }
    }

    IEnumerator CharacterStop(float _time)
    {
        t2Mgr.ChangeState(T2.Manager.State.be_Shot);
        t2Mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, false);
        yield return new WaitForSeconds(_time);
        t2Mgr.ChangeState(T2.Manager.State.idle);
    }

    IEnumerator EffectUi(GameObject _Ui, float _time)
    {
        _Ui.SetActive(true);
        yield return new WaitForSeconds(_time);
        _Ui.SetActive(false);
    }
}
