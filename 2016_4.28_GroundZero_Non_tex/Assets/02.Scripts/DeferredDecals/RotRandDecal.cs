using UnityEngine;
using System.Collections;

public class RotRandDecal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.Rotate(0,Random.Range(0f,360f),0);
	}

}
