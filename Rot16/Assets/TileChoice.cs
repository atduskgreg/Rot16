using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileChoice : MonoBehaviour {
	public int tileId;
	public int[] allowedStartingTiles;

	int id;

	BoardManager boardManager;

	public static Dictionary<string, int> combinationRules = new Dictionary< string, int>(){
		{"0+1", 2},
		{"1+0", 2},
		{"4+5", 9},
		{"5+4", 9},
		{"5+6", 10},
		{"6+5", 10},
		{"6+7", 11},
		{"7+6", 11}
	};

	public void assignStartingTile(){
		tileId = allowedStartingTiles[Random.Range(0, allowedStartingTiles.Length)];
		setTileId(tileId);
	}

	public void rotateTile(){
		Dictionary<int, int> rotDict = new Dictionary<int, int>(){
			{0, 1},
			{1, 0},
			{2,2},
			{3,3},
			{4,5},
			{5,6},
			{6,7},
			{7,4},
			{8,9},
			{9,10},
			{10,11},
			{11,8},
			{12,13},
			{13,14},
			{14,15},
			{15,12}
		};

		setTileId (rotDict[tileId]);
	}

	public bool canCombineWith(TileChoice tile){


		return false;
	}

	void setTileId(int newTileId){
		tileId = newTileId;
		Sprite[] sprites =  Resources.LoadAll<Sprite>("pieces");
		GetComponent<SpriteRenderer>().sprite = sprites[tileId];
	}

	public void SetId(int i){
		id = i;
	}

	public int GetId(){
		return id;
	}
	void Start () {
		boardManager = GameObject.Find("GameManager").GetComponent<BoardManager>();
		assignStartingTile();
	}
	
	void Update () {
		
	}
	

	void OnMouseEnter(){
		boardManager.MouseInTile(gameObject);
	}
 
	void OnMouseDown(){
		print ("down: " + id + " tileId " + tileId);
		boardManager.MouseDownInTile(gameObject);
	}
	
	void OnMouseUp(){
		boardManager.MouseUp();
	}
}
