using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SGameManager : MonoBehaviour
{

    private bool isOver;

    public static int score;
    private int tempScore;

    public Text scoreText;
    public Text resultText;
    public Animation uiAnim;

    public GameObject enemyPrefab;

    public GameObject energyBallPrefab;
    public GameObject speedBallPrefab;

    private Transform playerT;

    private AudioSource musicSource;

    public AudioClip[] musicClips;

    private List<GameObject> energys = new List<GameObject>();

    private float playTimer;
    private float scoreTimer;

    public bool isMainScene;

    void Awake()
    {
        musicSource = GetComponent<AudioSource>();    
    }

    void Start()
    {
        score = 0;
        tempScore = 0;

        playTimer = 0;

        musicSource.clip = musicClips[Random.Range(0, musicClips.Length)];
        musicSource.Play();

        if (!isMainScene)
        {
            playerT = GameObject.FindGameObjectWithTag("Player").transform;
            SpawnPlayerAround();
        }
        else
            SpawnFloorAround();
    }

    void Update()
    {
        if(!isOver)
        {
            if (score > tempScore && scoreTimer <= 0)
            {
                tempScore++;
                scoreTimer = 0.1f;
            }
            else if (scoreTimer > 0)
                scoreTimer -= 10 * Time.deltaTime;

            if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
                SpawnEnemy(10);

            scoreText.text = "Score : " + tempScore.ToString("###,###,##0");

            playTimer += Time.deltaTime;

            if (!playerT && !isMainScene)
            {
                isOver = true;

                resultText.text = "Score : " + score.ToString("###,###,##0") + "\n" + "Play Time : " + playTimer.ToString("00:00:00");

                uiAnim.Play("OffUI");
            }
        }
    }

    void SpawnPlayerAround()
    {
        for(int i = 0; i <= 10; i++)
        {
            Vector3 randomV = new Vector3(playerT.position.x + Random.Range(-10, 10), 0, playerT.position.z + Random.Range(-10, 10));

            energys.Add(Instantiate(energyBallPrefab, randomV, energyBallPrefab.transform.rotation));
        }

        SpawnFloorAround();
    }

    void SpawnFloorAround()
    {
        for (int i = 0; i <= 30; i++)
        {
            Vector3 randomV = new Vector3(Random.Range(-40, 41), 0, Random.Range(-40, 41));

            energys.Add(Instantiate(energyBallPrefab, randomV, energyBallPrefab.transform.rotation));
        }

        for (int i = 0; i <= 20; i++)
        {
            Vector3 randomV = new Vector3(Random.Range(-40, 41), 0, Random.Range(-40, 41));

            Instantiate(speedBallPrefab, randomV, speedBallPrefab.transform.rotation);
        }
        
        SpawnEnemy(10);
    }

    void SpawnEnemy(int value)
    {
        for (int i = 0; i <= value; i++)
        {
            Vector3 randomV = new Vector3(Random.Range(-40, 41), 0, Random.Range(-40, 41));

            Instantiate(enemyPrefab, randomV, enemyPrefab.transform.rotation);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SInGame");
    }

    public void QuitGame(bool isReload)
    {
        if(!isReload)
            Application.Quit();
        else
            SceneManager.LoadScene(3);
    }

    public static void ScoreUp()
    {
        score += 10;
    }
}
