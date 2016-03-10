using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Vectrosity;

public class BoardManager : MonoBehaviour {
	public int numStartingTiles = 2;
	public Text HorizontalScoreText;
	public Text VerticalScoreText;
	public Text VictoryText;
	public Tile NextTile;

	public Material VerticalLineMaterial;
	public Material HorizontalLineMaterial;

	List<Path> VerticalPaths = new List<Path>();
	List<Path> HorizontalPaths = new List<Path>(); 
	List<VectorLine> scoreLines = new List<VectorLine>();

	public GameObject tilePrefab;
	PathFinder pathFinder;

	public Tile[,] AllTiles = new Tile[4,4];
	private List <Tile[]> columns = new List<Tile[]> ();
	private List <Tile[]> rows = new List<Tile[]> ();

	private int horizontalScore = 0;
	private int verticalScore = 0;

	int gridWidth = 4;
	int gridHeight = 4;
	int spriteSize = 312;

    bool mouseLeftTile;

    void Start () {
		PopulateTiles();
		pathFinder = GetComponent<PathFinder>();
		pathFinder.LoadTileSet(AllTiles);
		NextTile.assignStartingTile();
	}

	void UpdateScoreDisplay(){
		horizontalScore = HorizontalScore();
		verticalScore = VerticalScore();

		HorizontalScoreText.text = "" + horizontalScore;
		VerticalScoreText.text = "" + verticalScore;
	}

	// TODO: capture scoring paths and draw them using the lineRenderers

	int VerticalScore(){
		int score = 0;
		VerticalPaths.Clear();

		Vector2[] startPoints = {new Vector2(1,0), new Vector2(3,0), new Vector2(5,0), new Vector2(7,0)};
		Vector2[] endPoints = {new Vector2(1,8), new Vector2(3,8), new Vector2(5,8), new Vector2(7,8)};

		for(int i = 0; i < startPoints.Length; i++){
			for(int j = 0; j < endPoints.Length; j++){
				Path verticalPath = pathFinder.GetPath(startPoints[i], endPoints[j]);
				if(verticalPath.Exists){
					score++;
					VerticalPaths.Add(verticalPath);
				}
			}
		}

		return score;
	}

	int HorizontalScore(){
		int score = 0;
		HorizontalPaths.Clear ();

		Vector2[] startPoints = {new Vector2(0,1), new Vector2(0,3), new Vector2(0,5), new Vector2(0,7)};
		Vector2[] endPoints = {new Vector2(8,1), new Vector2(8,3), new Vector2(8,5), new Vector2(8,7)};
		
		for(int i = 0; i < startPoints.Length; i++){
			for(int j = 0; j < endPoints.Length; j++){
				Path horizontalPath =  pathFinder.GetPath(startPoints[i], endPoints[j]);
				if(horizontalPath.Exists){
					score++;
					HorizontalPaths.Add(horizontalPath);
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

        AllTiles[1, 1].assignStartingTile();
        AllTiles[1, 2].assignStartingTile();
        AllTiles[2, 1].assignStartingTile();
        AllTiles[2, 2].assignStartingTile();

		columns.Add (new Tile[]{AllTiles [0, 0], AllTiles [1, 0], AllTiles [2, 0], AllTiles [3, 0]});
		columns.Add (new Tile[]{AllTiles [0, 1], AllTiles [1, 1], AllTiles [2, 1], AllTiles [3, 1]});
		columns.Add (new Tile[]{AllTiles [0, 2], AllTiles [1, 2], AllTiles [2, 2], AllTiles [3, 2]});
		columns.Add (new Tile[]{AllTiles [0, 3], AllTiles [1, 3], AllTiles [2, 3], AllTiles [3, 3]});
		
		rows.Add (new Tile[]{AllTiles [0, 0], AllTiles [0, 1], AllTiles [0, 2], AllTiles [0, 3]});
		rows.Add (new Tile[]{AllTiles [1, 0], AllTiles [1, 1], AllTiles [1, 2], AllTiles [1, 3]});
		rows.Add (new Tile[]{AllTiles [2, 0], AllTiles [2, 1], AllTiles [2, 2], AllTiles [2, 3]});
		rows.Add (new Tile[]{AllTiles [3, 0], AllTiles [3, 1], AllTiles [3, 2], AllTiles [3, 3]});
	}

	bool TilesCanSlide(Tile[] tiles){
		bool result = false;
		for(int i = 0; i < tiles.Length-1; i++){
			Tile here = tiles[i];
			Tile next = tiles[i+1];

			bool thisResult = here.CanCombineWith(next) || here.CanMoveInto(next);
			result = result || thisResult;
		}


		return result;
	}

	bool SlidesAvailable(){
		bool result = false;

		foreach(Tile[] column in columns){
			result = result || TilesCanSlide(column);
		}
		foreach(Tile[] row in rows){
			result = result || TilesCanSlide(row);
		}

		return result;
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
        mouseLeftTile = false;
	}

	Tile mouseInTile;
	public void MouseInTile(Tile tile){
		mouseInTile = tile;
        if (mouseDownStartTile && !mouseInTile.SameTile(mouseDownStartTile)) {
            mouseLeftTile = true;
        }
	}

	public void MouseUp(){
        if (mouseInTile.SameTile(mouseDownStartTile)) {
            if (!mouseLeftTile) {
                mouseInTile.GetComponent<Tile>().rotateTile();
                BoardStateChanged();
            }
			return;
		} 

		ResetMergedFlags();

		if(mouseInTile.row == mouseDownStartTile.row){

			if(mouseInTile.col < mouseDownStartTile.col){
				bool moveMade = false;
				while (MakeOneMoveDownIndex(rows[mouseInTile.row])) {
					moveMade = true;
				}

				if(moveMade){
					int lastIndex = rows[mouseInTile.row].Length - 1;
					AddNextTile(rows[mouseInTile.row], lastIndex);
				}


			} else {
				bool moveMade = false;
				while (MakeOneMoveUpIndex(rows[mouseInTile.row])) {
					moveMade = true;
				}

				if(moveMade){
					AddNextTile(rows[mouseInTile.row], 0);

				}

			}
			BoardStateChanged();
			return;
		}

		if(mouseInTile.col == mouseDownStartTile.col){

			if(mouseInTile.row < mouseDownStartTile.row){
				bool moveMade = false;
				while (MakeOneMoveDownIndex(columns[mouseInTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					int lastIndex = columns[mouseInTile.col].Length - 1;
					AddNextTile(columns[mouseInTile.col], lastIndex);
				}

			} else {
				bool moveMade = false;
				while (MakeOneMoveUpIndex(columns[mouseInTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					AddNextTile(columns[mouseInTile.col], 0);
				}

			}
			BoardStateChanged();
			return;
		}

		print ("no slide");
	}

	void AddNextTile(Tile[] rowOrColumn, int index){
		rowOrColumn[index].setTileId(NextTile.tileId);
		NextTile.assignStartingTile();
	}

	void DrawPath(List<Path> paths, Material material){
		foreach(Path path in paths){
			List<Vector3> linePoints = new List<Vector3>();

			foreach(Node node in path.nodes){
				linePoints.Add(new Vector3((node.col-1) * spriteSize, (node.row-1) * spriteSize, -1));
			}

			VectorLine pathLine = new VectorLine("ScoreLine", linePoints, material, 5, LineType.Continuous, Joins.Weld);
			pathLine.Draw();
			scoreLines.Add(pathLine);
		}
	}

	void AfterRotate(){
		BoardStateChanged();
	}

	void AfterSlide(){
		lastRotatedTile = null;
		BoardStateChanged();
	}

	void BoardStateChanged(){
		pathFinder.LoadTileSet(AllTiles);
		UpdateScoreDisplay();

		VectorLine.Destroy(scoreLines);
		scoreLines.Clear();


		DrawPath(VerticalPaths, VerticalLineMaterial);
		DrawPath(HorizontalPaths, HorizontalLineMaterial);

		
		if(!SlidesAvailable ()){
			if(verticalScore > horizontalScore){
				VictoryText.text = "Vertical\nVictory";
			}else if(horizontalScore > verticalScore){
				VictoryText.text = "Horizontal\nVictory";
			} else {
				VictoryText.text = "No Victory";
			}
		}
	}

	void Update () {
	}
}
