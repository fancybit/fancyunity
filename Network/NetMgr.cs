using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace FancyUnity
{
    public class NetMgr<T> : Singleton<T> where T : Component
    {
        public void Get<TRespData>(string url, Action<TRespData> callback)
            => StartCoroutine(GetRequest(url, callback));

        public IEnumerator GetRequest<TRespData>(string url, Action<TRespData> callback)
        {
            using (var webRequest = UnityWebRequest.Get(url))
            {
                var req = webRequest.SendWebRequest();
                yield return req;

                // 出现网络错误
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                }
                else// 正常处理
                {
                    // 获取到的数据
                    string strData = webRequest.downloadHandler.text;
                    var respData = JsonConvert.DeserializeObject<TRespData>(strData);
                    callback?.Invoke(respData);
                }
            }
        }

        public void Post<TRespData, TReqData>(string url, TReqData postData,
            Action<TRespData> callback)
            => StartCoroutine(PostRequest(url, postData, callback));

        public IEnumerator PostRequest<TRespData, TReqData>(
            string url, TReqData postData, Action<TRespData> callback)
        {
            var jsonData = JsonConvert.SerializeObject(postData);
            var form = new Dictionary<string, string>();
            form.Add("data", jsonData);
            using (var webRequest = UnityWebRequest.Post(url, form))
            {
                var req = webRequest.SendWebRequest();
                yield return req;

                // 出现网络错误
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                }
                else// 正常处理
                {
                    // 获取到的数据
                    string strData = webRequest.downloadHandler.text;
                    var respData = JsonConvert.DeserializeObject<TRespData>(strData);
                    callback?.Invoke(respData);
                }
            }
        }

        public string UploadFile(string url, string fileName, string token = null)
        {
            var client = new WebClient();
            client.QueryString.Add("token", token);
            var byteResp = client.UploadFile(url, fileName);
            return Encoding.UTF8.GetString(byteResp);
        }
    }
}
