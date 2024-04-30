using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    [SerializeField] ParticleSystem movementParticle;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 4f)]
    [SerializeField] int dustFormationPeriod;

    public Rigidbody2D car;

    float counter;

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (Mathf.Abs(car.velocity.x) > occurAfterVelocity)
        {
            if(counter > dustFormationPeriod)
            {
                movementParticle.Play();
                counter = 0;
            }
        }
    }
}
