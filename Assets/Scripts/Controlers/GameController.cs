using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Grid mGrid;

    public float conveyerBeltAnimTime;
    private IEnumerator conveyerBeltAnimCoroutine;
    public List<ConveyerBelt> conveyerBeltList;

    public float conveyerBeltAnimDelay;

    // Start is called before the first frame update
    void Start()
    {
        conveyerBeltList = new List<ConveyerBelt>();
        conveyerBeltAnimTime = 0.0f;
        conveyerBeltAnimCoroutine = IncrementConveyerBeltAnimPer(conveyerBeltAnimDelay);
        StartCoroutine(conveyerBeltAnimCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // TODO add an incrementer to go over list and increment the nimation of all belts before continuing
    private IEnumerator IncrementConveyerBeltAnimPer(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            foreach (ConveyerBelt belt in conveyerBeltList)
            {
                Animator animator = belt.GetComponent<Animator>();
                animator.SetFloat("animTime", conveyerBeltAnimTime);
            }

            conveyerBeltAnimTime += 0.125f;
        }
    }

    public void RemoveConveyerBelt(ConveyerBelt belt)
    {
        if (conveyerBeltList.Remove(belt))
        {
            Debug.Log("Removed Belt");
        }
        else
        {
            Debug.Log("Failed to remove belt");
        }
    }
}