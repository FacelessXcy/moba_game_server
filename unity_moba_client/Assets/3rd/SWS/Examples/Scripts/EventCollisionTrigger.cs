/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Events;

namespace SWS
{
    /// <summary>
    /// Can be placed on game objects with colliders to trigger movement script actions.
    /// <summary>
    public class EventCollisionTrigger : MonoBehaviour
    {
        /// <summary>
        /// Checkbox to toggle actions on trigger.
        /// </summary>
        public bool onTrigger = true;

        /// <summary>
        /// Checkbox to toggle actions on collision.
        /// </summary>
        public bool onCollision = true;

        /// <summary>
        /// Unity Events invoked when colliding.
        /// <summary>
        public UnityEvent myEvent;


        void OnTriggerEnter(Collider other)
        {
            if (!onTrigger) return;

            //do something here directly,
            //or assign event methods in the inspector

            myEvent.Invoke();
        }


        void OnCollisionEnter(Collision other)
        {
            if (!onCollision) return;

            //do something here directly,
            //or assign event methods in the inspector

            myEvent.Invoke();
        }


        /// <summary>
        ///  Applies an explosion force to the colliding object.
        /// </summary>
        public void ApplyForce(int power)
        {
            Vector3 position = transform.position;
            float radius = 5f;

            Collider[] colliders = Physics.OverlapSphere(position, radius);
            foreach (Collider hit in colliders)
            {
                navMove move = hit.GetComponent<navMove>();
                if (move != null)
                {
                    move.Stop();
                    hit.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                    hit.isTrigger = false;
                }
                    
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(power, position, radius, 100.0F);
            }
        }
    }
}