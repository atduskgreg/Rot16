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

	public Vector3 canonicalPosition;
	
	BoardManager boardManager;
	GameObject tileDisplay;

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

	public void MoveTo(Vector3 newPos) {
		tileDisplay.transform.position = newPos;
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
		// FIXME: we find and set this in Start() why do we have to do this again here?
		tileDisplay = transform.FindChild("TileDisplay").gameObject;
		tileDisplay.GetComponent<SpriteRenderer>().sprite = sprites[tileId];
	}

	public void DisableCollider(){
		GetComponent<BoxCollider2D>().enabled = false;
	}

	public void EnableCollider(){
		GetComponent<BoxCollider2D>().enabled = true;
	}

	public void ResetToCanonicalPosition(){
		tileDisplay.transform.position = canonicalPosition;
	}

	public void MakeTransparent(){
		tileDisplay.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
	}

	public void MakeOpaque(){
		tileDisplay.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);

	}

	void Start () {
		boardManager = GameObject.Find("GameManager").GetComponent<BoardManager>();
		tileDisplay = transform.FindChild("TileDisplay").gameObject;
		updateSprite();
		canonicalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}
	
	void Update () {}
	
    public bool SameTile(Tile otherTile) {
        return SameRow(otherTile) && SameColumn(otherTile);
    }

	public bool SameRow(Tile otherTile){
		return row == otherTile.row;
	}

	public bool SameColumn(Tile otherTile){
		return col == otherTile.col;
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
