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

    private List<GameObject> charactersSample_;

    public List<GameObject> StartCharacters {
        get {
            return charactersSample_;
        }
        set {
            charactersSample_ = value;
            CheckInCharacter();
        }
    }


    private class Characters {
        public Characters(List<Character> characters) {
            characters_ = characters;
            currentCharacter_ = 0;
        }

        public Character Character { 
            get {
                return characters_[currentCharacter_];
            }
        }

        public void MoveToNextCharacter() {
            currentCharacter_ = (currentCharacter_ + 1) % characters_.Count;
        }

        private List<Character> characters_;
        private int currentCharacter_;
    }

    private Characters characters_;

    private Character character_ { get { return characters_.Character; } }

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
        }
    }

    private void CheckInCharacter() {
        var charQueue = GetComponentInChildren<CharacterQueue>();

        var characters = new List<Character>();
        for (int i = 0; i < StartCharacters.Count; ++i) {
            var sample = StartCharacters[i];
            var characterGameObject = Instantiate(sample);
            characterGameObject.transform.parent = charQueue.transform;
            var character = characterGameObject.GetComponent<Character>();
            Debug.Assert(character != null, "Character should not be null");
            characters.Add(character);
        }
        StartCoroutine(StartGameLoop(characters, charQueue.startingPoints));
    }

    private void TurnEnded(Character character) {
        disableUi_ = false;

        Character nextCharacter = NextCharacter(character);
        ActivateFrom(nextCharacter);
    }

    private Character NextCharacter(Character character) {
        characters_.MoveToNextCharacter();
        return characters_.Character;
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

        characters_ = new Characters(characters);
        ActivateFrom(character_);
    }

    //    IEnumerator GameLoop(List<Character> characters) {
    //        while (true) {
    //            foreach (var character in characters) {
    //                yield return WaitForClick();
    //            }
    ////            yield return WaitForClick();
    ////            yield return MoveCharacter();
    ////            yield return NextCharacter();
    //        }
    //    }
    //
    //    private IEnumerator WaitForClick() {
    //        DeactivateTiles();
    //        while (!clicked_) {
    //            yield return new Wait
    //        }
    //    }
    //
    //    private IEnumerator MoveCharacter() {
    //        throw new NotImplementedException();
    //    }

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
