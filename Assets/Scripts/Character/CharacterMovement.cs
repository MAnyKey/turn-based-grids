using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// [RequireComponent(typeof(Animation))]
public class CharacterMovement : MonoBehaviour {
    
    public float speed = 2.5f;
    //    public float rotationSpeed = 0.04f;

    public Func<Tile, Vector3> TilePosFunc { get; set; }

    private Animation animation_;
    private Transform transform_;

    public bool IsMoving { get; private set; }

    private string animationState;

    void Awake() {
        IsMoving = false;
        animation_ = GetComponent<Animation>();
        if (animation_) {
            animation_.wrapMode = WrapMode.Loop;
        }
        transform_ = transform;
    }

    // we assume that our current position is included in the path
    public void MoveTo(List<Tile> path, Action<CharacterMovement> endMove) {
        if (path.Count <= 1) {
            endMove(this);
            return;
        }
        
        IsMoving = true;
        StartCoroutine(MovingCoroutine(path, endMove));
    }

    public void TeleportTo(Tile tile) {
        Debug.Assert(!IsMoving, "You should teleport stationary characters!");
        transform_.position = CalcTilePos(tile);
    }

    private Vector3 CalcTilePos(Tile tile) {
        var tilePos = TilePosFunc(tile);
        tilePos.y = transform.position.y;
        return tilePos;
    }

    private void GoToAnimation(string animation) {
        if (!animation_) {
            return;
        }

        if (animation != animationState) {
            animation_.CrossFade(animation);
            animationState = animation;
        }
    }

    IEnumerator MovingCoroutine(List<Tile> path, Action<CharacterMovement> endMove) {
        for (int curTileIdx = 1; curTileIdx < path.Count; ++curTileIdx) {
            var curTilePos = CalcTilePos(path[curTileIdx]);
            yield return MoveTowardsCor(curTilePos);
        }
        FinishMoving(path[0], path[path.Count - 1], endMove);
    }

    private IEnumerator MoveTowardsCor(Vector3 to) {
        var direction = to - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        GoToAnimation("run");

        while (transform.position != to) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, to, step);
            yield return null;
        }
    }

    private void FinishMoving(Tile startPoint, Tile endPoint, Action<CharacterMovement> endMove) {
        IsMoving = false;
        GoToAnimation("idle");
        endMove(this);
    }
}
