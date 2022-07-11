using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using FirebaseWebGLBridge = FirebaseWebGL.Scripts.FirebaseBridge;
using Random = System.Random;

#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Firestore;
using Firebase.Extensions;
#endif

namespace Firebase
{
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreData]
#endif
    [Serializable]
    public class PlayerData
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("playerId")]
#endif
        public uint PlayerId { 
            get => playerId;
            set => playerId = value;
        }
        [SerializeField]
        private uint playerId;
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("totalAttempts")]
#endif
        public int TotalAttempts
        {
            get => totalAttempts; 
            set => totalAttempts = value;
        }
        [SerializeField]
        private int totalAttempts;
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("totalDeaths")]
#endif
        public int TotalDeaths
        {
            get => totalDeaths; 
            set => totalDeaths = value;
        }
        [SerializeField]
        private int totalDeaths;
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("totalWins")]
#endif 
        public int TotalWins
        {
            get => totalWins; 
            set => totalWins = value;
        }
        [SerializeField]
        private int totalWins;
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("enemiesKilled")]
#endif
        public int EnemiesKilled
        {
            get => enemiesKilled; 
            set => enemiesKilled = value;
        }
        [SerializeField]
        private int enemiesKilled;
#if !UNITY_WEBGL || UNITY_EDITOR
        [FirestoreProperty("tokensCollected")]
#endif
        public int TokensCollected
        {
            get => tokensCollected; 
            set => tokensCollected = value;
        }
        [SerializeField]
        private int tokensCollected;
        public PlayerData()
        {
            var random = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            PlayerId = (uint) random.Next();
        }

        public void IncrementKills()
        {
            EnemiesKilled++;
        }

        public void IncrementDeaths()
        {
            TotalDeaths++;
            SendProfileToServer();
        }

        public void IncrementWins()
        {
            TotalWins++;
            SendProfileToServer();
        }
        
        public void IncrementAttempts()
        {
            TotalAttempts++;
        }
        
        public void IncrementTokens()
        {
            TokensCollected++;
        }
        
        public void SendProfileToServer()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("gameplay").Document(PlayerId.ToString());
            docRef.SetAsync(this).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Added data to the Player document in the Gameplay collection.");
            });
#else
                String jsonData = JsonUtility.ToJson(this);
                FirebaseWebGLBridge.FirebaseFirestore.AddDocument("gameplay", jsonData, PlayerId.ToString(), "DisplayInfo", "DisplayErrorObject");
#endif
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("PlayerId = " + playerId);
            stringBuilder.Append(" - Attempts = " + totalAttempts);
            stringBuilder.Append(" - Wins = " + totalWins);
            stringBuilder.Append(" - Deaths = " + totalDeaths);
            stringBuilder.Append(" - Kills = " + enemiesKilled);
            stringBuilder.Append(" - Tokens = " + tokensCollected);
            return stringBuilder.ToString();
        }
    }
}