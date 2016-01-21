using UnityEngine;
using System.Collections.Generic;
using System;

// [RequireComponent(typeof(Animation))]
public class CharacterMovement : MonoBehaviour {

    public int maxMoveDistance = 5;

    public float speed = 2.5f;
    public float rotationSpeed = 0.04f;

    public float minNextTileDest = 0.07f;

    public Func<Tile, Vector3> TilePosFunc { get; set; }

    private Animation animation_;
    private Transform transform_;

    public bool IsMoving { get; private set; }

    public Tile tile;
    
    private struct Moving {
        public int curTileIdx;
        public Vector3 curTilePos;
        public List<Tile> path;
        public Action<CharacterMovement> endMove;

        public Moving(int curTileIdx, Vector3 curTilePos, List<Tile> path, Action<CharacterMovement> endMove) {
            this.curTilePos = curTilePos;
            this.curTileIdx = curTileIdx;
            this.path = path;
            this.endMove = endMove;
        }
    } 

    private Moving move_;

    // we assume that our current position is included in the path
    public void MoveTo(List<Tile> path, Action<CharacterMovement> endMove) {
        if (path.Count <= 1) {
            return;
        }
        var curTileIdx = 1;
        var curTilePos = CalcTilePos(path[curTileIdx]);
        move_ = new Moving(curTileIdx, curTilePos, path, endMove);
        IsMoving = true;
    }

    public void TeleportTo(Tile tile) {
        if (IsMoving) {
            return;
        }
        this.tile = tile;
        tile.Occupy();
        transform_.position = CalcTilePos(tile);
    }

    private Vector3 CalcTilePos(Tile tile) {
        var tilePos = TilePosFunc(tile);
        tilePos.y = transform.position.y;
        return tilePos;
    }

    void Awake() {
        IsMoving = false;
        animation_ = GetComponent<Animation>();
        if (animation_) {
            animation_.wrapMode = WrapMode.Loop;    
        }
        transform_ = transform;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!IsMoving) {
            return;
        }
        if (CloseEnoughToCurrentTile()) {
            bool finish = SwitchToNextTile();
            if (finish) {
                FinishMoving();
                return;
            }
        }
        MoveTowards(move_.curTilePos);
	}

    private void FinishMoving() {
        var endMove = move_.endMove;
        tile = move_.path[move_.path.Count - 1];
        var previousTile = move_.path[0];

        move_ = new Moving();
        IsMoving = false;

        GoToAnimation("idle");

        previousTile.Free();
        tile.Occupy();

        endMove(this);
    }

    private void MoveTowards(Vector3 curTilePos) {
        // TODO: make smooth rotation
        var direction = move_.curTilePos - transform.position;
        //transform.rotation = Quaternion.Slerp(transform.rotation, 
        //    Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);

        var forward = transform.forward;
        forward *= speed;

        // just cosine of angle between |direction| and current object forward direction
        //float speedModifier = Vector3.Dot(direction.normalized, transform.forward);
        //const float speedModifierThreshold = 0.95f;
        //if (speedModifier > speedModifierThreshold) {
        //    // walk
        //    controller_.SimpleMove(forward * speedModifier);
        //    //if (!animation_["walk"].enabled) {
        //    //    animation_.CrossFade("walk");
        //    //}
        //} else {
        //    // idle, just rotating
        //    //if (!animation_["idle"].enabled) {
        //    //    animation_.CrossFade("idle");
        //    //}
        //}
        //controller_.SimpleMove(forward);
        GoToAnimation("run");
        transform.position += forward;
    }

    // returns true if we reached the last tile of the path
    private bool SwitchToNextTile() {
        move_.curTileIdx++;
        if (move_.curTileIdx == move_.path.Count) {
            return true;
        }
        move_.curTilePos = CalcTilePos(move_.path[move_.curTileIdx]);
        return false;
    }

    private bool CloseEnoughToCurrentTile() {
        var distanceVector = transform_.position - move_.curTilePos;
        return distanceVector.sqrMagnitude < minNextTileDest * minNextTileDest;
    }

    private void GoToAnimation(string animation) {
        if (!animation_) {
            return;
        }

        if (!animation_[animation].enabled) {
            animation_.CrossFade(animation);
        }
    }
}
