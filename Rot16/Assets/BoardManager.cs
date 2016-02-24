using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {
	public int numStartingTiles = 2;
	public Text HorizontalScoreText;
	public Text VerticalScoreText;


	public GameObject tilePrefab;
	PathFinder pathFinder;

	private Tile[,] AllTiles = new Tile[4,4];
	private List <Tile[]> columns = new List<Tile[]> ();
	private List <Tile[]> rows = new List<Tile[]> ();

	int gridWidth = 4;
	int gridHeight = 4;
	int spriteSize = 312;

	void Start () {
		PopulateTiles();
		pathFinder = GetComponent<PathFinder>();
		pathFinder.LoadTileSet(AllTiles);
	}

	void UpdateScoreDisplay(){
		HorizontalScoreText.text = "" + HorizontalScore();
		VerticalScoreText.text = "" + VerticalScore();
	}

	int VerticalScore(){
		int score = 0;

		Vector2[] startPoints = {new Vector2(1,0), new Vector2(3,0), new Vector2(5,0), new Vector2(7,0)};
		Vector2[] endPoints = {new Vector2(1,8), new Vector2(3,8), new Vector2(5,8), new Vector2(7,8)};

		for(int i = 0; i < startPoints.Length; i++){
			for(int j = 0; j < endPoints.Length; j++){
				if(pathFinder.GetPath(startPoints[i], endPoints[j]).Exists){
					score++;
				}
			}
		}

		return score;
	}

	int HorizontalScore(){
		int score = 0;
		
		Vector2[] startPoints = {new Vector2(0,1), new Vector2(0,3), new Vector2(0,5), new Vector2(0,7)};
		Vector2[] endPoints = {new Vector2(8,1), new Vector2(8,3), new Vector2(8,5), new Vector2(8,7)};
		
		for(int i = 0; i < startPoints.Length; i++){
			for(int j = 0; j < endPoints.Length; j++){
				if(pathFinder.GetPath(startPoints[i], endPoints[j]).Exists){
					score++;
				}
			}
		}
		
		return score;
	}



	void PopulateTiles(){
		for (int col = 0; col < gridWidth; col++) {
			for (int row = 0; row < gridHeight; row++) {
				GameObject go = (GameObject)Instantiate(tilePrefab, new Vector3(col * spriteSize*2, row * spriteSize*2), Quaternion.identity);			
				Tile tile = go.GetComponent<Tile>();

				tile.row = row;
				tile.col = col;
				AllTiles[row, col] = tile;
			}
		}

		List<int> placedTileIndices = new List<int>();
		while(placedTileIndices.Count < numStartingTiles){
			int idx = Random.Range (0, AllTiles.Length-1);
			if(!placedTileIndices.Contains(idx)){

				int row = idx / 4;
				int col = idx % 4;

				AllTiles[row,col].assignStartingTile();

				placedTileIndices.Add(idx);
			}
		}

		columns.Add (new Tile[]{AllTiles [0, 0], AllTiles [1, 0], AllTiles [2, 0], AllTiles [3, 0]});
		columns.Add (new Tile[]{AllTiles [0, 1], AllTiles [1, 1], AllTiles [2, 1], AllTiles [3, 1]});
		columns.Add (new Tile[]{AllTiles [0, 2], AllTiles [1, 2], AllTiles [2, 2], AllTiles [3, 2]});
		columns.Add (new Tile[]{AllTiles [0, 3], AllTiles [1, 3], AllTiles [2, 3], AllTiles [3, 3]});
		
		rows.Add (new Tile[]{AllTiles [0, 0], AllTiles [0, 1], AllTiles [0, 2], AllTiles [0, 3]});
		rows.Add (new Tile[]{AllTiles [1, 0], AllTiles [1, 1], AllTiles [1, 2], AllTiles [1, 3]});
		rows.Add (new Tile[]{AllTiles [2, 0], AllTiles [2, 1], AllTiles [2, 2], AllTiles [2, 3]});
		rows.Add (new Tile[]{AllTiles [3, 0], AllTiles [3, 1], AllTiles [3, 2], AllTiles [3, 3]});
	}

	bool MakeOneMoveDownIndex(Tile[] LineOfTiles){
		for (int i =0; i< LineOfTiles.Length-1; i++) 
		{
			//MOVE BLOCK 
			// move into empty spaces.
			if (LineOfTiles[i].isEmpty()  && !LineOfTiles[i+1].isEmpty()){
				LineOfTiles[i].setTileId(LineOfTiles[i+1].tileId);
				LineOfTiles[i+1].setTileId(Tile.EmptyTileId);
				return true;
			}
			// MERGE BLOCK
			if (!LineOfTiles[i].isEmpty() && LineOfTiles[i].CanCombineWith(LineOfTiles[i+1]) &&
			    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i+1].mergedThisTurn == false){

				LineOfTiles[i].CombineWith(LineOfTiles[i+1]);
				LineOfTiles[i+1].setTileId(Tile.EmptyTileId);
				LineOfTiles[i].mergedThisTurn = true;
				return true;
			}
		}
		return false;
	}

	bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
	{
		for (int i =LineOfTiles.Length-1; i > 0; i--) {
			//MOVE BLOCK 
			if (LineOfTiles[i].isEmpty() && !LineOfTiles[i-1].isEmpty ()){

				LineOfTiles[i].setTileId(LineOfTiles[i-1].tileId);
				LineOfTiles[i-1].setTileId(Tile.EmptyTileId);
				return true;
			}
			// MERGE BLOCK
			if (!LineOfTiles[i].isEmpty() && LineOfTiles[i].CanCombineWith(LineOfTiles[i-1]) &&
			    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i-1].mergedThisTurn == false)
			{
				LineOfTiles[i].CombineWith(LineOfTiles[i-1]);
				LineOfTiles[i-1].setTileId(Tile.EmptyTileId);
				LineOfTiles[i].mergedThisTurn = true;
				return true;
			}
		}
		return false;
	}


	private void ResetMergedFlags(){
		foreach (Tile t in AllTiles){
			t.mergedThisTurn = false;
		}
	}

	Tile mouseDownStartTile;
	public void MouseDownInTile(Tile tile){
		mouseDownStartTile = tile;
	}

	Tile mouseInTile;
	public void MouseInTile(Tile tile){
		mouseInTile = tile;
	}

	public void MouseUp(){
		if(mouseInTile.row == mouseDownStartTile.row && mouseInTile.col == mouseDownStartTile.col){
			mouseInTile.GetComponent<Tile>().rotateTile();
			pathFinder.LoadTileSet(AllTiles);
			UpdateScoreDisplay();

			return;
		} 

		ResetMergedFlags();

		if(mouseInTile.row == mouseDownStartTile.row){
			print ("slide row " +mouseInTile.row +" col: " + mouseDownStartTile.col + " -> " + mouseInTile.col );

			if(mouseInTile.col < mouseDownStartTile.col){
				bool moveMade = false;
				while (MakeOneMoveDownIndex(rows[mouseInTile.row])) {
					moveMade = true;
				}

				if(moveMade){
					print ("move down row");
					int lastIndex = rows[mouseInTile.row].Length - 1;
					rows[mouseInTile.row][lastIndex].assignStartingTile();
				}


			} else {
				bool moveMade = false;
				while (MakeOneMoveUpIndex(rows[mouseInTile.row])) {
					moveMade = true;
				}

				if(moveMade){
					print ("move up row");
					rows[mouseInTile.row][0].assignStartingTile();
				}

			}
			pathFinder.LoadTileSet(AllTiles);
			UpdateScoreDisplay();
			return;
		}

		if(mouseInTile.col == mouseDownStartTile.col){
			print ("slide col " +mouseInTile.col +" row: " + mouseDownStartTile.row + " -> " + mouseInTile.row );

			if(mouseInTile.row < mouseDownStartTile.row){
				bool moveMade = false;
				while (MakeOneMoveDownIndex(columns[mouseInTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					print ("move down col");
					int lastIndex = columns[mouseInTile.col].Length - 1;
					columns[mouseInTile.col][lastIndex].assignStartingTile();
				}

			} else {
				bool moveMade = false;
				while (MakeOneMoveUpIndex(columns[mouseInTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					print ("move up col");
					columns[mouseInTile.col][0].assignStartingTile();
				}

			}
			pathFinder.LoadTileSet(AllTiles);
			UpdateScoreDisplay();

			return;
		}

		print ("no slide");
	}


	void Update () {
	
	}
}
