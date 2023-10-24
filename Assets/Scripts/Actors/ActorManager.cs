using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    private List<Actor> actorList;
    private List<Actor> allyList;
    private List<Actor> enemyList;

    public static ActorManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There's more than one Actor Manager!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        actorList = new List<Actor>();
        allyList = new List<Actor>();
        enemyList = new List<Actor>();
    }
    private void Start()
    {
        Actor.OnAnyActorSpawn += Actor_OnAnyActorSpawn;
        Actor.OnAnyActorDestroyed += Actor_OnAnyActorDestroyed;
    }

    private void Actor_OnAnyActorSpawn(object sender, System.EventArgs e)
    {
        Actor actor = sender as Actor;
        if (actor.IsEnemy())
        {
            enemyList.Add(actor);
        } else
        {
            allyList.Add(actor);
        }
        actorList.Add(actor);
        Debug.Log(actor.name + " spawned.");
    }
    private void Actor_OnAnyActorDestroyed(object sender, System.EventArgs e)
    {
        Actor actor = sender as Actor;
        if (actor.IsEnemy())
        {
            enemyList.Remove(actor);
        }
        else
        {
            allyList.Remove(actor);
        }

        actorList.Remove(actor);
        Debug.Log(actor.name + " died.");
    }

    public List<Actor> GetActorsList() { return actorList; }
    public List<Actor> GetEnemyList() { return enemyList; }
    public List<Actor> GetAlliesList() { return allyList; }
}
