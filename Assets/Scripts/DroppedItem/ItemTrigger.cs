using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    private Rigidbody rb;
    private bool isAttracting = false;
    private Transform target;

    private void StartAttract(Transform player)
    {
        isAttracting = true;
        target = player;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        if (other.CompareTag("PickupTrigger"))
        {
            //Debug.Log("Pickup");
            StartAttract(other.transform.parent);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttracting)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);
            float speed = Mathf.Lerp(5f, 20f, 1 - distance / 5f);
            transform.position += direction * speed * Time.deltaTime;
            if (distance < 0.5f)
            {
                target.GetComponent<PlayerControl>().PickUp(GetComponentInChildren<DroppedItem>().blockType);
                Destroy(gameObject);
            }

        }
    }
}
