using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryImageToExpand : MonoBehaviour
{
    [SerializeField] Transform startPoint; //the position from where the image will start the animation
    [SerializeField] float transitionTime = 5f;
    [SerializeField] float speed = 5f;
    [SerializeField] float scaleIncrease = 3.5f;

    bool move = false;
    Vector3 screenCenter;

    private Image imageToExpand;

    private void Awake()
    {
        imageToExpand = GetComponent<Image>();

        float centerX = (Screen.width * 0.5f);
        float centerY = (Screen.height * 0.5f);
        screenCenter = new Vector3(centerX, centerY, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            Vector3 direction = screenCenter - transform.position;
            Vector3 movement = direction.normalized * speed;
            transform.Translate(movement);
        }


        //check if destination is reached+stop moving
        if (Vector3.Distance(transform.position, screenCenter) <= 2 && move)
        {
            transform.position = screenCenter;
            move = false;
        }
    }

    private void OnEnable()
    {
        //set the position to match startPoint
        transform.position = startPoint.position;

        //transition time from seconds to milliseconds conversion
        float framesForTransition = transitionTime / Time.deltaTime;

        //calculate speed needed to get to the screenCenter
        float distanceToCover = Vector3.Distance(startPoint.position, screenCenter);
        speed = distanceToCover / framesForTransition;

        //start moving
        move = true;

        //scale it up
        StartCoroutine(ScaleImageUp());
    }

    IEnumerator ScaleImageUp()
    {
        Vector3 finalScale = transform.localScale * scaleIncrease;
        Vector3 initiallScale = transform.localScale;
        float distanceToCover = Vector3.Distance(startPoint.position, screenCenter);
        float scaleMagnitudesRaio = initiallScale.magnitude / finalScale.magnitude;

        while (transform.localScale.magnitude <= finalScale.magnitude)
        {
            float currentDistanceFromCenter = Vector3.Distance(transform.position, screenCenter);
            float finalPositionReachedPercentage = 1 - (currentDistanceFromCenter / distanceToCover);

            //We consider the ratio between the normal scale and the one we want to achieve as a percentage

            float slowGrowthRatio = scaleMagnitudesRaio + (finalPositionReachedPercentage * 0.5f);

            if (slowGrowthRatio > finalPositionReachedPercentage)
            {
                transform.localScale = finalScale * slowGrowthRatio;
            }
            else
            {
                transform.localScale = finalScale * finalPositionReachedPercentage;
            }
            yield return null;
        }
        transform.localScale = finalScale;
    }

    private void OnDisable()
    {
        transform.localScale = transform.localScale / scaleIncrease;
    }
}
