using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTowerController : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    private Transform target;
    [SerializeField]
    private Transform lookAtPivot;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player1").transform;
        PoolManager.instance.CreatePool(projectilePrefab, 10);
        StartCoroutine(IntermittentProjectileFire());
    }
    

    private IEnumerator IntermittentProjectileFire()
    {
        while (target != null)
        {
            AimFire();
            yield return new WaitForSeconds(1);
        }
    }

    private void AimFire()
    {
        Vector3 adjustedTargetPosition = new Vector3(target.position.x, target.position.y + 0.25f, 0f);
        Vector3 diff = adjustedTargetPosition - lookAtPivot.position;
        diff.Normalize();
        float zRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        lookAtPivot.rotation = Quaternion.Euler(0f, 0f, zRotation);
        PoolManager.instance.ReuseObject(projectilePrefab, lookAtPivot.position, lookAtPivot.rotation, this.tag);
    }

}
