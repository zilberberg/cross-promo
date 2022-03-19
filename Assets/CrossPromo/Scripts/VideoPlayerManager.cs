using Assets.CrossPromo.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    VideoPlayer videoPlayer;
    CrossPromoManager crossPromoManager;
    // Start is called before the first frame update
    void Start()
    {
        crossPromoManager = gameObject.GetComponent<CrossPromoManager>();
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        GameObject camera = GameObject.Find("Main Camera");
        videoPlayer = camera.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
        videoPlayer.isLooping = true;
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVideoPlayerUrl(videoData _videoData)
    {
        videoPlayer.url = _videoData.videoPath;
        videoPlayer.Play();
    }

    private void EndReached(VideoPlayer vp)
    {
        Next();
    }

    public void Next()
    {
        if (crossPromoManager.VideosLoaded())
        {
            videoData nextVideo = crossPromoManager.GetNextVideo();
            PlayVideo(nextVideo);
        }
    }

    public void Previous()
    {
        if (crossPromoManager.VideosLoaded())
        {
            videoData nextVideo = crossPromoManager.GetPreviousVideo();
            PlayVideo(nextVideo);
        }
    }

    public void Pause()
    {
        if (!videoPlayer.isPaused)
        {
            videoPlayer.Pause();
        }
    }

    public void Resume()
    {
        if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
    }

    private void PlayVideo(videoData vd)
    {
        videoPlayer.url = vd.videoPath;
        videoPlayer.Play();
    }
}
