using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointDistributor : MonoBehaviour
{
    public int numberOfPoints = 10;
    public float radius = 5f;
    private List<Vector3> points;
    public Camera mainCamera; // Assign this in the Inspector or find it dynamically

    void Start()
    {
        // If the main camera is not assigned in the inspector, find the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        GeneratePoints();
        StartCoroutine(CaptureViews());
         // Quit the application after all captures are done
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void GeneratePoints()
    {
        points = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            points.Add(Random.onUnitSphere * radius);
        }
    }

    IEnumerator CaptureViews()
    {
        // Ensure the Captures directory exists
        string capturesPath = Application.dataPath + "/Captures";
        System.IO.Directory.CreateDirectory(capturesPath);

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            mainCamera.transform.position = point;
            mainCamera.transform.LookAt(Vector3.zero); // Make the camera face the origin

            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            mainCamera.targetTexture = renderTexture;
            mainCamera.Render();

            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            string filename = $"Capture_{i}.png";
            System.IO.File.WriteAllBytes(capturesPath + "/" + filename, bytes);

            // Cleanup
            RenderTexture.active = null;
            mainCamera.targetTexture = null;
            Object.Destroy(renderTexture);
        }

        // Optionally, reset the camera's position or do additional cleanup here
    }
   
}
