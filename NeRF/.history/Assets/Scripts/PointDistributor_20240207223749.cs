using System.Collections;
using UnityEngine;

public class PointDistributor : MonoBehaviour
{
    public int numberOfPoints = 50;
    public float radius = 10f;
    private List<Vector3> points;

    // Start is called before the first frame update
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
        cam.backgroundColor = Color.black; // Set background color to black or any desired color
        cam.clearFlags = CameraClearFlags.Skybox; // Or CameraClearFlags.SolidColor if you want a solid background

        for (int i = 0; i < points.Count; i++)
        {
            // Set camera position and rotation
            Vector3 point = points[i];
            cameraHolder.transform.position = point;
            cameraHolder.transform.LookAt(Vector3.zero); // Assuming the origin is the point of interest

            // Wait for the end of frame, ensuring the camera has rendered
            yield return new WaitForEndOfFrame();

            // Capture the screenshot
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cam.targetTexture = renderTexture;
            cam.Render();

            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            // Convert to PNG and save
            byte[] bytes = texture.EncodeToPNG();
            string filename = $"Capture_{i}.png";
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + filename, bytes);

            // Cleanup
            RenderTexture.active = null; // Reset active render texture
            cam.targetTexture = null;
            Object.Destroy(renderTexture); // Destroy the render texture to free memory

            yield return null; // Optional: add a slight delay or yield return null to space out captures
        }

        // Optionally destroy the camera after use
        Destroy(cameraHolder);
    }
}
