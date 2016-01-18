using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class TileBehavior : MonoBehaviour, PathFinding.IHasNeighbours<TileBehavior> {
    //public Color selectionColor;
    public Color pathColor;
    public Color nonPassableColor;

    [HideInInspector]
    public Tile tile;

    private Collider collider_;
    private Renderer renderer_;
    private Color startColor_;

    private List<TileBehavior> allNeighbours_;
    private bool isPathPart_;


    public void FindNeighbours(Func<Point, TileBehavior> boardFunc) {
        var neighbours = new List<TileBehavior>();
        foreach (var neighbourLocation in tile.PossibleNeighbours()) {
            var neighbour = boardFunc(neighbourLocation);
            if (neighbour != null) {
                neighbours.Add(neighbour);
            }
        }
        allNeighbours_ = neighbours;
        tile.AllNeighbours = neighbours.Select(tb => tb.tile).ToList();
    }

    public IEnumerable<TileBehavior> Neighbours() {
        return allNeighbours_.Where(tb => tb.tile.Passable);
    }

    public static double Distance(TileBehavior a, TileBehavior b) {
        return Tile.Distance(a.tile, b.tile);
    }

    public bool IsPathPart {
        set {
            isPathPart_ = value;
            UpdateColor();
        }
    }


    void Start() {
        collider_ = GetComponent<Collider>();
        renderer_ = GetComponent<Renderer>();
        startColor_ = renderer_.material.color;
    }

    void Update() {
        bool isMouseOver = IsMouseOver();
        
        if (isMouseOver && Input.GetMouseButtonDown(1)) {
            GridManager.Instance.TileClicked(this, 1);
        }

        if (!tile.Passable) {
            return;
        }

        if (isMouseOver && Input.GetMouseButtonDown(0)) {
            GridManager.Instance.TileClicked(this, 0);
        }
    }

    public void TogglePassable() {
        tile.Passable = !tile.Passable;
        UpdateColor();
    }

    private void UpdateColor() {
        renderer_.material.color = NeededColor();
    }

    private Color NeededColor() {
        if (isPathPart_) {
            return pathColor;
        }
        if (!tile.Passable) {
            return nonPassableColor;
        }
        return startColor_;
    }

    private bool IsMouseOver() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        return collider_.Raycast(ray, out hit, 100);
    }
}
