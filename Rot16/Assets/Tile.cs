using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour {
	public static int EmptyTileId = 16;

	public int tileId;
	public int[] allowedStartingTiles;
	public int row;
	public int col;
	public bool mergedThisTurn = false;
	
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

		if(!isEmpty()){
			setTileId (rotDict[tileId]);
		}
	}

	public bool CanCombineWith(Tile tile){
		string combineKey = tileId + "+" + tile.tileId;
		return combinationRules.ContainsKey(combineKey);
	}

	public void CombineWith(Tile tile){
		string combineKey = tileId + "+" + tile.tileId;
		setTileId(combinationRules[combineKey]);
	}

	public bool isEmpty(){
		return tileId == Tile.EmptyTileId;
	}

	public void setTileId(int newTileId){
		tileId = newTileId;
		Sprite[] sprites =  Resources.LoadAll<Sprite>("pieces");
		GetComponent<SpriteRenderer>().sprite = sprites[tileId];
	}

	void Start () {
		boardManager = GameObject.Find("GameManager").GetComponent<BoardManager>();
		assignStartingTile();
	}
	
	void Update () {
		
	}
	

	void OnMouseEnter(){
		boardManager.MouseInTile(GetComponent<Tile>());
	}
 
	void OnMouseDown(){
		boardManager.MouseDownInTile(GetComponent<Tile>());
	}
	
	void OnMouseUp(){
		boardManager.MouseUp();
	}
}
