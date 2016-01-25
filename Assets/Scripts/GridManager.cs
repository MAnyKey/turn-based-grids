using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridManager : MonoBehaviour, CharacterQueue.IObserver {

    public Canvas uiCanvas;

    private Dictionary<Point, TileBehavior> board_;

    public Dictionary<Point, TileBehavior> Board {
        get { return board_; }
        set {
            board_ = value;
            CheckInBoard();
        }
    }

    private Character GetCurrentCharacter() { 
        return characterQueue_.GetCurrentCharacter();
    }

    private bool disableUi_;

    private bool canWait_ = true;

    private ActionState actionState_;

    public enum ActionState {
        MOVE,
        ATTACK
    }

    private CharacterQueue characterQueue_;

    void Awake() {
        characterQueue_ = GetComponentInChildren<CharacterQueue>();
        characterQueue_.AddObserver(this);
    }

    void Update() {
        bool wait = Input.GetKeyUp(KeyCode.W);
        if (wait) {
            if (!canWait_ || actionState_ == ActionState.ATTACK) {
                Debug.Log(String.Format("Cannot wait! {0}, {1}", canWait_, actionState_));
                return;
            }
            actionState_ = ActionState.MOVE;
            characterQueue_.CharacterIsWaiting();
            ActivateFrom(GetCurrentCharacter());
        }
        bool defend = Input.GetKeyUp(KeyCode.D);
        if (defend) {
            actionState_ = ActionState.MOVE;
            characterQueue_.CharacterDidAction();
            ActivateFrom(GetCurrentCharacter());
        }
    }


    public void EndOfTheTurn() {
        canWait_ = true;
    }

    public void WaitStage() {
        canWait_ = false;
    }

    public void TileClicked(TileBehavior tile, int button) {
        if (disableUi_) {
            // ignore click while moving
            return;
        }
        DeactivateTiles();
        if (button == 0 && actionState_ == ActionState.MOVE) {
            MoveCharacterTo(tile);
            disableUi_ = true;
        } else if (button == 1) {
            tile.TogglePassable();
        }
        if (!disableUi_) {
            ActivateFrom(GetCurrentCharacter());
        }
    }

    private void MoveCharacterTo(TileBehavior tile) {
        var startTile = GetCurrentCharacter().Tile;
        var endTile = tile.Tile;
        Debug.Log(String.Format("Path from {0} to {1}", startTile.Location, endTile.Location));

        Func<Tile, Tile, double> distance = (x, y) => 1; // we expect it to be called only for adjacent tiles

        var path = PathFinding.FindPath(startTile, endTile, distance, Tile.Distance);
        if (path == null) {
            Debug.Log("Empty path");
            return;
        }
        
        var pathTiles = path.Reverse().ToList();
        GetCurrentCharacter().MoveTo(pathTiles, MoveEnded);
    }

    private void MoveEnded(Character character) {
        disableUi_ = false;
        actionState_ = ActionState.ATTACK;

        // TODO: mark can attack characters
    }

    private void AttackEnded(Character character) {
        disableUi_ = false;

        actionState_ = ActionState.MOVE;
        characterQueue_.CharacterDidAction();
        ActivateFrom(GetCurrentCharacter());
    }

    private List<Tile> activeTiles;

    private void ActivateFrom(Character character) {
        if (activeTiles != null) {
            DeactivateTiles();
        }
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

            characterGameObject.GetComponent<CharacterUI>().targetCanvas = uiCanvas;

        }
        StartCoroutine(StartGameLoop(characters, characterQueue_.startingPoints));
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
        ActivateFrom(GetCurrentCharacter());
    }
}
