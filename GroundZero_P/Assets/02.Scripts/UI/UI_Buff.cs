using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Buff : MonoBehaviour {
    
    Image[] image = new Image[10];
    public int iCountBuff; // 현재 버프 개수
    int iInterval = 54;

    void Awake ()
    {
        image = transform.GetComponentsInChildren<Image>();
	}

    void Start()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        // if 어떤 상태가 된다면
        // 이미지를 바꾸고
        // 상태 개수에 따라 나열 위치 변동
        // 추가가 될 때마다 각 위치 (iInterval)


        // 나열
        transform.localPosition = new Vector3 (
            iInterval * (iCountBuff + 1) * 0.25f,
            transform.localPosition.y,
            transform.localPosition.z);
        for (int i = iCountBuff; i < image.Length; i++)
        {
            if (image[i].enabled.Equals(true))
                image[i].enabled = false;
        }
        for (int i = 0; i < iCountBuff; i++)
        {
            if (image[i].enabled.Equals(false))
                image[i].enabled = true;
            image[i].rectTransform.localPosition = new Vector3(
                transform.localPosition.x - (iInterval * (iCountBuff - i)),
                image[i].rectTransform.localPosition.y,
                image[i].rectTransform.localPosition.z);
        }
    }
}
