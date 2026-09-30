using UnityEngine;
using UnityEngine.Events;

/*
 * AnimationEvents receives events from the model's imported animation clips.
 * ThrowAction tells PlayerController to release the axe at the release frame;
 * Step invokes OnStep. The empty methods keep unused clip events from logging
 * missing receivers.
 */

public class AnimationEvents : MonoBehaviour
{
    public UnityEvent OnStep;
    public PlayerController playerController;

    public void ThrowAction()
    {
        if (playerController.togglePrecision == false)
        {
            playerController.LaunchAxe();
        }
        else
        {
            playerController.LaunchAxePrecise();
        }
    }

    public void ChopAction() { }

    public void AnimationDone() { }

    public void Interact() { }

    public void Step()
    {
        OnStep?.Invoke();
    }
}
