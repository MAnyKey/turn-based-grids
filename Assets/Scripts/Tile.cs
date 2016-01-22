
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Tile : PathFinding.IHasNeighbours<Tile> {

    public Point Location { get; set; }

    private enum TileState {
        FREE,
        NOT_PASSABLE,
        OCCUPIED
    }


    private TileState tileState_;
    private bool canGoHere_;

    public TileBehavior tb_;


    public Tile(Point location) {
        Location = location;
        tileState_ = TileState.FREE;
    }

    public int X { get { return Location.X; } }
    public int Y { get { return Location.Y; } }
    public int Z { get { return -(X + Y); } }

    public bool IsFree { get { return tileState_ == TileState.FREE; } }
    public bool IsOccupied { get { return tileState_ == TileState.OCCUPIED; } }
    public bool IsNotPassable { get { return tileState_ == TileState.NOT_PASSABLE; } }
    public bool CanGoHere { 
        get { return canGoHere_; }
        set { 
            Debug.Assert(IsFree, "Tile must be free to assign CanGoHere");
            canGoHere_ = value;
            NotifyState();
        }
    }

    public void TogglePassable() {
        Debug.Assert(!IsOccupied, "Tile must not be occupied to TogglePassable()");
        switch (tileState_) {
            case TileState.FREE:
                tileState_ = TileState.NOT_PASSABLE;
                break;
            case TileState.NOT_PASSABLE:
                tileState_ = TileState.FREE;
                break;
        }
        NotifyState();
    }

    public void Occupy() {
        Debug.Assert(IsFree, "Tile must be free to Occupy()");
        tileState_ = TileState.OCCUPIED;
        NotifyState();
    }
    
    public void Free() {
        Debug.Assert(IsOccupied, "Tile must be occupied to Free()");
        tileState_ = TileState.FREE;
        NotifyState();
    }

    private static Point[] shifts = new Point[] {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1),
            new Point(-1, 1),
            new Point(1, -1)
        };

    public IEnumerable<Point> PossibleNeighbours() {
        return shifts.Select(shift => new Point(X + shift.X, Y + shift.Y));
    }

    // TODO: resolve Tile/TileBehavior duality
//    public void FindNeighbours(Func<Point, Tile> boardFunc) {
//        var neighbours = new List<Tile>();
//        foreach (var neighbourLocation in PossibleNeighbours()) { 
//            var tile = boardFunc(neighbourLocation);
//            if (tile != null) {
//                neighbours.Add(tile);
//            }
//        }
//        AllNeighbours = neighbours;
//    }

    public List<Tile> AllNeighbours { get; set; }

    public IEnumerable<Tile> Neighbours() {
        return AllNeighbours.Where(n => n.tileState_ == TileState.FREE);
    }

    public static double Distance(Tile start, Tile end) {
        int xDiff = Math.Abs(start.X - end.X);
        int yDiff = Math.Abs(start.Y - end.Y);
        int zDiff = Math.Abs(start.Z - end.Z);
        return Math.Max(xDiff, Math.Max(yDiff, zDiff));
    }

    public override string ToString() {
        return String.Format("Tile(X: {0}, Y: {1}, TileState:{2})", X, Y, tileState_);
    }

    private void NotifyState() {
        tb_.TileStateUpdated();
    }
}