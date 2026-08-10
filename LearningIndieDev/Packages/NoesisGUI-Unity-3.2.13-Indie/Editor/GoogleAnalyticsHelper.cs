public sealed class GoogleAnalyticsHelper
{
    private const string MeasurementId = "G-KE76SPT9HR";
    private const string ApiSecret = "tzdSSCIiTOa0NGt7wEzKfQ";

    public static async void Install(string version)
    {
        try
        {
            string clientId = System.Guid.NewGuid().ToString();
            string url = $"https://www.google-analytics.com/mp/collect?measurement_id={MeasurementId}&api_secret={ApiSecret}";

            string jsonBody = @"
            {
                ""client_id"": """ + clientId + @""",
                ""events"": [
                    {
                        ""name"": ""unity_install"",
                        ""params"": {
                            ""platform"": """ + UnityEngine.Application.platform + @""",
                            ""unity_version"": """ + UnityEngine.Application.unityVersion + @""",
                            ""noesis_version"": """ + version + @"""
                        }
                    }
                ]
            }";

            using (var request = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
            }
        }
        catch (System.Exception) {}
    }
}