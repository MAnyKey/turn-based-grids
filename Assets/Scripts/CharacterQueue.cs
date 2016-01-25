using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterQueue : MonoBehaviour {

    // TODO: move outside of this class
    public List<Point> startingPoints;

    public void PlaceCharacters(List<Character> characters) {
        allCharacters_ = new HashSet<Character>(characters);
        StartNewTurn();
    }

    public void StartNewTurn() {
        queue_ = new TurnQueue(this, allCharacters_.ToList());
        turn_++;
    }

    public Character GetCurrentCharacter() {
        return queue_.Character;
    }

    public void CharacterDidAction() {
        queue_.DoAction();
    }

    public void CharacterIsWaiting() {
        queue_.Wait();
    }

    // Character can move only on next turn
    public void AddCharacter(Character character) {
        allCharacters_.Add(character);
    }

    // Character dies
    public void RemoveCharacter(Character character) {
        allCharacters_.Remove(character);
        queue_.RemoveCharacter(character);
    }

    public interface IObserver {
        void EndOfTheTurn();
        void WaitStage();
    }

    public void AddObserver(IObserver observer) {
        observers_.Add(observer);
    }

    public void RemoveObserver(IObserver observer) {
        observers_.Remove(observer);
    }

    private void NotifyEndOfTheTurn() {
        StartNewTurn();
        foreach (var observer in observers_) {
            observer.EndOfTheTurn();   
        }
    }

    private void NotifyWaitStage() {
        foreach (var observer in observers_) {
            observer.WaitStage();
        }
    }

    // FIXME: changing initiative of character doesn't change it's order in the queue
    private class TurnQueue {
        public TurnQueue(CharacterQueue parent, List<Character> characters) {
            parent_ = parent;
            // sort characters by increasing of initiative, so current character is contained in the last element of list
            characters.Sort((ch1, ch2) => ch1.currentStats.initiative.CompareTo(ch2.currentStats.initiative));
            normalTurn_ = characters;
            waiting_ = new List<Character>();
        }

        public Character Character { 
            get { 
                if (normalTurn_.Count != 0) {
                    return Last(normalTurn_);
                }
                return Last(waiting_);
            }
        }


        public void Wait() {
            Debug.Assert(!IsEmpty());
            Debug.Assert(!IsWaitStage, "Cannot wait in wait stage");

            using (var notifier = new TurnNotifier(this)) {
                var character = RemoveLast(normalTurn_);
                waiting_.Add(character);    
            }
        }

        public void DoAction() {
            Debug.Assert(!IsEmpty());

            using (var notifier = new TurnNotifier(this)) {
                if (IsWaitStage) {
                    RemoveLast(waiting_);
                    return; 
                } 
                RemoveLast(normalTurn_);
            }
        }

        public void RemoveCharacter(Character character) {
            using (var notifier = new TurnNotifier(this)) {
                normalTurn_.Remove(character);
                waiting_.Remove(character);
            }
        }

        private class TurnNotifier : IDisposable {
            public TurnNotifier(TurnQueue queue) {
                queue_ = queue;
                wasWaitStage_ = queue.IsWaitStage;
            }

            public void Dispose() {
                if (queue_.IsEndOfTheTurn) {
                    queue_.parent_.NotifyEndOfTheTurn();
                }                
                if (!wasWaitStage_ && queue_.IsWaitStage) {
                    queue_.parent_.NotifyWaitStage();
                }
            }

            private readonly TurnQueue queue_;
            private readonly bool wasWaitStage_;
        }

        private bool IsWaitStage { get { return normalTurn_.Count == 0; } }

        private bool IsEndOfTheTurn { get { return IsEmpty(); } }

        private bool IsEmpty() {
            return normalTurn_.Count == 0 && waiting_.Count == 0;
        }

        private List<Character> normalTurn_;
        private List<Character> waiting_;
        private CharacterQueue parent_;

        private static T Last<T>(List<T> list) {
            Debug.Assert(list.Count != 0);
            return list[list.Count - 1];
        }

        private static T RemoveLast<T>(List<T> list) {
            var element = Last(list);
            list.RemoveAt(list.Count - 1);
            return element;
        }
    }

    private HashSet<Character> allCharacters_ = new HashSet<Character>();
    private TurnQueue queue_ = null;
    private int turn_ = 0;

    private List<IObserver> observers_ = new List<IObserver>();
}
