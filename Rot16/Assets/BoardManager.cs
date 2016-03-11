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

	public int pathHighlightWeight = 2;

	public int verticalPathOffset = 2;
	public int horizontalPathOffset = -2;

	List<Path> VerticalPaths = new List<Path>();
	List<Path> HorizontalPaths = new List<Path>(); 
	List<VectorLine> scoreLines = new List<VectorLine>();

	public GameObject tilePrefab;
	PathFinder pathFinder;

	public Tile[,] AllTiles = new Tile[4,4];
	public List <Tile[]> columns = new List<Tile[]> ();
	public List <Tile[]> rows = new List<Tile[]> ();

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
		bool canSlide = false;
		for(int i = 0; i < tiles.Length-1; i++){
			Tile here = tiles[i];
			Tile next = tiles[i+1];
            bool thisResult = here.CanCombineWith(next) || here.CanMoveInto(next);
            canSlide = canSlide || thisResult;
        }
		return canSlide;
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

	bool[] CheckTileCombinationsDownIndex(Tile[] LineOfTiles){
		bool[] canCombine = new bool[LineOfTiles.Length-1];

		for (int i = 0; i < LineOfTiles.Length-1; i++) {
			//MOVE BLOCK 
			if (LineOfTiles[i].isEmpty()  && !LineOfTiles[i+1].isEmpty()){
				canCombine[i] = true;
			}

			// if the one to our left merged, we can't merge with it
			else if(i > 0 && canCombine[i-1] == true){
				canCombine[i] = false;	
			}

			// MERGE BLOCK
			else if (!LineOfTiles[i].isEmpty() && LineOfTiles[i].CanCombineWith(LineOfTiles[i+1])){
				canCombine[i] = true;	
			}
		}
		return canCombine;
	}

	bool[] CheckTileCombinationsUpIndex(Tile[] LineOfTiles){
		bool[] canCombine = new bool[LineOfTiles.Length-1];
		int resultIndex = 0;

		for (int i = LineOfTiles.Length-1; i > 0; i--) {
			//MOVE BLOCK 
			if (LineOfTiles[i].isEmpty()  && !LineOfTiles[i-1].isEmpty()){
				canCombine[resultIndex] = true;
				resultIndex++;
			}
			
			// if the one to our left merged, we can't merge with it
			else if(resultIndex > 0 && canCombine[resultIndex-1] == true){
				canCombine[resultIndex] = false;
				resultIndex++;
			}
			
			// MERGE BLOCK
			else if (!LineOfTiles[i].isEmpty() && LineOfTiles[i].CanCombineWith(LineOfTiles[i-1])){
				canCombine[resultIndex] = true;
				resultIndex++;
			}
		}
		return canCombine;
	}

	bool MakeOneMoveDownIndex(Tile[] LineOfTiles){
        for (int i = 0; i < LineOfTiles.Length - 1; i++) {
            //MOVE BLOCK 
            // move into empty spaces.
            if (LineOfTiles[i].isEmpty() && !LineOfTiles[i + 1].isEmpty()) {
                LineOfTiles[i].setTileId(LineOfTiles[i + 1].tileId);
                LineOfTiles[i + 1].setTileId(Tile.EmptyTileId);
                return true;
            // MERGE BLOCK
            } else if (!LineOfTiles[i].isEmpty() && LineOfTiles[i].CanCombineWith(LineOfTiles[i+1]) &&
			    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i+1].mergedThisTurn == false){

				LineOfTiles[i].CombineWith(LineOfTiles[i+1]);
				LineOfTiles[i+1].setTileId(Tile.EmptyTileId);
				LineOfTiles[i].mergedThisTurn = true;
				return true;
			}
		}
		return false;
	}

	bool MakeOneMoveUpIndex(Tile[] LineOfTiles) {
        for (int i = LineOfTiles.Length - 1; i > 0; i--) {
            // MOVE BLOCK 
            if (LineOfTiles[i].isEmpty() && !LineOfTiles[i - 1].isEmpty()) {
                LineOfTiles[i].setTileId(LineOfTiles[i - 1].tileId);
                LineOfTiles[i - 1].setTileId(Tile.EmptyTileId);
                return true;
            // MERGE BLOCK
            } else if (
                !LineOfTiles[i].isEmpty() &&
                LineOfTiles[i].mergedThisTurn == false &&
                LineOfTiles[i - 1].mergedThisTurn == false &&
                LineOfTiles[i].CanCombineWith(LineOfTiles[i-1])
            ) {
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

	Move currentMove;
	public void MouseDownInTile(Tile tile){
		currentMove = new Move(tile);
		currentMove.startingTile.DisableCollider();
		tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 10);
	}

	public void MouseInTile(Tile tile){
		if(currentMove != null){
			currentMove.MoveToTile(tile);
			currentMove.ComputeMoveDirection(this);
		}
	}

	Tile lastRotatedTile;

	public void MouseUp(){

        if (currentMove.isClick) {
			if(!lastRotatedTile || !currentMove.currentTile.SameTile(lastRotatedTile)){
				currentMove.currentTile.GetComponent<Tile>().rotateTile();
                AfterRotate();
			}
		} else {

			ResetMergedFlags();
			
        	bool moveMade = false;
			
			if(currentMove.moveDirection == MoveDirection.Left ){
				while (MakeOneMoveDownIndex(rows[currentMove.currentTile.row])) {
					moveMade = true;
				}
				if(moveMade){
					int lastIndex = rows[currentMove.currentTile.row].Length - 1;
					AddNextTile(rows[currentMove.currentTile.row], lastIndex);
				}
			} else if(currentMove.moveDirection == MoveDirection.Right) {
				while (MakeOneMoveUpIndex(rows[currentMove.currentTile.row])) {
					moveMade = true;
				}
				if(moveMade){
					AddNextTile(rows[currentMove.currentTile.row], 0);
				}
			} else if(currentMove.moveDirection == MoveDirection.Down){
				while (MakeOneMoveDownIndex(columns[currentMove.currentTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					int lastIndex = columns[currentMove.currentTile.col].Length - 1;
					AddNextTile(columns[currentMove.currentTile.col], lastIndex);
				}
			} else if(currentMove.moveDirection == MoveDirection.Up) {
				while (MakeOneMoveUpIndex(columns[currentMove.currentTile.col])) {
					moveMade = true;
				}
				if(moveMade){
					AddNextTile(columns[currentMove.currentTile.col], 0);
				}
			}

			if(moveMade){
				AfterSlide();
			}
		}

		ResetDraggedTiles();
	}

	void AddNextTile(Tile[] rowOrColumn, int index){
		rowOrColumn[index].setTileId(NextTile.tileId);
		NextTile.assignStartingTile();
	}

	void DrawPath(List<Path> paths, Material material, int pathOffset){
		foreach(Path path in paths){
			List<Vector3> linePoints = new List<Vector3>();

//			for(int i = 0; i < path.nodes.Count-1; i+=2){
//				Vector3 prev = new Vector3((path.nodes[i].col-1) * spriteSize, (path.nodes[i].row-1) * spriteSize, -1);
//				Vector3 next = new Vector3((path.nodes[i+1].col-1) * spriteSize, (path.nodes[i+1].row-1) * spriteSize, -1);
//
//				if(prev.y == next.y){
//					prev = new Vector3(prev.x + pathOffset, prev.y+pathOffset, prev.z);
//					next = new Vector3(next.x + pathOffset, next.y+pathOffset, next.z);
//				}
//
//				linePoints.Add(prev);
//				linePoints.Add(next);
//
//			}

			foreach(Node node in path.nodes){
				linePoints.Add(new Vector3((node.col-1) * spriteSize + pathOffset, (node.row-1) * spriteSize - pathOffset, -1));
			}

			VectorLine pathLine = new VectorLine("ScoreLine", linePoints, material, pathHighlightWeight, LineType.Continuous, Joins.Weld);
			pathLine.Draw();
			scoreLines.Add(pathLine);
		}
	}
	
	void AfterRotate(){
		lastRotatedTile = currentMove.currentTile;
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


		DrawPath(VerticalPaths, VerticalLineMaterial, verticalPathOffset);
		DrawPath(HorizontalPaths, HorizontalLineMaterial, horizontalPathOffset);

		
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

	void ResetDraggedTiles(){
		currentMove.startingTile.EnableCollider();
		currentMove.startingTile.ResetToCanonicalPosition();
		currentMove = null;

	}

	void ReEnableBoxCollider() { 
		currentMove.startingTile.GetComponent<BoxCollider2D>().enabled = true;
	}

	void Update () {
		if(currentMove != null){
			Vector3 newPos = currentMove.startingTile.canonicalPosition + currentMove.GetMouseMoveWorldSpace();
			currentMove.startingTile.transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
		}


		if(Input.GetKeyDown(KeyCode.Space)){
			if(currentMove.currentTile){
				bool[] rowCombinations = CheckTileCombinationsUpIndex(rows[currentMove.currentTile.row]);
				bool[] colCombinations = CheckTileCombinationsUpIndex(columns[currentMove.currentTile.col]);

				print ("rowCombinations: ");
				for(int i = 0; i < rowCombinations.Length; i++){
					print ("["+i+"] " + rowCombinations[i]);
				}

				print ("colCombinations: ");
				for(int i = 0; i < colCombinations.Length; i++){
					print ("["+i+"] " + colCombinations[i]);
				}

			}
		}
	}
}

public enum MoveDirection {
	Up, Down, Left, Right
};

public class Move {
	public Tile startingTile;
	public Tile currentTile;
	public Tile[] tileList;
	public MoveDirection moveDirection;
	public bool isClick;

	Vector3 startingMousePositionScreenSpace; // screen space

	public Move(Tile startingTile){
		this.startingTile = startingTile;
		this.currentTile = startingTile;
		this.startingMousePositionScreenSpace = Input.mousePosition;
		isClick = true;
	}

	public Vector3 GetMouseMoveScreenSpace(){
		return Input.mousePosition - startingMousePositionScreenSpace;
	}

	public Vector3 GetMouseMoveWorldSpace(){
		return Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(startingMousePositionScreenSpace);
	}

	public void MoveToTile(Tile tile){
		currentTile = tile;
		if(!currentTile.SameTile(startingTile)){
			isClick = false;
		}
	}

	public void ComputeMoveDirection(BoardManager boardManager){
		if (!startingTile.SameTile(currentTile)) {

			// figure out if we're dragging the column or the row
			if(startingTile.col == currentTile.col){
				tileList = boardManager.columns[currentTile.col];
				
				// determine direction of move within col
				if(currentTile.row > startingTile.row){
					moveDirection = MoveDirection.Up;
				} else {
					moveDirection = MoveDirection.Down;
				}

			}

			if(startingTile.row == currentTile.row){
				tileList = boardManager.rows[currentTile.row];

				// determine direction of move within row
				if(currentTile.col > startingTile.col){
					moveDirection = MoveDirection.Right;
				} else {
					moveDirection = MoveDirection.Left;
				}
			
			}
		}
	}
	
}
