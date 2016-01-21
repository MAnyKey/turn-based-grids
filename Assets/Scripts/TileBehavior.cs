using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class TileBehavior : MonoBehaviour, PathFinding.IHasNeighbours<TileBehavior> {
    
    public Color pathColor;
    public Color nonPassableColor;
    public Color occupiedColor;
    public Color canGoColor;

    private Tile tile_;
    public Tile Tile {
        get {
            return tile_;
        }
        set {
            if (tile_ != null) {
                tile_.tb_ = null;
            }
            tile_ = value;
            tile_.tb_ = this;
        }
    }

    [HideInInspector]
    public GridManager gridManager;

    private Collider collider_;
    private Renderer renderer_;
    private Color startColor_;

    private List<TileBehavior> allNeighbours_;

//    private bool canGoHere_;
//    public bool canGoHere {
//        get {
//            return canGoHere_;
//        }
//        set {
//            canGoHere_ = value;
//            UpdateColor();
//        }
//    }


    public void FindNeighbours(Func<Point, TileBehavior> boardFunc) {
        var neighbours = new List<TileBehavior>();
        foreach (var neighbourLocation in Tile.PossibleNeighbours()) {
            var neighbour = boardFunc(neighbourLocation);
            if (neighbour != null) {
                neighbours.Add(neighbour);
            }
        }
        allNeighbours_ = neighbours;
        Tile.AllNeighbours = neighbours.Select(tb => tb.Tile).ToList();
    }

    internal void TileStateUpdated() {
        UpdateColor();
    }

    public IEnumerable<TileBehavior> Neighbours() {
        return allNeighbours_.Where(tb => tb.Tile.IsFree);
    }

    public static double Distance(TileBehavior a, TileBehavior b) {
        return Tile.Distance(a.Tile, b.Tile);
    }


    void Awake() {
        collider_ = GetComponent<Collider>();
        renderer_ = GetComponent<Renderer>();
        startColor_ = renderer_.material.color;
    }

    void Update() {
        bool isMouseOver = IsMouseOver();
        if (isMouseOver && Input.GetMouseButtonDown(1)) {
            gridManager.TileClicked(this, 1);
        }

        if (!Tile.IsFree) {
            return;
        }

        if (!Tile.CanGoHere) {
            return;
        }

        if (isMouseOver && Input.GetMouseButtonDown(0)) {
            gridManager.TileClicked(this, 0);
        }
    }

    public void TogglePassable() {
        Tile.TogglePassable();
    }

    private void UpdateColor() {
        renderer_.material.color = NeededColor();
    }

    private Color NeededColor() {
        if (Tile.IsNotPassable) {
            return nonPassableColor;
        }
        if (Tile.IsOccupied) {
            return occupiedColor;
        }
        if (Tile.CanGoHere) {
            return canGoColor;
        }
        return startColor_;
    }

    private bool IsMouseOver() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        return collider_.Raycast(ray, out hit, 100);
    }
}
