using UnityEngine;
using System.Collections;

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
