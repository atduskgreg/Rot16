using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour {
	public static int EmptyTileId = 16;

	public int tileId = 16;
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
		{"4+7", 8},
		{"7+4", 8},
		{"7+6", 11},
		// three-sided squares. not allowed because lame
//		{"7+9", 12},
//		{"9+7", 12},
//		{"5+8", 12},
//		{"8+5", 12},
//		{"6+8", 15},
//		{"8+6", 15},
//		{"6+9", 13},
//		{"9+6", 13},
//		{"4+10", 13},
//		{"10+4", 13},
//		{"10+7", 14},
//		{"7+10", 14},
//		{"4+11", 15},
//		{"11+4", 15},
//		{"5+11", 14},
//		{"11+5", 14},
		{"9+11", 3},
		{"11+9", 3},
		{"8+10", 3},
		{"10+8", 3},
		{"12+6", 3},
		{"6+12", 3},
		{"7+13", 3},
		{"13+7", 3},
		{"4+14", 3},
		{"14+4", 3},
		{"15+5", 3},
		{"5+15", 3}
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

	// if this is not an empty tile
	// it can move into the place of empty tiles
	public bool CanMoveInto(Tile tile){
		return (tileId != Tile.EmptyTileId) && (tile.tileId == Tile.EmptyTileId);
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
		updateSprite();
	}

	void updateSprite(){
		Sprite[] sprites =  Resources.LoadAll<Sprite>("pieces");
		GetComponent<SpriteRenderer>().sprite = sprites[tileId];
	}

	void Start () {
		boardManager = GameObject.Find("GameManager").GetComponent<BoardManager>();
		updateSprite();
	}
	
	void Update () {
		
	}
	

	void OnMouseEnter(){
		boardManager.MouseInTile(GetComponent<Tile>());
	}
 
	void OnMouseDown(){

		boardManager.MouseDownInTile(GetComponent<Tile>());

		List<Vector2[]> nodeConnections = PathFinder.ConnectedNodesForTileId(tileId);
		for(int i = 0; i < nodeConnections.Count; i++){
			int fromNodeCol = (int)nodeConnections[i][0].x + col *2;
			int fromNodeRow = (int)nodeConnections[i][0].y + row *2;
			
			int toNodeCol = (int)nodeConnections[i][1].x + col *2;
			int toNodeRow = (int)nodeConnections[i][1].y + row *2;
		}
	}
	
	void OnMouseUp(){
		boardManager.MouseUp();
	}
}
