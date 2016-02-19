using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
	public GameObject tilePrefab;

	int gridWidth = 4;
	int gridHeight = 4;
	int spriteSize = 312;

	void Start () {
		PopulateTiles();
	}

	void PopulateTiles(){
		System.Random randomSequence = new System.Random(1234);
		for (int col = 0; col < gridWidth; col++) {
			for (int row = 0; row < gridWidth; row++) {
				GameObject tile = (GameObject)Instantiate(tilePrefab, new Vector3(col * spriteSize*2, row * spriteSize*2), Quaternion.identity);			
				tile.GetComponent<TileChoice>().SetId(randomSequence.Next());
			}
		}
	}


	GameObject mouseDownStartTile;
	public void MouseDownInTile(GameObject tile){
		mouseDownStartTile = tile;
	}

	GameObject mouseInTile;
	public void MouseInTile(GameObject tile){
		mouseInTile = tile;
	}

	public void MouseUp(){
		print ("up: " + mouseInTile.GetComponent<TileChoice>().GetId());
		if(mouseInTile.GetComponent<TileChoice>().GetId() == mouseDownStartTile.GetComponent<TileChoice>().GetId()){
			mouseInTile.GetComponent<TileChoice>().rotateTile();
		}
	}

	void OnMouseDown(){
		print ("clicked on BoardManager");
	}
	
	void Update () {
	
	}
}
