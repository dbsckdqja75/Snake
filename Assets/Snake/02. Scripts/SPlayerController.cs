using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPlayerController : MonoBehaviour
{

    public float moveSpeed;
    public float connectSpeed;

    public List<GameObject> childs = new List<GameObject>();

    public GameObject childPrefab;
    public GameObject energyPrefab;

    private float tempMSpeed, tempCSpeed;

    private float speedTimer;

    private int zoomCount;

    void Start()
    {
        tempMSpeed = moveSpeed;
        tempCSpeed = connectSpeed;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * h * 180 * Time.deltaTime);
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

    void AddBall()
    {
        if (childs.Count > 0)
            childs.Add(Instantiate(childPrefab, childs[childs.Count - 1].transform.position, Quaternion.identity));
        else
            childs.Add(Instantiate(childPrefab, transform.position - transform.forward, Quaternion.identity));

        if ((childs.Count / 8) > zoomCount && zoomCount < 2)
        {
            CameraTarget.offset.y = CameraTarget.offset.y + 12;

            zoomCount = childs.Count / 8;
        }
    }

    void SpeedChange(bool TF = false) // Down | Up
    {
        if (TF) { moveSpeed = moveSpeed * 2; connectSpeed = connectSpeed * 2; speedTimer = 3; }
        else if(!TF) { moveSpeed = tempMSpeed; connectSpeed = tempCSpeed; }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Ball") || col.gameObject.CompareTag("EnemyBall") || col.gameObject.CompareTag("Enemy"))
        {
            for (int i = 0; i < childs.Count; i++)
            {
                Instantiate(energyPrefab, childs[i].transform.position, energyPrefab.transform.rotation);

                Destroy(childs[i]);
            }

            Instantiate(energyPrefab, transform.position, energyPrefab.transform.rotation);

            CameraTarget.offset.y = 34;

            Destroy(gameObject);

            Debug.Log("<b><color=red>플레이어가 죽었습니다.</color></b>");
        }
    }

    void OnTriggerEnter(Collider col) 
    {
        if(col.gameObject.CompareTag("SpeedBall") && speedTimer <= 0)
            SpeedChange(true);
    }
}
