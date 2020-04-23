using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float moveSpeed;
    public float connectSpeed;
    public float rotateSpeed = 2;

    private Transform target; // 쫓아갈 타겟

    public List<GameObject> childs = new List<GameObject>();

    public GameObject childPrefab;
    public GameObject energyPrefab;
    public GameObject energyPrefab_IsOnce;

    public float radius;
    public Collider[] cols;

    private Vector3 targetPos;
    private Quaternion targetRotation;

    private float targetDistance = 99;
    private Transform dumpT;

    private bool isDanger; // 위험 상태

    private float tempMSpeed, tempCSpeed, tempRSpeed;
    private float dumpRotL = 90, dumpRotR = 90;

    private float speedTimer;

    void Start()
    {
        tempMSpeed = moveSpeed;
        tempCSpeed = connectSpeed;
        tempRSpeed = rotateSpeed;
    }

    void Update()
    {
        int layerId = 11;
        int layerMask = 1 << layerId;

        RaycastHit enemyBall_Hit;

        RaycastHit r_Hit;
        RaycastHit l_Hit;

        cols = Physics.OverlapSphere(transform.position, radius, layerMask);

        Debug.DrawRay(transform.position, transform.forward * 3, Color.cyan);
        Debug.DrawRay(transform.position, transform.forward * 2 + (transform.right * 3), Color.green);
        Debug.DrawRay(transform.position, transform.forward * 2 + (-transform.right * 3), Color.green);

        if (Physics.SphereCast(transform.position, 3, transform.forward, out enemyBall_Hit, 3, 1 << 0))
        {
            if (enemyBall_Hit.collider)
            {
                isDanger = true;

                Debug.Log("<b><color=red>AI 가 부딪히려고 합니다!</color></b>");
            }
        }

        if (isDanger)
        {
            target = null;
            targetDistance = 99;
        }

        if (Physics.Raycast(transform.position, transform.forward * 2 + (transform.right * 3), out r_Hit, 2, 1 << 0))
        {
            if (r_Hit.collider)
            {
                isDanger = true;
                transform.Rotate(Vector3.down * dumpRotR * Time.deltaTime);

                dumpRotR += dumpRotR * 1.25f * Time.deltaTime; 
            }
            else
                dumpRotR = 90;

            Debug.Log("<b><color=blue>왼쪽으로 회피 기동합니다.</color></b>");
        }

        if (Physics.Raycast(transform.position, transform.forward * 2 + (-transform.right * 3), out l_Hit, 2, 1 << 0))
        {
            if (l_Hit.collider)
            {
                isDanger = true;
                transform.Rotate(Vector3.up * dumpRotL * Time.deltaTime);

                dumpRotL += dumpRotL * 1.25f * Time.deltaTime; 
            }
            else
                dumpRotL = 90;

            Debug.Log("<b><color=blue>오른쪽으로 회피 기동합니다.</color></b>");
        }

        if (!r_Hit.collider && !l_Hit.collider && !enemyBall_Hit.collider)
            isDanger = false;
        else if(r_Hit.collider && l_Hit.collider && enemyBall_Hit.collider)
            transform.Rotate(Vector3.up * Random.Range(-180, 180) * Time.deltaTime);

        if (cols.Length > 0)
        {
            foreach(Collider col in cols)
            {
                if(Vector3.Distance(transform.position, col.transform.position) < targetDistance)
                {
                    targetDistance = Vector3.Distance(transform.position, col.transform.position);

                    target = col.transform;
                    dumpT = col.transform;
                }
            }
        }
        else if(!target && dumpT)
            target = dumpT;

        if (target && !isDanger)
            Targeting();

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (childs.Count > 0)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                if (i > 0)
                    childs[i].transform.position = Vector3.Lerp(childs[i].transform.position, childs[i - 1].transform.position, connectSpeed * Time.deltaTime);
                else
                    childs[i].transform.position = Vector3.Lerp(childs[i].transform.position, transform.position, connectSpeed * Time.deltaTime);
            }
        }

        if(speedTimer <= 0)
            SpeedChange(false);
        else
            speedTimer -= Time.deltaTime;
    }

    void Targeting()
    {
        targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);

        targetRotation = Quaternion.LookRotation(targetPos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (!target.GetComponent<SphereCollider>().enabled)
        {
            target = null;

            targetDistance = 99;
        }
    }

    void AddBall()
    {
        if (childs.Count > 0)
            childs.Add(Instantiate(childPrefab, childs[childs.Count - 1].transform.position, Quaternion.identity));
        else
            childs.Add(Instantiate(childPrefab, transform.position - transform.forward, Quaternion.identity));

        target = null;

        targetDistance = 99;
    }

    void SpeedChange(bool TF = false) // Down | Up
    {
        if (TF) { moveSpeed = moveSpeed * 2; connectSpeed = connectSpeed * 2; rotateSpeed = rotateSpeed * 2; speedTimer = 3; }
        else if(!TF) { moveSpeed = tempMSpeed; connectSpeed = tempCSpeed; rotateSpeed = tempRSpeed; }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ball") || col.gameObject.CompareTag("EnemyBall") || col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < childs.Count; i++)
            {
                if(GameObject.FindGameObjectsWithTag("EnergyBall").Length > 1000)
                    Instantiate(energyPrefab_IsOnce, childs[i].transform.position, energyPrefab.transform.rotation);
                else
                    Instantiate(energyPrefab, childs[i].transform.position, energyPrefab.transform.rotation);

                Destroy(childs[i]);
            }

            Instantiate(energyPrefab, transform.position, energyPrefab.transform.rotation);

            Destroy(gameObject);

            Debug.Log("<b><color=red>AI 가 죽었습니다.</color></b>");
        }
    }

    void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.CompareTag("SpeedBall") && speedTimer <= 0)
            SpeedChange(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.matrix = transform.localToWorldMatrix;
        // Gizmos.DrawWireSphere(Vector3.zero, (radius * 2));
    }
}
