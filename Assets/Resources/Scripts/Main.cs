﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Player player; // префаб игрока
    public Transform rocketsPool; // пул прожектайлов
    public Transform healthPanelsPool; // пул UI панелей здоровья
    public float matrixCoeff; // коэффициент замедления времени во время движения игрока 
    public float curSlowerCoeff; // текущее замедление (либо нормальная скорость [=1], либо пониженная [=matrixCoeff])
    public Image EnergySlider;
    public Text EnergyCount;

    public List<Enemy> enemies = new List<Enemy>();

    public Text MessagePanel;


    void Start()
    {
        // заполняем пул прожектайлами
        for (int i = 0; i < 100; i++)
        {
            GameObject rocket = Instantiate(Resources.Load<GameObject>("Prefabs/Rocket")) as GameObject;
            rocket.transform.SetParent(rocketsPool);
            rocket.GetComponent<Rocket>().main = this;
        }

        player = FindObjectOfType<Player>();
        Transform hPanel = Instantiate(Resources.Load<GameObject>("Prefabs/healthPanel")).transform;
        hPanel.SetParent(healthPanelsPool);
        hPanel.localScale = new Vector3(1, 1, 1);
        player.healthPanel = hPanel;
        player.healthSlider = hPanel.GetChild(0).GetComponent<Image>();

        curSlowerCoeff = 1; // на старте игры скорость игры нормальная
        enemies = FindObjectsOfType<Enemy>().ToList();
        foreach (Enemy e in enemies)
        {
            e.main = this;
            Transform hp = Instantiate(Resources.Load<GameObject>("Prefabs/healthPanel")).transform;
            hp.SetParent(healthPanelsPool);
            hp.localScale = new Vector3(1, 1, 1);
            e.healthPanel = hp;
            e.healthSlider = hp.GetChild(0).GetComponent<Image>();
        }
    }

    public void BodyHitReaction(MeshRenderer mr, MaterialPropertyBlock MPB, Color color)
    {
        StartCoroutine(ChangeBodyColor(mr, MPB, color));
    }

    IEnumerator ChangeBodyColor(MeshRenderer mr, MaterialPropertyBlock MPB, Color color)
    {
        mr.GetPropertyBlock(MPB);
        MPB.SetColor("_Color", Color.red);
        mr.SetPropertyBlock(MPB);

        yield return new WaitForSeconds(0.2f);

        if (mr != null)
        {
            mr.GetPropertyBlock(MPB);
            MPB.SetColor("_Color", color);
            mr.SetPropertyBlock(MPB);
        }
    }
}
