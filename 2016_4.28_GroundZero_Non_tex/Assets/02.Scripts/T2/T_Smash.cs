using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class T_Smash : MonoBehaviour {

    public GameObject _smash1;
    public GameObject _smash2;
    public GameObject _smash3;
    public GameObject _smash_sel; // 선택 시 하이라이트용,

    Vector3 _point;
    Vector3 _center;
    float _degree_mousePos;
    float _x, _y;
    int _select; // 스매쉬 스킬을 고를 시 값 저장 0=nope! 1=스매쉬1 2=스매쉬2 3=스매쉬3,

    // Use this for initialization
    void Awake()
    {
        _smash1.GetComponent<GameObject>();
        _smash2.GetComponent<GameObject>();
        _smash3.GetComponent<GameObject>();

        _center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
    }

    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        _point = Input.mousePosition;
        _x = _center.x - _point.x;
        _y = _center.y - _point.y;
        _degree_mousePos = (Mathf.Atan2(_y, _x) * Mathf.Rad2Deg) + 180f;
        Debug.DrawRay(_center, (_point - _center), Color.green);
        print("degree : "+_degree_mousePos);

        if (_degree_mousePos <= 90f && _degree_mousePos > 30f)
        {
            _select = 1;
            _smash_sel.transform.position = _smash1.transform.position;
        }
        else if (_degree_mousePos <= 30f || _degree_mousePos > 330f)
        {
            _select = 2;
            _smash_sel.transform.position = _smash2.transform.position;
        }
        else if (_degree_mousePos <= 330f && _degree_mousePos > 270f)
        {
            _select = 3;
            _smash_sel.transform.position = _smash3.transform.position;
        }
        else
        {
            _select = 0;
            _smash_sel.transform.position = new Vector3(0, 0); // 선택을 안했을 시 임시용,
        }
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        switch (_select)
        {
            case 0:
                print("스킬 못 써! 안 써!");
                break;
            case 1:
                print("스매쉬1이 슈슈슝!");
                break;
            case 2:
                print("스매쉬2가 송송송!");
                break;
            case 3:
                print("스매쉬3이 뿅뿅뿅!");
                break;
        }
    }
}
