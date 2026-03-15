using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(DownedAnimal))]
public class FaunaBehavior : Attackable
{
    public static UnityEvent<bool> spookEvent = new UnityEvent<bool>();

    public UnityEvent onDeath = new UnityEvent();

    public string faunaName;

    public AudioClip harvestCarcassSound;
    public Container skinSack;

    public float hearRadius;
    public float hearLoudRadius;

    public float stoppingDistance = 1;
    public float heightOffset;

    public float roamingSpeed;
    public float fleeingSpeed;

    [Header("Pack Behavior")]
    public FaunaBehavior following;
    public float followDistance;

    [Header("Hunting")]
    public float health;
    public List<ItemQuantity> harvestItems = new List<ItemQuantity>();

    [Header("Roaming")]
    public Vector2 roamTimeInterval;
    public List<Transform> roamLocations = new List<Transform>();

    [Header("Sight")]
    public float sightDistance;

    [Header("Fleeing")]
    public float fleeThreshold;

    public float fleeCooldown;

    Vector3 currentTarget;
    float roamTimer;

    GameObject lastThreat;
    public float fleeTimer { get; private set; }

    private void Awake()
    {
        spookEvent.AddListener(ListenForThreat);
    }

    private void Start()
    {
        OnHit(0);
        if (following) following.onDeath.AddListener(() => { following = null; });
    }

    private void Update()
    {
        if (health <= 0)
            return;

        LookForThreat();

        fleeTimer -= Time.deltaTime;

        if(fleeTimer <= 0)
        {
            lastThreat = null;
            if (!following) Roam();
        }
        if(following && !lastThreat) Follow();
        if (lastThreat)
            FleeFrom(lastThreat.transform, true);
    }

    void LookForThreat()
    {
        Collider[] player = Physics.OverlapSphere(transform.position, sightDistance, LayerMask.GetMask("Player"));
        if (player.Length > 0)
        {
            Ray ray = new Ray(transform.position, player[0].transform.position - transform.position);
            Physics.Raycast(ray, out RaycastHit hit, sightDistance);


            //Is player behind an obstacle
            if (hit.transform && hit.transform == player[0].transform
                /*&& Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0*/)
            {
                //Player was seen
                //Run away
                if (Vector3.Distance(transform.position, player[0].transform.position) <= fleeThreshold)
                {
                    SetThreat(player[0].gameObject);
                }
                //Walk away
                else
                {
                    fleeTimer = fleeCooldown;
                    FleeFrom(player[0].transform, lastThreat);
                }
            }
        }
    }

    void Roam()
    {
        roamTimer -= Time.deltaTime;

        if (roamTimer <= 0)
        {
            SetRandomTarget();
        }

        MoveTo(currentTarget, false);
    }
    void Follow()
    {
        MoveTo(following.transform.position, false);
    }

    void SetRandomTarget()
    {
        currentTarget = roamLocations[Random.Range(0, roamLocations.Count)].position;
        roamTimer = Random.Range(roamTimeInterval.x, roamTimeInterval.y);
    }

    void MoveTo(Vector3 target, bool fleeing)
    {
        if ((!following && Vector3.Distance(transform.position, target) <= stoppingDistance) ||
            (following && Vector3.Distance(transform.position, following.transform.position) <= followDistance))
        {
            if(fleeTimer <= 0) IdleState();
        }
        else
        {
            float speed = roamingSpeed;
            if (fleeing)
            {
                FleeState();
                speed = fleeingSpeed;
            }
            else
                RoamState();

            Vector3 nextPos = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            transform.position = new Vector3(nextPos.x, Terrain.activeTerrain.SampleHeight(nextPos) + heightOffset, nextPos.z);

            transform.LookAt(target);
        }
    }


    async void ListenForThreat(bool loud)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, loud ? hearLoudRadius : hearRadius, LayerMask.GetMask("Player"));
        if(cols.Length > 0)
        {
            await Task.Delay(250);
            SetThreat(cols[0].gameObject);
            return;
        }
    }

    public void SetThreat(GameObject threat)
    {
        fleeTimer = fleeCooldown;
        lastThreat = threat;
        SetRandomTarget();
    }

    public void FleeFrom(Transform target, bool spooked)
    {
        if (!following)
        {
            Vector3 direction = (stoppingDistance * 2) * (target.position - transform.position);
            MoveTo(transform.position - direction, spooked);
        }
        else
        {
            if(spooked && following.fleeTimer <= 0) following.SetThreat(target.gameObject);
            MoveTo(following.transform.position, spooked);
        }
    }
    public virtual void FleeState()
    {

    }
    public virtual void RoamState()
    {

    }
    public virtual void IdleState()
    {

    }
    public virtual void LookAnimate(Vector3 target)
    {

    }
    public virtual void AnimateDeath() { }

    public override void OnHit(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            onDeath.Invoke();

            gameObject.GetComponent<DownedAnimal>().Initiate(this);

            //Vector3 pos = transform.position;

            AnimateDeath();

            //transform.position = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, hearRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeThreshold);
    }
}
