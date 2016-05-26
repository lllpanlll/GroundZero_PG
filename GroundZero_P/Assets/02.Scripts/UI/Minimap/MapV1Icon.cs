using UnityEngine;
using System.Collections;

public class MapV1Icon : MonoBehaviour {

    Transform trMapUI;

    void Awake()
    {
        trMapUI = transform.GetChild(0).GetComponent<Transform>();
    }
	// Use this for initialization
	void Start () {
        //oMapUI = Instantiate(oMapUI, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        //oMapUI.transform.parent = transform;
        trMapUI.localRotation = Quaternion.Euler(90, 0, 0);
        //oMapUI.transform.localScale = new Vector3(1, 1, 1); // 안전 장치.
    }
	
	// Update is called once per frame
	void Update () {
        trMapUI.position = new Vector3(transform.position.x, -10, transform.position.z);
	}
}
