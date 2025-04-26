using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public GenerateMap mapGen;
    void Update()
    {
        Vector3 toMove = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        if ((transform.position.x + toMove.x) < 0
            || (transform.position.x + toMove.x) > mapGen.GetComponent<GenerateMap>().x)
            toMove.x = 0;
        if ((transform.position.y + toMove.y) < 0
            || (transform.position.y + toMove.y) > mapGen.GetComponent<GenerateMap>().y)
            toMove.y = 0;

        float mouseScroll = Input.mouseScrollDelta.y;
        float ortographicSize = GetComponent<Camera>().orthographicSize;
        ortographicSize = Mathf.Clamp(ortographicSize - mouseScroll, 5, 20);
        GetComponent<Camera>().orthographicSize = ortographicSize;

        gameObject.transform.Translate(toMove * speed * Time.deltaTime);
    }
}
