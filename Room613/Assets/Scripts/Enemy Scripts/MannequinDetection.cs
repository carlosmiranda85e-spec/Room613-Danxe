
/*
 * D4NXE EDITION - Mannequin Detection
 * Detección configurable del jugador.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinDetection : MonoBehaviour
{
    [Header("D4NXE - Detección")]
    public MannequinAIScript thisMannequin;

    [Tooltip("Activar o desactivar la detección del jugador.")]
    public bool deteccionActiva = true;

    private bool jugadorDentro = false;

    private void Start()
    {
        if (thisMannequin != null)
        {
            thisMannequin.playerundetected();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!deteccionActiva)
            return;

        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;

            if (thisMannequin != null)
            {
                thisMannequin.playerdetected();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;

            if (thisMannequin != null)
            {
                thisMannequin.playerundetected();
            }
        }
    }

    public void ActivarDeteccion(bool activar)
    {
        deteccionActiva = activar;

        if (!activar && jugadorDentro)
        {
            jugadorDentro = false;

            if (thisMannequin != null)
            {
                thisMannequin.playerundetected();
            }
        }
    }
}
