using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float destroyAfterSeconds;

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(destroyAfterSeconds));
    }


    IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
