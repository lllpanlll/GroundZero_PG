using UnityEngine;
using System.Collections;

/// <summary>
/// 2016-05-04
/// 캐릭터의 SeventhFlow스킬로 인해 생기는 잔상의 생존시간을 구현한 클래스.
/// 
/// 특정 기능이 없는 관계로 SetLifeTimer클래스로 대체하도록 한다.
/// </summary>
public class AfterImageLifeTime : MonoBehaviour {

    public float lifeTime = 3.0f;
    private float lifeTimer = 0.0f;

    // Use this for initialization
    void Start () {
        //StartCoroutine(LifeTimer(lifeTime));
	}

    void OnEnable()
    {
        lifeTimer = 0.0f;
    }

    void FixedUpdate()
    {
        if (lifeTimer > lifeTime)
        {
            gameObject.SetActive(false);
        }
        else
            lifeTimer += Time.deltaTime;
    }

    //IEnumerator LifeTimer(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    this.gameObject.SetActive(false);
    //}
}
