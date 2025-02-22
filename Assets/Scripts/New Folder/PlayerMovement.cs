using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 移動速度

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A(-1) / D(1)
        float moveZ = Input.GetAxis("Vertical");   // W(1) / S(-1)

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
