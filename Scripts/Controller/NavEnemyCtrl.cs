using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavEnemyCtrl : NetworkBehaviour
{
    private NavMeshAgent navAgent;

    // �ִϸ��̼�
    public Animator animator;

    // ���� ������ ���(�÷��̾�)
    public Transform targetPlayer;

    // ���� ����
    [SerializeField]
    private float searchRange = 20f;

    // �ֱ������� �÷��̾ Ž���� ����
    private float searchInterval = 2f;
    private float searchTimer = 0f;

    // ���������� AI ������ ó���ϵ��� ����
    public override void OnStartServer()
    {
        base.OnStartServer();
        navAgent = GetComponent<NavMeshAgent>();
    }

    [ServerCallback]
    void Update()
    {
        // ���������� �����ϵ��� ����
        if (!isServer) return;

        // ���� �ð� �������� �÷��̾ ã�´�
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            FindNearestPlayer();
        }

        // Ÿ�� �÷��̾ ������ ����
        if (targetPlayer != null)
        {
            navAgent.SetDestination(targetPlayer.position);

            float atkDis = Vector3.Distance(targetPlayer.position, navAgent.transform.position);
            if (atkDis < 1.8f)
            {
                Animation_ATK(true);
            }
            else
            {
                Animation_MOVE(true);
            }
        }
        else
        {
           
            Animation_IDLE(true);
        }
    }

    [Server]
    void Animation_ATK(bool isATK)
    {
        animator.SetBool("ATTACK", isATK);
        animator.SetBool("IDLE", false);
        animator.SetBool("MOVE", false);
        animator.SetBool("DIE", false);
    }

    [Server]
    void Animation_MOVE(bool isMove)
    {
        animator.SetBool("ATTACK", false);
        animator.SetBool("IDLE", false);
        animator.SetBool("MOVE", isMove);
        animator.SetBool("DIE", false);
    }

    [Server]
    void Animation_IDLE(bool isIdle)
    {
        animator.SetBool("ATTACK", false);
        animator.SetBool("IDLE", isIdle);
        animator.SetBool("MOVE", false);
        animator.SetBool("DIE", false);
    }


    /// <summary>
    /// ���� ����� �÷��̾ ã�´�
    /// </summary>
    void FindNearestPlayer()
    {
        // ���� ���� �����ϴ� NetworkIdentity �߿��� Player ��Ʈ�ѷ��� ���� ������Ʈ�� ���´�
        var players = FindObjectsByType<PlayerMovementController>(0)
            .Select(x => x.transform)
            .ToList();

        float minDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < minDistance && distance <= searchRange)
            {
                minDistance = distance;
                nearest = player;

                targetPlayer = nearest;
                //float atkDis = Vector3.Distance(targetPlayer.position, navAgent.transform.position);
                Debug.Log(nearest.name + " - ����");

            }
            else 
            { 
                navAgent.ResetPath(); 
            }
        }
        
        
    }
}
