using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{

    public bool isOnce;

    private float timer;

    private Animation anim;

    void Start()
    {
        if(isOnce)
            timer = Random.Range(10, 31);
    }

    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * 90 * Time.deltaTime);

        if (timer > 0 && isOnce)
            timer -= Time.deltaTime;
        else if(timer < 0 && !anim.isPlaying && isOnce)
            anim.Play("OffEnergyBall");
    }

    void ScoreUp()
    {
        SGameManager.ScoreUp();

        Debug.Log("<b><color=green>Get Energy</color></b>");
    }

    public void End()
    {
        if(isOnce)
            Destroy(gameObject);

        transform.position = new Vector3(Random.Range(-40, 41), 0, Random.Range(-40, 41));

        anim.Play("OnEnergyBall");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("EnergyBall"))
        {
            End();
            Debug.Log("겹침");
        }

        if (col.gameObject.CompareTag("Player"))
            ScoreUp();

        if(col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Enemy"))
            col.gameObject.SendMessage("AddBall", SendMessageOptions.DontRequireReceiver);

        anim.Play("OffEnergyBall");
    }
}
