using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private static TargetManager instance;

    [Header("Delivery")]
    [SerializeField] protected Transform[] collectionPoints;
    [SerializeField] protected Transform[] deliveryPoints;

    private int currentCollectionPointIndex;
    private int currentDeliveryPointIndex;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public static Transform GetTarget(bool hasPackage) => hasPackage ? instance.GetNextDeliveryPoint() : instance.GetNextCollectionPoint();

    public Transform GetNextCollectionPoint()
    {
        this.currentCollectionPointIndex++;
        if (this.currentCollectionPointIndex >= this.collectionPoints.Length)
            this.currentCollectionPointIndex = 0;

        return this.collectionPoints[this.currentCollectionPointIndex];
    }

    public Transform GetNextDeliveryPoint()
    {
        this.currentDeliveryPointIndex++;
        if (this.currentDeliveryPointIndex >= this.deliveryPoints.Length)
            this.currentDeliveryPointIndex = 0;

        return this.deliveryPoints[this.currentDeliveryPointIndex];
    }
}
