using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        bool crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S);

        if (animator != null)
            animator.SetBool("isCrouching", crouch);
    }
}
