using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyState enemyState;

        [SerializeField] private float speed;
        [SerializeField] private Vector2 startLocation;
        [SerializeField] private float startWaitTime;

        [SerializeField] private Transform nextSpot;
        
        [SerializeField] private float attackRange;
        [SerializeField] private GameObject target;

        [SerializeField] private float patrolRange;
        [SerializeField] private SpriteRenderer spriteRender;

        private float min_X;
        private float max_X;
        private float waitTime;

        public enum EnemyState
        {
            Patrolling,
            Chasing,
            Attacking
        }

        private void Awake()
        {
            startLocation = transform.position;
            min_X = startLocation.x - patrolRange;
            max_X = startLocation.x + patrolRange;
            waitTime = startWaitTime;
            

            nextSpot.position = new Vector2(Random.Range(min_X, max_X), startLocation.y);
        }

        private void CheckingState(EnemyState currentState)
        {
            if (currentState == EnemyState.Patrolling)
            {
                Debug.Log("Patrolling");
                Patrolling();
            }
            else if(currentState == EnemyState.Chasing)
            {
                Debug.Log("Chasing");
                Chasing();
            }
            else if(currentState == EnemyState.Attacking)
            {
                Debug.Log("Attacking");
            }
        }
        private EnemyState CheckingAttack()
        {
            if (attackRange >= Vector2.Distance(target.transform.position, this.transform.position))
            {
                return EnemyState.Attacking;
            }
            else return EnemyState.Chasing;
        }

        private void Patrolling()
        {
            transform.position = Vector2.MoveTowards(transform.position, nextSpot.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, nextSpot.position) < 0.25f)
            {
                if (waitTime <= 0)
                {
                    nextSpot.position = new Vector2(Random.Range(min_X, max_X), startLocation.y);
                    waitTime = startWaitTime;
                }
                else waitTime -= Time.deltaTime;
            }
        }
        private void Chasing()
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                target = collision.gameObject;
                enemyState = CheckingAttack();
            }
        }

        private void Update()
        {
            CheckingState(enemyState);
            if (target != null)
            {
                if (Vector2.Distance(target.transform.position, this.transform.position) >= patrolRange)
                {
                    enemyState = EnemyState.Patrolling;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(min_X, startLocation.y), new Vector2(max_X, startLocation.y));
        }
    }
}
