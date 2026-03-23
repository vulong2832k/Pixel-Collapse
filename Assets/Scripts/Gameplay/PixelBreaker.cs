using UnityEngine;

public class PixelBreaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject cube = hit.collider.gameObject;

                var destruct = cube.GetComponentInParent<PixelDestructible>();

                if (destruct != null)
                {
                    destruct.BreakCube(cube);
                }
            }
        }
    }
}