using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

    public static GridManager Instance { get; private set; }

    public Dictionary<Point, TileBehavior> Board { get; set; }

    public bool SelectingPath { get; private set; }

	// Use this for initialization
	void Start () {
	    if (Instance != null) {
            throw new System.Exception("Should be only one grid manager");
        }
        Instance = this;
	}

    List<TileBehavior> endPointTiles = new List<TileBehavior>();
    TileBehavior[] pathTiles;


    public void TileClicked(TileBehavior tile) {
        ResetPath();
        endPointTiles.Add(tile);
        if (endPointTiles.Count == 2) {
            FindAndShowPath();
            endPointTiles = new List<TileBehavior>();
        }
    }

    private void FindAndShowPath() {
        var startTb = endPointTiles[0];
        var endTb = endPointTiles[1];

        Func<TileBehavior, TileBehavior, double> distance = (x, y) => 1; // we expect it to be called only for adjacent tiles

        var path = PathFinding.FindPath(startTb, endTb, TileBehavior.Distance, TileBehavior.Distance);
        pathTiles = path.ToArray();
        UpdatePathTiles(true);
    }

    public void ResetPath() {
        UpdatePathTiles(false);
    }

    private void UpdatePathTiles(bool isPathPart) {
        if (pathTiles == null) {
            return;
        }
        foreach (var tb in pathTiles) {
            tb.IsPathPart = isPathPart;
        }
    }
}
