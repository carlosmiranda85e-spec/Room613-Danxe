
/*
 * D4NXE EDITION - Mannequin AI
 * Persecución configurable y teletransporte terrorífico.
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

    [Header("D4NXE - Efecto de aparición")]
    [Min(0f)]
    public float tiempoDesaparicion = 1.2f;

    public GameObject self;
    NavMeshPath path;

    // Sonidos
    public AudioSource chasesound;
    public AudioSource teleportsound;
    bool playing = false;
    bool teleportando = false;

    // Puntos de detección
    public SpawnDetectionScript A;
    public SpawnDetectionScript B;
    public SpawnDetectionScript C;
    public SpawnDetectionScript D;
    public SpawnDetectionScript E;

    public SpawnDetectionScript[] SDS;

    // Ubicaciones de teletransporte
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
        if (teleportando)
            return;

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
            if (chasesound != null)
                chasesound.Play();

            playing = true;
        }
        else if (playing && !todo)
        {
            if (chasesound != null)
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
        if (teleportando || SDS == null || SDS.Length == 0)
            return;

        if (prefLoc >= 0 && prefLoc < SDS.Length &&
            SDS[prefLoc] != null &&
            !SDS[prefLoc].actorDetected)
        {
            TeleportStart();
        }
        else
        {
            for (int i = 0; i < SDS.Length; i++)
            {
                if (SDS[i] != null && !SDS[i].actorDetected)
                {
                    TeleportStart(i);
                    return;
                }
            }
        }
    }

    private void TeleportStart()
    {
        StartCoroutine(TeleportHorrorEffect(prefLoc));
    }

    private void TeleportStart(int i)
    {
        StartCoroutine(TeleportHorrorEffect(i));
    }

    private IEnumerator TeleportHorrorEffect(int location)
    {
        if (location < 0 || location >= TeleportLocations.Length)
            yield break;

        teleportando = true;

        NavMeshAgent agent = self.GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.enabled = false;

        Renderer[] renderers =
            self.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
            r.enabled = false;

        if (teleportsound != null)
            teleportsound.Play();

        yield return new WaitForSeconds(tiempoDesaparicion);

        self.transform.localPosition = TeleportLocations[location];

        foreach (Renderer r in renderers)
            r.enabled = true;

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        teleportando = false;
    }

    private void TeleportEnd()
    {
    }

    private void gameover()
    {
        SceneManager.LoadScene("GameOverScreen");
    }
}
