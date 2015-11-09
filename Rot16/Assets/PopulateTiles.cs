using UnityEngine;
using System.Collections;

public class PopulateTiles : MonoBehaviour {
	public GameObject tilePrefab;

	int gridWidth = 4;
	int gridHeight = 4;
	int spriteSize = 312;

	void Start () {
		for (int col = 0; col < gridWidth; col++) {
			for (int row = 0; row < gridWidth; row++) {
				GameObject tile = (GameObject)Instantiate(tilePrefab, new Vector3(col * spriteSize*2, row * spriteSize*2), Quaternion.identity);
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
