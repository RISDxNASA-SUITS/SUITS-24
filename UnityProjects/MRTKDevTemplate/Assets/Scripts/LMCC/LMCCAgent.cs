using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;

public class LMCCAgent : MonoBehaviour
{
    string url = "http://localhost:5000";
    float time_since_last_update;

    // Update is called once per frame
    void Update()
    {

    }

    public record UpdateStatePayload
    {
        public string taskName;
        public int step;
    }

    public void PostUpdateState(string taskName, int step)
    {
        UpdateStatePayload payload = new UpdateStatePayload { taskName = taskName, step = step };

        IEnumerator UpdateState()
        {
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var request = new UnityWebRequest(url + "/update-state", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                // Handle the response from the Flask backend
                Debug.Log("Response: " + responseJson);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        StartCoroutine(UpdateState());
    }
}