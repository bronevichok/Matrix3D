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
    public Transform deathEffectsPool; // пул эффектов смерти
    public float matrixCoeff; // коэффициент замедления времени во время движения игрока 
    public float curSlowerCoeff; // текущее замедление (либо нормальная скорость [=1], либо пониженная [=matrixCoeff])
    public Image EnergySlider;
    public Text EnergyCount;

    public List<Enemy> enemies = new List<Enemy>();

    public Text MessagePanel;


    void Start()
    {
        // заполняем пул прожектайлами
        for (int i = 0; i < 50; i++)
        {
            GameObject rocket = Instantiate(Resources.Load<GameObject>("Prefabs/Rocket")) as GameObject;
            rocket.transform.SetParent(rocketsPool);
            rocket.GetComponent<Rocket>().main = this;
        }

        // заполняем пул эффектами смерти
        for (int i = 0; i < 20; i++)
        {
            GameObject DE = Instantiate(Resources.Load<GameObject>("Prefabs/DeathEffect")) as GameObject;
            DE.transform.SetParent(deathEffectsPool);
        }

        // находим игрока на сцене
        player = FindObjectOfType<Player>();
        // инстанциируем для игрока хэлс бар
        Transform hPanelp = Instantiate(Resources.Load<GameObject>("Prefabs/healthPanel")).transform;
        hPanelp.SetParent(healthPanelsPool);
        hPanelp.localScale = new Vector3(1, 1, 1);
        player.healthPanel = hPanelp;
        player.healthPanelScript = hPanelp.GetComponent<HealthPanel>();

        curSlowerCoeff = 1; // на старте игры скорость игры нормальная
        
        // находим врагов на сцене
        enemies = FindObjectsOfType<Enemy>().ToList();
        foreach (Enemy e in enemies)
        {
            e.main = this;
            // инстанциируем для врагов хэлс бары
            Transform hPanele = Instantiate(Resources.Load<GameObject>("Prefabs/healthPanel")).transform;
            hPanele.SetParent(healthPanelsPool);
            hPanele.localScale = new Vector3(1, 1, 1);
            e.healthPanel = hPanele;
            e.healthPanelScript = hPanele.GetComponent<HealthPanel>();
         }
    }

    public void BodyHitReaction(MeshRenderer mr, MaterialPropertyBlock MPB, Color color)
    {
        StartCoroutine(ChangeBodyColor(mr, MPB, color));
    }

    // меняем цвет тушки
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

    public void EnemyDie(Enemy e)
    {
        StartCoroutine(EnemyDeath(e));
    }

    // убиваем врага
    IEnumerator EnemyDeath(Enemy e)
    {
        enemies.Remove(e);
        e.healthPanel.GetComponent<Image>().enabled = false;
        e.enabled = false;
        foreach (MeshRenderer mr in e.GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
        e.GetComponent<Collider>().enabled = false;

        Transform deathEffect = deathEffectsPool.GetChild(0);
        deathEffect.SetParent(null);
        deathEffect.position = e.transform.position;

        yield return new WaitForSeconds(1);

        deathEffect.SetParent(deathEffectsPool);
        Destroy(e.gameObject);
        Destroy(e.healthPanel.gameObject);

        if (enemies.Count == 0)
        {
            MessagePanel.text = "ТЫ ПОБЕДИЛ!\n ёпта";
        }
    }

    public void PlayerDie(Player p)
    {
        StartCoroutine(PlayerDeath(p));
    }

    // убиваем игрока
    IEnumerator PlayerDeath(Player p)
    {
        p.healthPanel.GetComponent<Image>().enabled = false;
        p.enabled = false;
        foreach (MeshRenderer mr in p.GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
        p.GetComponent<Collider>().enabled = false;

        Transform deathEffect = deathEffectsPool.GetChild(0);
        deathEffect.SetParent(null);
        deathEffect.position = p.transform.position;

        yield return new WaitForSeconds(1);

        deathEffect.SetParent(deathEffectsPool);
        Destroy(p.gameObject);
        Destroy(p.healthPanel.gameObject);

        MessagePanel.text = "ТЫ ПРОИГРАЛ!\n ёпта";
    }
}
