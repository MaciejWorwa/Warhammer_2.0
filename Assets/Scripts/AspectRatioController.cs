using UnityEngine;

public class AspectRatioController : MonoBehaviour
{
    //private int lastWidth = 0;
    //private int lastHeight = 0;

    //void Update()
    //{
    //    if (((Screen.width / Screen.height) > 1.8 || (Screen.width / Screen.height) < 1.6) && Screen.fullScreen == false)
    //    {
    //        var width = Screen.width; var height = Screen.height;

    //        if (lastWidth != width) // if the user is changing the width
    //        {
    //            // update the height
    //            var heightAccordingToWidth = width / 16.0 * 9.0;
    //            Screen.SetResolution(width, (int)Mathf.Round((float)heightAccordingToWidth), false, 0);
    //        }
    //        else if (lastHeight != height) // if the user is changing the height
    //        {
    //            // update the width
    //            var widthAccordingToHeight = height / 9.0 * 16.0;
    //            Screen.SetResolution((int)Mathf.Round((float)widthAccordingToHeight), height, false, 0);
    //        }
    //        lastWidth = width;
    //        lastHeight = height;
    //    }
    //}
}