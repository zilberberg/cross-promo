using Assets.CrossPromo.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CrossPromoManager : MonoBehaviour
{
    [SerializeField]
    int playerId = 0;
    [SerializeField]
    bool shouldIgnoreColliders = true;

    private string serverUrl = "https://run.mocky.io/v3/75deb965-b835-4591-b8f3-f04e5952d73b";
    private int videosCounter = 0;
    private int videosCount = 0;
    private List<videoData> videoDataList = new List<videoData>();
    private bool isInitialized = false;
    private int indexToPlay = 0;
    private VideoPlayerManager vpMgr;

    // Start is called before the first frame update
    void Start()
    {
        vpMgr = gameObject.GetComponent<VideoPlayerManager>();
        StartCoroutine(GetData());
    }

    // Update is called once per frame
    void Update()
    {
        if (videosCount > 0 && videosCounter == videosCount && !isInitialized)
        {
            isInitialized = true;
            vpMgr.SetVideoPlayerUrl(videoDataList[indexToPlay]);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                if (shouldIgnoreColliders && hitInformation.collider != null)
                {
                   // Ignored collider clicked
                } else
                {
                    OnClick();
                }
            }
        }
    }

    private void OnClick()
    {

        if (!videoDataList[indexToPlay].isTracked)
        {
            videoDataList[indexToPlay].isTracked = true;
            StartCoroutine(TrackClick(videoDataList[indexToPlay]));
        }
        Application.OpenURL(videoDataList[indexToPlay].click_url);
    }

    private IEnumerator GetData()
    {
        UnityWebRequest wwwServer = UnityWebRequest.Get(serverUrl);
        yield return wwwServer.SendWebRequest();

        if (wwwServer.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("server url error");
        }
        else
        {
            ProcessJsonData(wwwServer.downloadHandler.text);
        }
    } 

    private void ProcessJsonData(string json)
    {
        jsonDataClass jsonData = JsonUtility.FromJson<jsonDataClass>(json);
        videosCount = jsonData.results.Count;

        foreach (videoData video in jsonData.results)
        {
            StartCoroutine(DownloadVideo(video));
        }
    }

    private IEnumerator DownloadVideo(videoData vd)
    {
        
        UnityWebRequest wwwVideo = UnityWebRequest.Get(vd.video_url);
        yield return wwwVideo.SendWebRequest();

        if (wwwVideo.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(wwwVideo.error);
        }
        else
        {
            string path = Application.persistentDataPath + "Video" + vd.id + ".mp4";
            File.WriteAllBytes(path, wwwVideo.downloadHandler.data);

            vd.videoPath = path;
            vd.isTracked = false;
            videoDataList.Add(vd);
            videosCounter++;
        }
    }

    public videoData GetPreviousVideo()
    {
        if (indexToPlay == 0)
        {
            indexToPlay = (videosCount - 1);
        } else
        {
            indexToPlay--;
        }
        videoData previousVideo = videoDataList[indexToPlay];
        return previousVideo;
    }

    public videoData GetNextVideo()
    {
        indexToPlay++;
        if (indexToPlay > (videosCount - 1))
        {
            indexToPlay = 0;
        }
        videoData nextVideo = videoDataList[indexToPlay];
        return nextVideo;
    }

    private IEnumerator TrackClick(videoData vd)
    {
        string url = vd.tracking_url;
        url = url.Replace("[PLAYER_ID]", playerId.ToString());

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!" + www.result);
        }
    }

    public bool VideosLoaded()
    {
        return videoDataList.Count == videosCount;
    }
}
