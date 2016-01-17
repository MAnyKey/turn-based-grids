using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

    public static GridManager Instance { get; private set; }

    //public GameObject Character { get; set; }

    //private void PlaceCharacter(GameObject characterSample, Point gridPoint) {
    //    var character = Instantiate(characterSample);
    //    controller_ = character.GetComponent<CharacterControllerAI>();
    //    var tb = Board[gridPoint];
    //    controller_.SetTile(tb);
    //}

    public Dictionary<Point, TileBehavior> Board { get; set; }
    public CharacterMovement Character { get; set; }

    //private struct CharacterDescription {
    //    public Tile tile;
    //    public CharacterMovement characterMovement;
    //}

    //private List<CharacterDescription> characters_;

    //public bool SelectingPath { get; private set; }

	// Use this for initialization
	void Start () {
	    if (Instance != null) {
            throw new System.Exception("Should be only one grid manager");
        }
        Instance = this;
	}

    void Update() {
        //if (Input.GetMouseButtonDown(0)) {
        //    ResetPath();
        //}
    }

    List<TileBehavior> endPointTiles = new List<TileBehavior>();
    TileBehavior[] pathTiles;


    public void TileClicked(TileBehavior tile) {
        if (Character.IsMoving) {
            // ignore click while moving
            // TODO: we should call this function from TileBehavior in order to ignore all clicks, not just this type
            Debug.Log("Is moving");
            return;
        }
        Debug.Log("Move to " + tile.tile.Location);
        MoveCharacterTo(tile);
        //ResetPath();
        //endPointTiles.Add(tile);
        //if (endPointTiles.Count == 2) {
        //    FindAndShowPath();
        //    endPointTiles = new List<TileBehavior>();
        //}
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
        
        Action<CharacterMovement> action = (x) => { }; // TODO: do something
        var pathTiles = path.Reverse().Skip(1).ToList();
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
