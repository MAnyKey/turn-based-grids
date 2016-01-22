using UnityEngine;
using System.Collections;
using System;

public struct Point {
    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString() {
        return String.Format("Point({0},{1})", X, Y);
    }
}
