using UnityEngine;

public class Player : MonoBehaviour
{
   [SerializeField] private Rigidbody _rigidbody;

    private void Start()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
     private void Update()
    {
        Debug.Log("Я помню чудное мгновенье:");
        Debug.Log("Передо мной явилась ты,");
        Debug.Log("Как мимолетное виденье,");
    }

    private void Move(float direction)
    {
        
    }
}
