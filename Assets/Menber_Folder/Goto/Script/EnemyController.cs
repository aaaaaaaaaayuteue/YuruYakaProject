using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("巡回ポイントの親オブジェクト（子オブジェクトを上から順に巡回する）")]
    [SerializeField] Transform patrolPointsParent;
    [Header("巡回時の移動速度")]
    [SerializeField] float patrolSpeed = 2f;
    [Header("追跡時の移動速度")]
    [SerializeField] float chaseSpeed = 4f;
    [Header("プレイヤーを検知する範囲")]
    [SerializeField] float detectionRange = 5f;
    [Header("追跡をやめてプレイヤーが範囲外になる距離")]
    [SerializeField] float loseRange = 8f;
    [Header("巡回ポイントに到着したと判定する距離")]
    [SerializeField] float arrivalDistance = 0.5f;
    [Header("プレイヤーのTransform")]
    [SerializeField] Transform playerTransform;
    [Header("プレイヤー発見時の方向転換にかかる時間（秒）")]
    [SerializeField] float lookAtPlayerDuration = 0.3f;
    [Header("プレイヤーの方向を向いてから追跡開始するまでの待機時間（秒）")]
    [SerializeField] float chaseStartDelay = 0.1f;
    [Header("プレイヤーを見失ってから巡回に戻るまでの時間（秒）")]
    [SerializeField] float losePlayerWaitTime = 3f;

    private NavMeshAgent agent;          // NavMeshAgentの参照
    private int currentPatrolIndex = 0;  // 現在の巡回ポイントのインデックス
    private bool isChasing = false;      // 追跡中かどうかのフラグ
    private Transform[] patrolPoints;    // 巡回ポイントの配列（Startで自動生成）

    private void Start()
    {
        // NavMeshAgentコンポーネントを取得
        agent = GetComponent<NavMeshAgent>();

        // 親オブジェクトの子オブジェクトを上から順に取得して配列に格納する
        patrolPoints = new Transform[patrolPointsParent.childCount];
        for (int i = 0; i < patrolPointsParent.childCount; i++)
        {
            patrolPoints[i] = patrolPointsParent.GetChild(i);
        }

        // 最初の巡回ポイントに向かう
        MoveToNextPatrolPoint();
    }

    private void Update()
    {
        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isChasing)
        {
            // ーーー追跡中の処理ーーー
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            // ーーー巡回中の処理ーーー
            Patrol(distanceToPlayer);
        }
    }

    // 巡回処理
    private void Patrol(float distanceToPlayer)
    {
        // 巡回速度を設定
        agent.speed = patrolSpeed;

        // プレイヤーが検知範囲内に入ったら追跡開始
        if (distanceToPlayer <= detectionRange)
        {
            // コルーチン内でisChasing = trueにするのでここではしない
            StartCoroutine(LookAtPlayerRoutine());
            return;
        }

        // 現在の目標ポイントに十分近づいたら次のポイントに移動
        if (agent.remainingDistance <= arrivalDistance && !agent.pathPending)
        {
            MoveToNextPatrolPoint();
        }
    }

    // lookAtPlayerDuration秒かけてプレイヤーの方向に向くコルーチン
    private IEnumerator LookAtPlayerRoutine()
    {
        float elapsed = 0f;

        // 回転中は敵の移動を止める
        agent.isStopped = true;

        // 回転開始時の向きを保存しておく
        Quaternion startRotation = transform.rotation;

        while (elapsed < lookAtPlayerDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lookAtPlayerDuration; // 0〜1の進行割合

            // プレイヤーの方向を計算（Y軸だけ回転させるためにY座標を自分と同じにする）
            Vector3 lookTarget = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

            // 開始時の向きからターゲットの向きへ球面補間で回転する
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        // 回転を最終値に確定する
        Vector3 finalLookTarget = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.rotation = Quaternion.LookRotation(finalLookTarget - transform.position);

        // chaseStartDelay秒待ってから追跡開始する
        yield return new WaitForSeconds(chaseStartDelay);

        // 追跡開始して敵の移動を再開する
        isChasing = true;
        agent.isStopped = false;
    }

    // 次の巡回ポイントに移動する処理
    private void MoveToNextPatrolPoint()
    {
        // 巡回ポイントが設定されていない場合は何もしない
        if (patrolPoints.Length == 0) return;

        // 次の巡回ポイントをNavMeshの目標に設定
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        // 次のインデックスに進む（最後まで行ったら最初に戻る）
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // 現在地から一番近い巡回ポイントを探してそこから巡回を再開する処理
    private void MoveToNearestPatrolPoint()
    {
        // 巡回ポイントが設定されていない場合は何もしない
        if (patrolPoints.Length == 0) return;

        int nearestIndex = 0;                    // 一番近いポイントのインデックス
        float nearestDistance = float.MaxValue;  // 一番近い距離（最初は最大値で初期化）

        // 全巡回ポイントとの距離を計算して一番近いものを探す
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);

            if (distance < nearestDistance)
            {
                // より近いポイントが見つかったら更新する
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        // 一番近いポイントに向かい、次はその次のポイントから巡回を続ける
        agent.SetDestination(patrolPoints[nearestIndex].position);
        currentPatrolIndex = (nearestIndex + 1) % patrolPoints.Length;
    }

    // 追跡処理
    private void ChasePlayer(float distanceToPlayer)
    {
        // 追跡速度を設定
        agent.speed = chaseSpeed;

        // プレイヤーの位置を目標に設定（毎フレーム更新）
        agent.SetDestination(playerTransform.position);

        // プレイヤーが追跡をやめる距離より遠くなったら巡回に戻る
        if (distanceToPlayer >= loseRange)
        {
            isChasing = false;

            // 追跡をやめたら一番近い巡回ポイントから巡回を再開する
            MoveToNearestPatrolPoint();
        }
    }

    // Sceneビューで検知範囲と追跡をやめる距離を可視化する
    private void OnDrawGizmosSelected()
    {
        // 検知範囲を黄色で表示
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 追跡をやめる距離を赤色で表示
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }

    // 外部からプレイヤーを見失わせる関数（LockerControllerから呼ぶ）
    public void LosePlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            StartCoroutine(LosePlayerRoutine());
        }
    }

    // プレイヤーを見失ってからlosePlayerWaitTime秒後に巡回に戻るコルーチン
    private IEnumerator LosePlayerRoutine()
    {
        // 敵をその場で止める
        agent.isStopped = true;

        // losePlayerWaitTime秒待つ
        yield return new WaitForSeconds(losePlayerWaitTime);

        // 移動を再開して巡回に戻る
        agent.isStopped = false;
        MoveToNearestPatrolPoint();
    }
}