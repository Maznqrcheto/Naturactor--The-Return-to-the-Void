using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public GenerateMap mapGen;
    void Update()
    {
        Vector3 toMove = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        if ((transform.position.x + toMove.x) < 0
            || (transform.position.x + toMove.x) > mapGen.GetComponent<GenerateMap>().x)
            toMove.x = 0;
        if ((transform.position.y + toMove.y) < 0
            || (transform.position.y + toMove.y) > mapGen.GetComponent<GenerateMap>().y)
            toMove.y = 0;

        float mouseScroll = Input.mouseScrollDelta.y;
        if (mouseScroll != 0)
            StartCoroutine(CameraZoom((int)mouseScroll));

        gameObject.transform.Translate(toMove * speed * Time.deltaTime);
    }
    IEnumerator CameraZoom(int zoomAmount)
    {
        if(zoomAmount > 0)
        {
            for (float i = 0; i < zoomAmount; i += 0.1f)
            {
                float ortographicSize = GetComponent<Camera>().orthographicSize;
                ortographicSize = Mathf.Clamp(ortographicSize - i, 5, 20);
                GetComponent<Camera>().orthographicSize = ortographicSize;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            for (float i = 0; i > zoomAmount; i -= 0.1f)
            {
                float ortographicSize = GetComponent<Camera>().orthographicSize;
                ortographicSize = Mathf.Clamp(ortographicSize - i, 5, 20);
                GetComponent<Camera>().orthographicSize = ortographicSize;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
