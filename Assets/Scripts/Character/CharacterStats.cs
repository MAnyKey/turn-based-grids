using UnityEngine;
using System.Collections.Generic;
using System;


public class CharacterStats : MonoBehaviour {

    [Serializable]
    public struct Stats {
        public int health;
        public int armor;
        public int initiative;
        public int speed;
    }

    [SerializeField]
    Stats startStats;

    private Stats currentStats_;
    public Stats currentStats { get { return currentStats_; } }

    public int Health { 
        get { return currentStats.health; }
        set {
            currentStats_.health = value;
            NotifyObservers();
        }
    }

    public int Armor {
        get { return currentStats.armor; }
        set {
            currentStats_.armor = value;
            NotifyObservers();
        }
    }

    public int Initiative {
        get { return currentStats.initiative; }
        set {
            currentStats_.armor = value;
            NotifyObservers();
        }
    }

    public int Speed {
        get { return currentStats.speed; }
        set {
            currentStats_.initiative = value;
            NotifyObservers();
        }
    }

    void Awake() {
        currentStats_ = startStats;
    }

    public interface IObserver {
        void StatsChanged(CharacterStats sender);
    }

    public void AddObserver(IObserver observer) {
        observers_.Add(observer);
    }

    public void RemoveObserver(IObserver observer) {
        observers_.Remove(observer);
    }

    private void NotifyObservers() {
        foreach (var observer in observers_) {
            observer.StatsChanged(this);
        }
    }

    private List<IObserver> observers_ = new List<IObserver>();
}
