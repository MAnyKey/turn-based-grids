using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour {

    private Dictionary<Point, TileBehavior> board_;

    public Dictionary<Point, TileBehavior> Board {
        get { return board_; }
        set {
            board_ = value;
            CheckInBoard();
        }
    }

    private Character currentCharacter_ { get { return characterQueue_.GetCurrentCharacter(); } }

    private bool disableUi_;

    private CharacterQueue characterQueue_;

    void Awake() {
        characterQueue_ = GetComponentInChildren<CharacterQueue>();
    }


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
            ActivateFrom(currentCharacter_);
        }
    }

    private void MoveCharacterTo(TileBehavior tile) {
        var startTile = currentCharacter_.Tile;
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
        currentCharacter_.MoveTo(pathTiles, MoveEnded);
    }

    private void CheckInBoard() {
        Func<Point, TileBehavior> boardFunc = (point) => {
            TileBehavior tb;
            Board.TryGetValue(point, out tb);
            return tb;
        };
        foreach (KeyValuePair<Point, TileBehavior> pair in Board) {
            pair.Value.FindNeighbours(boardFunc);
        }
    }

    public void PlaceCharactersOnTheBoard(List<GameObject> startCharacters) {
        var characters = new List<Character>();
        for (int i = 0; i < startCharacters.Count; ++i) {
            var sample = startCharacters[i];
            var characterGameObject = Instantiate(sample);
            characterGameObject.transform.parent = characterQueue_.transform;
            var character = characterGameObject.GetComponent<Character>();
            Debug.Assert(character != null, "Character should not be null");
            characters.Add(character);
        }
        StartCoroutine(StartGameLoop(characters, characterQueue_.startingPoints));
    }

    private void MoveEnded(Character character) {
        disableUi_ = false;

        Character nextCharacter = NextCharacter(character);
        ActivateFrom(nextCharacter);
    }

    private Character NextCharacter(Character character) {
        characterQueue_.CharacterDidAction();
        return currentCharacter_;
    }


    private List<Tile> activeTiles;

    private void ActivateFrom(Character character) {
        var maxDistance = character.currentStats.speed;
        var reachable = PathFinding.ListReachable(character.Tile, maxDistance);
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

    private IEnumerator StartGameLoop(List<Character> characters, List<Point> startingPoints) {
        yield return null; // skip one frame to let Character component do Start();

        Debug.Assert(characters.Count <= startingPoints.Count);
        Func<Tile, Vector3> tilePosFunc = (tile) => Board[tile.Location].transform.position;
        for (int i = 0; i < characters.Count; ++i) {
            var point = startingPoints[i];
            var character = characters[i];

            character.Movement.TilePosFunc = tilePosFunc;
            character.TeleportTo(Board[point].Tile);
        }
            
        characterQueue_.PlaceCharacters(characters);
        ActivateFrom(currentCharacter_);
    }
}
