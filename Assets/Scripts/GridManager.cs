using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

    public static GridManager Instance { get; private set; }

    public Dictionary<Point, TileBehavior> Board { get; set; }
    public CharacterMovement Character { get; set; }

    private bool disableUi_;

	// Use this for initialization
	void Start () {
	    if (Instance != null) {
            throw new System.Exception("Should be only one grid manager");
        }
        Instance = this;
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
        var startTile = Character.tile;
        var endTile = tile.tile;
        Func<Tile, Tile, double> distance = (x, y) => 1; // we expect it to be called only for adjacent tiles

        var path = PathFinding.FindPath(startTile, endTile, distance, Tile.Distance);
        if (path == null) {
            Debug.Log("Empty path");
            return;
        }
        
        
        var pathTiles = path.Reverse().Skip(1).ToList();
        disableUi_ = true;
        Action<CharacterMovement> action = (x) => { disableUi_ = false; };
        Character.MoveTo(pathTiles, action);
    }

    //private void FindAndShowPath() {
    //    var startTb = endPointTiles[0];
    //    var endTb = endPointTiles[1];

    //    Func<TileBehavior, TileBehavior, double> distance = (x, y) => 1; // we expect it to be called only for adjacent tiles

    //    var path = PathFinding.FindPath(startTb, endTb, distance, TileBehavior.Distance);
    //    if (path != null) {
    //        pathTiles = path.ToArray();
    //        UpdatePathTiles(true);
    //    }
    //}

    //public void ResetPath() {
    //    UpdatePathTiles(false);
    //}

    //private void UpdatePathTiles(bool isPathPart) {
    //    if (pathTiles == null) {
    //        return;
    //    }
    //    foreach (var tb in pathTiles) {
    //        tb.IsPathPart = isPathPart;
    //    }
    //}
}
