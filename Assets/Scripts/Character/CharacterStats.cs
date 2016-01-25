﻿using UnityEngine;
using System.Collections;
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

    [HideInInspector]
    public Stats currentStats;


    void Awake() {
        currentStats = startStats;
    }
}
