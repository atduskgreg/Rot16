﻿using UnityEngine;
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

	public void MoveTiles(){
		Vector3 moveOffset = GetMouseMoveWorldSpace();
		if(moveDirection == MoveDirection.Left || moveDirection == MoveDirection.Right){
			moveOffset.y = 0;
		} else if(moveDirection == MoveDirection.Up || moveDirection == MoveDirection.Down){
			moveOffset.x = 0;
		}


		Tile[] tileset = (Tile[])Tileset().Clone();
		int numTilesMoving = 0;
		if(moveDirection == MoveDirection.Right || moveDirection == MoveDirection.Up){
			System.Array.Reverse(tileset);
		}

		for(int i = 1 ; i < tileset.Length; i++){
			Tile tile = tileset[i];

			bool shouldMove = true;

			if(tile.CanCombineWith(tileset[i-1]) || tileset[i-1].isEmpty()){
				shouldMove =  true;
			} else {
				shouldMove = false;
			}

			if(shouldMove){
				numTilesMoving++;
			}

			if(shouldMove){
				tile.MoveTo(tile.canonicalPosition + moveOffset*numTilesMoving);
			}
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
		if (!startingTile.SameTile(currentTile)) {

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
	}
	
}
