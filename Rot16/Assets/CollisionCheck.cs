using UnityEngine;
using System.Collections;

public class CollisionCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll){
		Debug.Log("collision: " + coll.gameObject.GetComponent<Tile>().tileId);
	}
}
