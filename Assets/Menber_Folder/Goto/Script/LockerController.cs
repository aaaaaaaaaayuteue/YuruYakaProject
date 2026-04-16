using UnityEngine;
using UnityEngine.UI;

public class LockerController : MonoBehaviour
{
    [Header("プレイヤーのTransform")]
    [SerializeField] Transform playerTransform;
    [Header("PlayerControllerの参照")]
    [SerializeField] PlayerController playerController;
    [Header("EnemyControllerの参照")]
    [SerializeField] EnemyController enemyController;
    [Header("ロッカーに入れる範囲")]
    [SerializeField] float interactRange = 2f;
    [Header("3D時の視界角度（この角度内にロッカーがあれば表示）")]
    [SerializeField] float fieldOfViewAngle = 60f;
    [Header("2D時にプレイヤーがロッカーの方向を向いてると判定する角度")]
    [SerializeField] float facingAngleThreshold = 60f;
    [Header("ロッカーから出た時の正面からの距離")]
    [SerializeField] float exitDistance = 1.5f;
    [Header("E : 隠れる / E : 出るのテキスト")]
    [SerializeField] Text promptText;
    [Header("画面を暗くする黒いパネル")]
    [SerializeField] Image blackPanel;
    [Header("3D時に使うカメラのTransform")]
    [SerializeField] Transform firstPersonCamera;

    private bool isPlayerInside = false;  // プレイヤーがロッカー内にいるかどうか

    private void Update()
    {
        // プレイヤーとロッカーの距離を計算
        float distanceToLocker = Vector3.Distance(playerTransform.position, transform.position);

        if (isPlayerInside)
        {
            // ーーーロッカー内にいる時の処理ーーー

            // Eキーを押したらロッカーから出る
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitLocker();
            }
        }
        else
        {
            // ーーーロッカー外にいる時の処理ーーー

            // 範囲内かつ条件を満たす時にUIを表示してEキーを受け付ける
            if (distanceToLocker <= interactRange && CanInteract())
            {
                // E : 隠れるを表示
                promptText.gameObject.SetActive(true);
                promptText.text = "E : 隠れる";

                // Eキーを押したらロッカーに入る
                if (Input.GetKeyDown(KeyCode.E))
                {
                    EnterLocker();
                }
            }
            else
            {
                // 条件を満たさない時はUIを非表示
                promptText.gameObject.SetActive(false);
            }
        }
    }

    // 3D・2Dそれぞれの条件を満たしてるか判定する
    private bool CanInteract()
    {
        if (playerController.IsTopDown)
        {
            // ーーー2D時の判定ーーー
            // プレイヤーからロッカーへの方向を計算
            Vector3 dirToLocker = (transform.position - playerTransform.position).normalized;

            // プレイヤーの向きとロッカーへの方向の角度を計算
            float angle = Vector3.Angle(playerTransform.forward, dirToLocker);

            // facingAngleThreshold以内の角度ならロッカーの方を向いてると判定
            return angle <= facingAngleThreshold;
        }
        else
        {
            // ーーー3D時の判定ーーー
            // カメラからロッカーへの方向を計算
            Vector3 dirToLocker = (transform.position - firstPersonCamera.position).normalized;

            // カメラの向きとロッカーへの方向の角度を計算
            float angle = Vector3.Angle(firstPersonCamera.forward, dirToLocker);

            // fieldOfViewAngle以内の角度ならロッカーが視界内にあると判定
            return angle <= fieldOfViewAngle / 2f;
        }
    }

    // ロッカーに入る処理
    private void EnterLocker()
    {
        // ロッカーの中心座標にプレイヤーを移動させる
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        cc.enabled = false;
        playerTransform.position = transform.position;
        cc.enabled = true;

        isPlayerInside = true;

        // 画面を瞬時に真っ暗にする
        blackPanel.gameObject.SetActive(true);

        // E : 出るに変更
        promptText.text = "E : 出る";

        // PlayerControllerに隠れた状態を伝える
        playerController.SetHiding(true);

        // 敵にプレイヤーを見失わせる
        enemyController.LosePlayer();
    }

    // ロッカーから出る処理
    private void ExitLocker()
    {
        // ロッカーの正面方向にexitDistance分だけ離れた座標を計算する
        Vector3 exitPosition = transform.position + transform.forward * exitDistance;

        // CharacterControllerを一時的に無効化して座標を変更する
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        cc.enabled = false;
        playerTransform.position = exitPosition;
        cc.enabled = true;

        isPlayerInside = false;

        // 画面の黒いパネルを消す
        blackPanel.gameObject.SetActive(false);

        // UIを非表示
        promptText.gameObject.SetActive(false);

        // PlayerControllerに出た状態を伝える
        playerController.SetHiding(false);
    }

    // Sceneビューでインタラクト範囲を可視化する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}