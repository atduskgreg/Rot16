using UnityEngine;
using System.Collections;

public enum MoveDirection {
	Up, Down, Left, Right, Click
};

public class Move {
	public Tile startingTile;
	public Tile currentTile;
	public Tile[] tileList;
	public MoveDirection moveDirection;
	public bool isClick;
	BoardManager boardManager;

	private float startTime;
	
	Vector3 startingMousePositionScreenSpace; // screen space
	
	public Move(BoardManager boardManager, Tile startingTile){
		this.boardManager = boardManager;
		this.startingTile = startingTile;
		this.currentTile = startingTile;
		this.moveDirection = MoveDirection.Click;
		this.startingMousePositionScreenSpace = Input.mousePosition;
//		isClick = true;
		startTime = Time.fixedTime;
	}
	
	public Vector3 GetMouseMoveScreenSpace(){
		return Input.mousePosition - startingMousePositionScreenSpace;
	}
	
	public Vector3 GetMouseMoveWorldSpace(){
		Vector3 move = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(startingMousePositionScreenSpace);
		move.z = 0;
		return move;
	}

	public float Duration(){
		return Time.fixedTime - startTime;
	}

	public bool HasLeftTile(){
		return !currentTile.SameTile(startingTile);
	}
	
	public void MoveToTile(Tile tile){
		currentTile = tile;
		ComputeMoveDirection();
	}

	public void Start(){
		Start(Tileset());
	}

	public void Stop(){
		Stop(Tileset ());
	}

	void Start(Tile[] tileset){
		foreach(Tile tile in tileset){
			tile.MakeTransparent();
		}
	}

	void Stop(Tile[] tileset){
		foreach(Tile tile in tileset){
			tile.ResetToCanonicalPosition();
			tile.MakeOpaque();
		}
	}

	// todo: move direction doesn't get computed until
	// we leave the tile so tile is sticky until you slide out of it
	// maybe this is good?
	public void MoveTiles(){
		Vector3 moveOffset = GetMouseMoveWorldSpace();
		if(moveDirection == MoveDirection.Left || moveDirection == MoveDirection.Right){
			moveOffset.y = 0;
		} else if(moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down){
			moveOffset.x = 0;
		}


		Tile[] tileset = (Tile[])Tileset().Clone();
		if(moveDirection == MoveDirection.Right || moveDirection == MoveDirection.Up){
			//Debug.Log("reversing");
			System.Array.Reverse(tileset);
		}

		//Debug.Log("start");
		int numTilesMoving = 0;
		bool alreadyCombined = false;
		bool previousTileStopped = false;
		Vector3 totalAmountMoved = new Vector3();
		for(int i = 1 ; i < tileset.Length; i++){
			Tile tile = tileset[i];


			bool shouldMove = false;
			if(tile.CanCombineWith(tileset[i-1]) && !alreadyCombined){
				numTilesMoving++;
				alreadyCombined = true;
				shouldMove = true;
			}

			if(tileset[i-1].isEmpty()){
				numTilesMoving++;
				shouldMove = true;
			}

				//Debug.Log("numTilesMoving: " + numTilesMoving);
				float maxMoveDist = numTilesMoving * boardManager.GetSpriteSize();
				float moveMag = moveOffset.magnitude;
//				Debug.Log("maxMoveDist: " + maxMoveDist + " moveMag: " + moveMag);
				float clampedMoveMag = Mathf.Clamp(moveMag, 0, maxMoveDist);
			//moveOffset.Normalize();
			//moveOffset = moveOffset * clampedMoveMag;
		
			tile.MoveTo(tile.canonicalPosition + moveOffset*numTilesMoving);
		

	
				
		}
	}
	
	Tile[] Tileset(){
		if(moveDirection == MoveDirection.Left || moveDirection == MoveDirection.Right){
			return boardManager.rows[startingTile.row];
		} else if(moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down){
			return boardManager.columns[startingTile.col];
		} else {
			return new Tile[]{startingTile}; 
		}
	}

	void MoveDirectionChanged(Tile[] prevSet){
		Stop(prevSet);
		Start(Tileset());
	}
	
	public void ComputeMoveDirection(){
		if (HasLeftTile()) {

			MoveDirection prev = moveDirection;
			Tile[] prevSet = Tileset();

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
			if(prev != moveDirection){
				MoveDirectionChanged(prevSet);
			}
		}
	//	Debug.Log("movedir: " + moveDirection);

	}
	
}
