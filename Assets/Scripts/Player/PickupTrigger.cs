using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    private PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
