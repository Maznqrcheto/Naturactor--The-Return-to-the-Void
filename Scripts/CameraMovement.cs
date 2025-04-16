using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public GenerateMap mapGen;
    void Update()
    {
        Vector3 toMove = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if ((transform.position.x + toMove.x) < 35
            || (transform.position.x + toMove.x) > mapGen.GetComponent<GenerateMap>().x - 35)
            toMove.x = 0;
        if ((transform.position.y + toMove.y) < 19.5f
            || (transform.position.y + toMove.y) > mapGen.GetComponent<GenerateMap>().y - 19.5f)
            toMove.y = 0;

        gameObject.transform.Translate(toMove * speed * Time.deltaTime);
    }
}
