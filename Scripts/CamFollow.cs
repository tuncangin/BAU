using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - player.transform.position;  //startta obje ile arasındaki vektörü offset olarak çekiyor ve update'te kullanıyor.
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
