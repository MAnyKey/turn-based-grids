
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Tile : PathFinding.IHasNeighbours<Tile> {
    public Point Location { get; set; }
    public bool Passable { get; set; }
    public bool Occupied { get; set; }

    public int X { get { return Location.X; } }
    public int Y { get { return Location.Y; } }
    public int Z {
        get {
            return -(X + Y);
        }
    }

    public Tile(Point location) {
        Passable = true;
        Location = location;
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

    public void FindNeighbours(Func<Point, Tile> boardFunc) {
        var neighbours = new List<Tile>();
        foreach (var neighbourLocation in PossibleNeighbours()) { 
            var tile = boardFunc(neighbourLocation);
            if (tile != null) {
                neighbours.Add(tile);
            }
        }
        AllNeighbours = neighbours;
    }

    public List<Tile> AllNeighbours { get; set; }

    public IEnumerable<Tile> Neighbours() {
        return AllNeighbours.Where(n => n.Passable && !n.Occupied);
    }

    public static double Distance(Tile start, Tile end) {
        int xDiff = Math.Abs(start.X - end.X);
        int yDiff = Math.Abs(start.Y - end.Y);
        int zDiff = Math.Abs(start.Z - end.Z);
        return Math.Max(xDiff, Math.Max(yDiff, zDiff));
    }

    public override string ToString() {
        return String.Format("Tile(X: {0}, Y: {1}, Passable:{2})", X, Y, Passable);
    }
}