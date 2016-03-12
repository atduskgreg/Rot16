using UnityEngine;
using System.Collections;

public enum MoveDirection {
	Up, Down, Left, Right
};

public class Move {
	public Tile startingTile;
	public Tile currentTile;
	public Tile[] tileList;
	public MoveDirection moveDirection;
	public bool isClick;
	BoardManager boardManager;
	
	Vector3 startingMousePositionScreenSpace; // screen space
	
	public Move(BoardManager boardManager, Tile startingTile){
		this.boardManager = boardManager;
		this.startingTile = startingTile;
		this.currentTile = startingTile;
		this.startingMousePositionScreenSpace = Input.mousePosition;
		isClick = true;
	}
	
	public Vector3 GetMouseMoveScreenSpace(){
		return Input.mousePosition - startingMousePositionScreenSpace;
	}
	
	public Vector3 GetMouseMoveWorldSpace(){
		Vector3 move = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(startingMousePositionScreenSpace);
		move.z = 0;
		return move;
	}
	
	public void MoveToTile(Tile tile){
		currentTile = tile;
		if(!currentTile.SameTile(startingTile)){
			isClick = false;
		}
		ComputeMoveDirection();
	}

	public void Start(){
		foreach(Tile tile in Tileset()){
			tile.MakeTransparent();
		}
	}

	public void Stop(){
		foreach(Tile tile in Tileset()){
			tile.ResetToCanonicalPosition();
			tile.MakeOpaque();
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
	
	public void ComputeMoveDirection(){
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
