using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

    //public static GridManager Instance { get; private set; }

    private Dictionary<Point, TileBehavior> board_;
    public Dictionary<Point, TileBehavior> Board {
        get {
            return board_;
        }
        set {
            board_ = value;
            CheckInBoard();
        }
    }

    private GameObject characterSample_;
    public GameObject CharacterSample {
        get {
            return characterSample_;
        }
        set {
            characterSample_ = value;
            CheckInCharacter();
        }
    }


    private CharacterMovement character_;


    private bool disableUi_;

	// Use this for initialization
	void Start () {
	    //if (Instance != null) {
     //       throw new System.Exception("Should be only one grid manager");
     //   }
     //   Instance = this;
	}


    public void TileClicked(TileBehavior tile, int button) {
        if (disableUi_) {
            // ignore click while moving
            return;
        }
        if (button == 0) {
            MoveCharacterTo(tile);
        } else if (button == 1) {
            tile.TogglePassable();
        }
    }

    private void MoveCharacterTo(TileBehavior tile) {
        var startTile = character_.tile;
        var endTile = tile.Tile;
        Debug.Log(String.Format("Path from {0} to {1}", startTile.Location, endTile.Location));

        Func<Tile, Tile, double> distance = (x, y) => 1; // we expect it to be called only for adjacent tiles

        var path = PathFinding.FindPath(startTile, endTile, distance, Tile.Distance);
        if (path == null) {
            Debug.Log("Empty path");
            return;
        }
        
        
        var pathTiles = path.Reverse().ToList();
        disableUi_ = true;
        Action<CharacterMovement> action = (x) => { disableUi_ = false; };
        character_.MoveTo(pathTiles, action);
    }

    private void CheckInBoard() {
        Func<Point, TileBehavior> boardFunc = (point) => {
            TileBehavior tb;
            Board.TryGetValue(point, out tb);
            return tb;
        };
        foreach (KeyValuePair<Point, TileBehavior> pair in Board) {
            pair.Value.FindNeighbours(boardFunc);
            pair.Value.gridManager = this;
        }
    }

    private void CheckInCharacter() {
        var chr = Instantiate(CharacterSample);
        var charMovement = chr.GetComponent<CharacterMovement>();
        charMovement.TilePosFunc = (tile) => Board[tile.Location].transform.position;
        charMovement.TeleportTo(Board[new Point(0, 0)].Tile);
        character_ = charMovement;
    }
}
