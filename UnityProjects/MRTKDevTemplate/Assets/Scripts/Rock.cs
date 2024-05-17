namespace MixedReality.Toolkit.Suits.Map
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    // using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.Networking;

    public record RockData
    {
        public float SiO2;
        public float TiO2;
        public float Al2O3;
        public float FeO;
        public float MnO;
        public float MgO;
        public float CaO;
        public float K2O;
        public float P2O3;
        public float other;
    }

    public record Rock
    {
        public string name;
        public int id;
        public RockData data;
    }
    public record RockInfo
    {
        public Rock eva1;
        public Rock eva2;
    }

    public record DeserializeRock
    {
        public RockInfo spec;
    }
}


