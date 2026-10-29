
/*
 * D4NXE EDITION - Mannequin AI
 * Mantiene el comportamiento original y permite
 * configurar la velocidad de persecución.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class MannequinAIScript : MonoBehaviour
{
    public GameObject player;
    public bool Chasing;
    public NavMeshAgent myAgent;

    [Header("D4NXE - Configuración del enemigo")]
    [Min(0.1f)]
    public float velocidadPersecucion = 4f;

    public GameObject self;
    NavMeshPath path;

    // Sonidos
    public AudioSource chasesound;
    public AudioSource teleportsound;
    bool playing = false;

    // Puntos de detección y aparición
    public SpawnDetectionScript A;
    public SpawnDetectionScript B;
    public SpawnDetectionScript C;
    public SpawnDetectionScript D;
    public SpawnDetectionScript E;

    public SpawnDetectionScript[] SDS;

    private Vector3[] TeleportLocations = new Vector3[]
    {
        new Vector3(0.5f, 1, 22),
        new Vector3(-21, 1, 2),
        new Vector3(2, 1, 2),
        new Vector3(23, 1, -14),
        new Vector3(21, 1, -22)
    };

    public int prefLoc;
    public KillBoxScript mykillbox;

    public void Start()
    {
        SDS = new SpawnDetectionScript[] { A, B, C, D, E };
    }

    void Update()
    {
        if (Chasing)
        {
            NavMeshPath currentPath = new NavMeshPath();

            myAgent.CalculatePath(player.transform.position, currentPath);

            if (currentPath.status == NavMeshPathStatus.PathPartial)
            {
                teleportme();
            }
        }

        if (Chasing &&
            gameObject.GetComponent<Renderer>().isVisible == false)
        {
            myAgent.speed = velocidadPersecucion;
            myAgent.isStopped = false;
            myAgent.SetDestination(player.transform.position);

            playchasesound(true);

            if (mykillbox.getplayer())
            {
                gameover();
            }
        }
        else if (gameObject.GetComponent<Renderer>().isVisible == true)
        {
            playchasesound(false);
            myAgent.isStopped = true;
            myAgent.ResetPath();
        }
    }

    private void playchasesound(bool todo)
    {
        if (!playing && todo)
        {
            chasesound.Play();
            playing = true;
        }
        else if (playing && !todo)
        {
            chasesound.Stop();
            playing = false;
        }
    }

    public void playerdetected()
    {
        Chasing = true;
    }

    public void playerundetected()
    {
        Chasing = false;
    }

    private void teleportme()
    {
        if (!SDS[prefLoc].actorDetected)
        {
            TeleportStart();
        }
        else
        {
            for (int i = 0; i < SDS.Length; i++)
            {
                if (!SDS[i].actorDetected)
                {
                    TeleportStart(i);
                    return;
                }
            }
        }
    }

    private void TeleportStart()
    {
        self.GetComponent<NavMeshAgent>().enabled = false;
        teleportsound.Play();
        self.transform.localPosition = TeleportLocations[prefLoc];
        self.GetComponent<NavMeshAgent>().enabled = true;
    }

    private void TeleportStart(int i)
    {
        self.GetComponent<NavMeshAgent>().enabled = false;
        teleportsound.Play();
        self.transform.localPosition = TeleportLocations[i];
        self.GetComponent<NavMeshAgent>().enabled = true;
    }

    private void TeleportEnd()
    {
    }

    private void gameover()
    {
        SceneManager.LoadScene("GameOverScreen");
    }
}
