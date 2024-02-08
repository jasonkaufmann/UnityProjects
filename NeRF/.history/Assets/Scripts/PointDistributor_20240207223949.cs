using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PointDistributor : MonoBehaviour
{
    public int numberOfPoints = 50;
    public float radius = 10f;
    private List<Vector3> points;

    void Start()
    {
        GeneratePoints();
        StartCoroutine(CaptureViews());
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
        GameObject cameraHolder = new GameObject("CameraHolder");
        Camera cam = cameraHolder.AddComponent<Camera>();

        // Ensure the Captures directory exists
        string capturesPath = Application.dataPath + "/Captures";
        System.IO.Directory.CreateDirectory(capturesPath);

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            cameraHolder.transform.position = point;
            cameraHolder.transform.LookAt(Vector3.zero);

            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cam.targetTexture = renderTexture;
            cam.Render();

            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            string filename = $"Capture_{i}.png";
            System.IO.File.WriteAllBytes(capturesPath + "/" + filename, bytes);

            RenderTexture.active = null;
            cam.targetTexture = null;
            Object.Destroy(renderTexture);
        }

        Destroy(cameraHolder);
    }
}
