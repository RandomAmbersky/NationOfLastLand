using UnityEngine;
using System.Linq;

public class CarManager : MonoBehaviour
{

    [SerializeField] private bool IsMoving;
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 2.0f; 
    [SerializeField] private float rotationSpeed = 2.0f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            ScanTarget();
        }

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

             Vector3 direction = target.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // Плавный переход к нужному углу
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ScanTarget () 
    {
        int layerMask = LayerMask.NameToLayer("Events");
        if (layerMask == -1)
        {
            Debug.LogWarning("Слой не найден: Events");
            return;
        }

// Используем FindObjectsByType вместо FindObjectsOfType
GameObject[] objectsInLayer = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
    .Where(go => go.activeInHierarchy && go.layer == layerMask)
    .Where(go => go.TryGetComponent<SpriteRenderer>(out _)) // Только спрайты
    .ToArray();

        if (objectsInLayer.Length == 0)
        {
            Debug.Log("Объекты на слое 'Events' не найдены.");
            return;
        }

        // Ищем ближайший
        Transform playerTransform = this.transform;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in objectsInLayer)
        {
            float distance = Vector3.Distance(playerTransform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = obj.transform;
            }
        }

        // Debug.Log($"Ближайший объект: {closestObject.name}, расстояние: {minDistance:F2}");
    }
}
