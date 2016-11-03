﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour {

    public GameObject avatar_;
    private MeshRenderer[] ren;
    private GameObject bar;

    [SyncVar] private Color color;

    void Start()
    {
        ren = GetComponentsInChildren<MeshRenderer>();
        if (!isLocalPlayer)
        {
            GetComponentInChildren<FirstPersonController>().enabled = false;
            GetComponentInChildren<AudioSource>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<CharacterController>().enabled = false;

            CmdSetColor(color);
            foreach (MeshRenderer i in ren)
            {
                i.material.color = color;
            }
        }
        else
        {
            CmdSetColor(color);
            bar = GameObject.FindGameObjectWithTag("ColorBar");
            bar.GetComponentInChildren<Image>().color = color;
            DontDestroyOnLoad(bar);
            avatar_.SetActive(false);
        }
    }

    void OnDestroy()
    {
        bar.GetComponentInChildren<Image>().color = Color.white;
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            RpcSetColor(color);
        }
    }

    [ClientRpc] void RpcSetColor(Color c)
    {
        color = c;
    }

    [Command] void CmdSetColor(Color c)
    {
        color = c;
    }
}
