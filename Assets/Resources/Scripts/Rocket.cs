﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    public int damage; // урон
    public float maxRange; // предельная дистанция полета
    public float speed; // скорость полета 
    public Vector3 startPoint; // начальная точка полета    
    public string MyShooterTag; // тэг стреляющего НПС
    public bool flying; // летит ли?
    public Vector3 direction; // направление полета

    public Rigidbody rb;
    public Main main;

    void Update()
    {
        if (flying)
        {
            // прожектайлы игрока не замедляются во время матрицы
            if (MyShooterTag == "Player") transform.position += direction.normalized * speed * Time.deltaTime;
            else transform.position += direction.normalized * speed * Time.deltaTime * main.curSlowerCoeff;

            // если прожектайл летит и достиг максимальной длины полета - возвращаем в пул
            if ((startPoint - transform.position).magnitude >= maxRange)
            {
                transform.SetParent(main.rocketsPool);
                flying = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // если прожектайл столкнулся с препятствием - возвращаем в пул
        if (other.gameObject.layer == 9)
        {
            transform.SetParent(main.rocketsPool);
            flying = false;
        }
        // если прожектайл столкнулся с НПС - наносим урон - возвращаем в пул
        else if (other.gameObject.layer == 10)
        {
            if (other.tag != MyShooterTag) // исключаем самопоражение и фрэндли файр
            {
                if (other.tag == "Enemy")
                {
                    Enemy e = other.GetComponent<Enemy>();
                    main.BodyHitReaction(e.mr, e.MPB, e.bodyColor);

                    e.curHealthPoint -= damage;
                    e.healthPanelScript.HitFunction(e.curHealthPoint / e.maxHealthPoint, damage);

                    if (e.curHealthPoint <= 0)
                    {
                        main.EnemyDie(e);
                    }
                }

                else if (other.tag == "Player")
                {
                    Player p = other.GetComponent<Player>();
                    main.BodyHitReaction(p.mr, p.MPB, p.bodyColor);

                    p.curHealthPoint -= damage;
                    p.healthPanelScript.HitFunction(p.curHealthPoint / p.maxHealthPoint, damage);

                    if (p.curHealthPoint <= 0)
                    {
                        main.PlayerDie(p);
                    }
                }

                flying = false;
                transform.SetParent(main.rocketsPool);
            }
        }
    }
}
