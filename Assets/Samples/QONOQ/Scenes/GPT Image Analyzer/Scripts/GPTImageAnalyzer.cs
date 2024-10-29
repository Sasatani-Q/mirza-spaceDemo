using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using TMPro;

public class GPTImageAnalyzer : MonoBehaviour
{
    [SerializeField] private string openAI_APIKey;
    [SerializeField] private TextAsset openAI_APIKey_Text;
    [SerializeField, TextArea(3, 10)] private string userPrompt = "この画像には何が写っていますか？";
    [SerializeField] private TextMeshProUGUI descriptionStateText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";

    [SerializeField] private RawImage analyzeRawImage;

    private void Awake()
    {
        if (openAI_APIKey_Text)
        {
            openAI_APIKey = openAI_APIKey_Text.text;
        }
    }

    public async UniTask AnalyzeImageAsync(string base64Image)
    {
        await GetImageDescriptionAsync(base64Image);
    }

    private async UniTask GetImageDescriptionAsync(string base64Image)
    {
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new List<object>
                    {
                        new { type = "text", text = userPrompt },
                        new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                    }
                }
            },
            max_tokens = 300
        };

        string json = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(OPENAI_API_URL, " "))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            www.uploadHandler.contentType = "application/json";
            www.SetRequestHeader("Authorization", $"Bearer {openAI_APIKey}");
            www.SetRequestHeader("Content-Type", "application/json");

            descriptionStateText.text = "解析中...";

            await www.SendWebRequest().ToUniTask();

            if (www.result != UnityWebRequest.Result.Success)
            {
                descriptionStateText.text = "解析エラー";
            }
            else
            {
                OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(www.downloadHandler.text);
                string description = response.choices[0].message.content;

                descriptionText.text = description;
                descriptionStateText.text = "解析終了";
            }
        }
    }

    [Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;

        [Serializable]
        public class Choice
        {
            public Message message;
        }

        [Serializable]
        public class Message
        {
            public string content;
        }
    }

    public void SaveSnapshotAsyncForget()
    {
        SaveSnapshotAsync().Forget();
    }

    private async UniTaskVoid SaveSnapshotAsync()
    {
        byte[] bytes;

        if (analyzeRawImage)
        {
            var tex = analyzeRawImage.texture;
            var sw = tex.width;
            var sh = tex.height;
            var result = new Texture2D(sw, sh, TextureFormat.RGBA32, false);
            var currentRT = RenderTexture.active;
            var rt = new RenderTexture(sw, sh, 32);

            Graphics.Blit(tex, rt);
            RenderTexture.active = rt;

            result.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            result.Apply();
            RenderTexture.active = currentRT;

            bytes = result.EncodeToPNG();

            string base64Image = Convert.ToBase64String(bytes);
            await AnalyzeImageAsync(base64Image);
        }
    }
}