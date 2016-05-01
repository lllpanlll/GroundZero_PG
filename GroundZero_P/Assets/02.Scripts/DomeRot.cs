using UnityEngine;
using System.Collections;

public class DomeRot : MonoBehaviour {

    public float[] rot;
    public GameObject[] met;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    for(int i =0; i<met.Length; i++)
        {
            met[i].transform.Rotate(Vector3.forward, rot[i]);
        }
	}
}
