using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleSphere : MonoBehaviour
{
   [SerializeField] private CollectibleGrouper grouper;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         grouper.AddCollectible();
         Destroy(gameObject);
      }
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawLine(transform.position, grouper.transform.position);
   }
}
