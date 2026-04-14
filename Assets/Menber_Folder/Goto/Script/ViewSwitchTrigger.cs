using UnityEngine;

public class ViewSwitchTrigger : MonoBehaviour
{
    // プレイヤーが範囲内に入ったときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.SetTopDownMode(true);
        }
    }

    // プレイヤーが範囲から出たときに呼ばれる
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // トリガーの中心からプレイヤーの出た方向を計算
                Vector3 exitDir = other.transform.position - transform.position;

                float yRotation = 0f;

                // XとZのどちらの成分が大きいかで出た方向を判定
                if (Mathf.Abs(exitDir.x) > Mathf.Abs(exitDir.z))
                {
                    // 左右から出た場合
                    if (exitDir.x > 0f)
                    {
                        // 右から出た → 右を向く
                        yRotation = 0f;
                    }
                    else
                    {
                        // 左から出た → 左を向く
                        yRotation = 0f;
                    }
                }
                else
                {
                    // 上下から出た場合
                    if (exitDir.z > 0f)
                    {
                        // 上から出た → 正面を向く
                        yRotation = 0f;
                    }
                    else
                    {
                        // 下から出た → 真後ろを向く
                        yRotation = 0f;
                    }
                }

                player.SetTopDownMode(false, yRotation);
            }
        }
    }
}