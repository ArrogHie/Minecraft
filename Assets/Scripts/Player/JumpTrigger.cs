using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private readonly HashSet<Collider> chunkContacts = new HashSet<Collider>();

    public bool IsTouchingChunk()
    {
        return chunkContacts.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            chunkContacts.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            chunkContacts.Remove(other);
        }
    }

    private void OnDisable()
    {
        chunkContacts.Clear();
    }
}
