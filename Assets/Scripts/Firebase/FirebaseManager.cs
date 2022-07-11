using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using FirebaseWebGLBridge = FirebaseWebGL.Scripts.FirebaseBridge;
using UnityEngine;

#if !UNITY_WEBGL || UNITY_EDITOR
using System.Text;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
#endif

namespace Firebase
{
    public class FirebaseManager : MonoBehaviour
    {
        [field: SerializeField] public GameObject playerListItemPrefab { get; private set; }
        [field: SerializeField] public GameObject playerListPrefab { get; private set; }

        public string callback;
        
        private void Start()
        {
            GetAllPlayerData();
        }

        public void GetAllPlayerData()
        {
            Debug.Log("Getting Player Data");
#if !UNITY_WEBGL || UNITY_EDITOR
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            Query allPlayers = db.Collection("gameplay");
            allPlayers.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot allPlayersQuerySnapshot = task.Result;
                foreach (DocumentSnapshot documentSnapshot in allPlayersQuerySnapshot.Documents)
                {
                    Debug.Log(String.Format("Document data for {0} document:", documentSnapshot.Id));
                    Dictionary<string, object> playerData = documentSnapshot.ToDictionary();
                    var stringBuilder = new StringBuilder();
                    foreach (KeyValuePair<string, object> pair in playerData)
                    {
                        stringBuilder.Append(String.Format("{0}: {1} - ", pair.Key, pair.Value));
                    }
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                    var newItem = Instantiate(playerListItemPrefab, playerListPrefab.transform);
                    newItem.GetComponent<PlayerDataItemUI>().DataText = stringBuilder.ToString();
                    // Newline to separate entries
                    Debug.Log("");
                }
            });
#else
            var newItem = Instantiate(playerListItemPrefab, playerListPrefab.transform);
            FirebaseWebGLBridge.FirebaseFirestore.GetDocumentsInCollection("gameplay", gameObject.name, "OnRequestSuccess", "DisplayErrorObject");
#endif
        }
        #if UNITY_WEBGL
        public void OnRequestSuccess(string data)
        {
            Debug.Log("Data: "+data);
            var parsedData = JsonConvert.DeserializeObject<List<PlayerData>>(data);
            foreach (var player in parsedData)
            {
                Debug.Log("Player Data"+ player);
                var newItem = Instantiate(playerListItemPrefab, playerListPrefab.transform);
                newItem.GetComponent<PlayerDataItemUI>().DataText = player.ToString();
            }
        }
        #endif
    }
}
