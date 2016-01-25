using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterUI))]
public class Character : MonoBehaviour {

    private CharacterStats stats_;

    public CharacterMovement Movement { get; private set; }

    public CharacterStats.Stats currentStats { get { return stats_.currentStats; } }

    public Tile Tile { get; private set; }

    void Awake() {
        stats_ = GetComponent<CharacterStats>();
        Movement = GetComponent<CharacterMovement>();
    }

    public void TeleportTo(Tile tile) {
        Tile = tile;
        tile.Occupy();
        Movement.TeleportTo(tile);
    }

    public void MoveTo(List<Tile> path, Action<Character> endMoveCallback) {
        Movement.MoveTo(path, (x) => FinishMoving(path, endMoveCallback));
    }

    private void FinishMoving(List<Tile> path, Action<Character> endMoveCallback) {
        Tile startPoint = path[0];
        Tile endPoint = path[path.Count - 1];

        Tile = endPoint;

        startPoint.Free();
        endPoint.Occupy();
        endMoveCallback(this);
    }
}
