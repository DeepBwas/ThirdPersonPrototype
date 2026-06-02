using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0))
            : Vector2.zero;

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        transform.Translate(movement * speed * Time.deltaTime);
    }
}
