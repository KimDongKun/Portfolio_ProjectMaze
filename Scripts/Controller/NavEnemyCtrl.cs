using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavEnemyCtrl : NetworkBehaviour
{
    private NavMeshAgent navAgent;

    // 애니메이션
    public Animator animator;

    // 적이 추적할 대상(플레이어)
    public Transform targetPlayer;

    // 추적 범위
    [SerializeField]
    private float searchRange = 20f;

    // 주기적으로 플레이어를 탐색할 간격
    private float searchInterval = 2f;
    private float searchTimer = 0f;

    // 서버에서만 AI 로직을 처리하도록 설정
    public override void OnStartServer()
    {
        base.OnStartServer();
        navAgent = GetComponent<NavMeshAgent>();
    }

    [ServerCallback]
    void Update()
    {
        // 서버에서만 동작하도록 보장
        if (!isServer) return;

        // 일정 시간 간격으로 플레이어를 찾는다
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            FindNearestPlayer();
        }

        // 타겟 플레이어가 있으면 추적
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
    /// 가장 가까운 플레이어를 찾는다
    /// </summary>
    void FindNearestPlayer()
    {
        // 현재 씬에 존재하는 NetworkIdentity 중에서 Player 컨트롤러를 가진 오브젝트만 골라온다
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
                Debug.Log(nearest.name + " - 추적");

            }
            else 
            { 
                navAgent.ResetPath(); 
            }
        }
        
        
    }
}
