using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

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


    public void TileClicked(TileBehavior tile, int button) {
        if (disableUi_) {
            // ignore click while moving
            return;
        }
        DeactivateTiles();
        if (button == 0) {
            MoveCharacterTo(tile);
        } else if (button == 1) {
            tile.TogglePassable();
        }
        if (!disableUi_) {
            ActivateFrom(character_);
        }
    }

    private void MoveCharacterTo(TileBehavior tile) {
        var startTile = character_.Tile;
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
        character_.MoveTo(pathTiles, TurnEnded);
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
        ActivateFrom(character_);
    }

    private void TurnEnded(CharacterMovement character) {
        disableUi_ = false;

        CharacterMovement nextCharacter = NextCharacter(character);
        ActivateFrom(nextCharacter);
    }

    private CharacterMovement NextCharacter(CharacterMovement character) {
        return character;
    }


    private List<Tile> activeTiles;

    private void ActivateFrom(CharacterMovement character) {
        var reachable = PathFinding.ListReachable(character.Tile, character.maxMoveDistance);
        foreach (var tb in reachable) {
            tb.CanGoHere = true;
        }
        activeTiles = reachable;
    }


    private void DeactivateTiles() {
        foreach (var tb in activeTiles) {
            tb.CanGoHere = false;
        }
        activeTiles = null;
    }


    IEnumerator GameLoop() {
        while (true) {
            yield return WaitForClick();
            yield return MoveCharacter();
            yield return NextCharacter();
        }
    }

    private IEnumerator WaitForClick() {
        yield return null;
    }

    private IEnumerator MoveCharacter() {
        throw new NotImplementedException();
    }

    private IEnumerator NextCharacter() {
        throw new NotImplementedException();
    }


    //IEnumerator TestCor() {
    //    for (int i = 0; i < 3; ++i) {
    //        yield return TestSub(i);
    //    }
    //}

    //IEnumerator TestSub(int i) {
    //    for (int j = 0; j < 3; ++j) {
    //        Debug.Log("TestSub " + i + " " + j);
    //        yield return null;
    //    }
    //}

    //void Start() {
    //    StartCoroutine(TestCor());
    //}
}
