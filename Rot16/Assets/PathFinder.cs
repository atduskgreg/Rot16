using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PathFinder : MonoBehaviour {
	private Node[,] nodes = new Node[9,9];
	private Dictionary<Node, List<Node>> edges = new Dictionary<Node,List<Node>>();

	// Get the list of nodes connected by any given tileId
	// The coordinates are in local tile space
	// All of these are assumed to be two-directional
	public static List<Vector2[]> ConnectedNodesForTileId(int tileId){
		List<Vector2[]> result = new List<Vector2[]>();
		switch(tileId){
		case 0:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,1)});
			result.Add (new[]{new Vector2(1,1), new Vector2(2,1)});
			break;
		case 1:
			result.Add (new[]{new Vector2(1,0), new Vector2(1,1)});
			result.Add (new[]{new Vector2(1,1), new Vector2(1,2)});
			break;
		case 2:
			result.Add (new[]{new Vector2(1,0), new Vector2(1,1)});
			result.Add (new[]{new Vector2(1,2), new Vector2(1,1)});
			result.Add (new[]{new Vector2(0,1), new Vector2(1,1)});
			result.Add (new[]{new Vector2(2,1), new Vector2(1,1)});
			break;
		case 3:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,2)});
			result.Add (new[]{new Vector2(1,2), new Vector2(2,1)});
			result.Add (new[]{new Vector2(2,1), new Vector2(1,0)});
			result.Add (new[]{new Vector2(1,0), new Vector2(0,1)});
			break;
		case 4:
			result.Add (new[]{new Vector2(1,2), new Vector2(2,1)});
			break;
		case 5:
			result.Add (new[]{new Vector2(0,1), new Vector2(2,1)});
			break;
		case 6:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,0)});
			break;
		case 7:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,2)});
			break;
		case 8:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,2)});
			result.Add (new[]{new Vector2(1,2), new Vector2(2,1)});
			break;
		case 9:
			result.Add (new[]{new Vector2(1,2), new Vector2(2,1)});
			result.Add (new[]{new Vector2(2,1), new Vector2(1,0)});
			break;
		case 10:
			result.Add (new[]{new Vector2(0,1), new Vector2(1,0)});
			result.Add (new[]{new Vector2(1,0), new Vector2(2,1)});
			break;
		case 11:
			result.Add (new[]{new Vector2(1,2), new Vector2(0,1)});
			result.Add (new[]{new Vector2(0,1), new Vector2(1,0)});
			break;
		}

		return result;
	}

	public Path GetPath(Vector2 start, Vector2 end){
		return GetPath(nodes[(int)start.y,(int)start.x], nodes[(int)end.y, (int)end.x]);
	}


	private Path GetPath(Node start, Node end){
		// breadth first search
		Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
		Queue<Node> frontier = new Queue<Node>();
		frontier.Enqueue(start);

		while(frontier.Count > 0){
			Node current = frontier.Dequeue();

			if(edges.ContainsKey(current)){
				foreach(Node neighbor in edges[current]){
					if(!cameFrom.ContainsKey(neighbor)){
						frontier.Enqueue(neighbor);
						cameFrom[neighbor] = current;
					}
				}
			}
		}


		Path result = new Path();


		Node step = end;
		result.nodes.Add(step);


		while(cameFrom.ContainsKey(step) && step != start){
			step = cameFrom[step];
			result.nodes.Add(step);

			if(step == start){
				result.Exists = true;
			}
		}

		result.nodes.Reverse();

		return result;
	}

	public void LoadTileSet(Tile[,] tileSet){

		// 3x3 grid within each tile with shared
		// edges between adjacent tiles
		int gridHeight = tileSet.GetLength(0)*2+1; 
		int gridWidth = tileSet.GetLength(1)*2+1;

		for(int row = 0; row < gridHeight; row++){
			for(int col = 0; col < gridWidth; col++){
				nodes[row,col] = new Node(row,col);
			}
		}

		UpdateEdges(tileSet);

	}

	void UpdateEdges(Tile[,] tileSet){
		for(int row = 0; row <  tileSet.GetLength(0); row++){
			for(int col = 0; col <  tileSet.GetLength(1); col++){
//				print (col+"x"+row +": " + tileSet[row,col].tileId);
				List<Vector2[]> nodeConnections = PathFinder.ConnectedNodesForTileId(tileSet[row,col].tileId);
				for(int i = 0; i < nodeConnections.Count; i++){
					int fromNodeCol = (int)nodeConnections[i][0].x + col *2;
					int fromNodeRow = (int)nodeConnections[i][0].y + row *2;
					
					int toNodeCol = (int)nodeConnections[i][1].x + col *2;
					int toNodeRow = (int)nodeConnections[i][1].y + row *2;
					AddEdgeForNodes(nodes[fromNodeRow, fromNodeCol], nodes[toNodeRow, toNodeCol]);
					AddEdgeForNodes(nodes[toNodeRow, toNodeCol], nodes[fromNodeRow, fromNodeCol]);
					
				}
			}
		}
	}

	void AddEdgeForNodes(Node node1, Node node2){
		if(!edges.ContainsKey(node1)){
			edges[node1] = new List<Node>();
		}
		edges[node1].Add(node2);
	}
}

public class Path {
	public bool Exists;
	public List<Node> nodes;

	public Path(){
		Exists = false;
		nodes = new List<Node>();
	}

}

public class Node {
	public int row;
	public int col;
	public Node(int _row, int _col){
		row = _row;
		col = _col;
	}
}
