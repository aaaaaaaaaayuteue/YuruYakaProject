using UnityEngine;
using CriWare;

public class occlusion : MonoBehaviour
{
    [Header("ソースのセット")]
    [SerializeField] private CriAtomSource atomSource;        // Inspector で割当て
    [Header("リスナー（プレイヤーorカメラ）のセット")]
    [SerializeField] private Transform listener;              // リスナー Transform（Camera.main など）
    [Header("レイヤーのセット（壁や床のレイヤーを選択）")]
    [SerializeField] private LayerMask occlusionLayer;        // 壁に割当てたレイヤー
    [Header("遮蔽を感じる際の最小音量の設定（1~0）")]
    [SerializeField] private float minOcclusion = 0.4f;     // 遮蔽が完全なときの最小音量（0..1）
    
    private float checkInterval = 0.1f;      // レイキャスト実行間隔（秒）
     private int sampleRays = 5;              // サンプル本数（奇数：中心+周り）
     private float sampleSpread = 0.2f;       // 周辺サンプルのオフセット距離（m）
     private float smoothTime = 0.1f;         // スムージングの時間
     

    
    float currentOcclusion = 0f;    // 現在の遮蔽割合（0..1）
    float occlusionVel = 0f;        // スムージング用の速度変数
    float lastCheckTime = 0f;       // 最後に遮蔽をチェックした時間

    void Update()
    {
        // ヌルチェック
        if (atomSource == null || listener == null) return;

        // checkInterval（実行間隔）ごとに遮蔽を計算している
        if (Time.time - lastCheckTime >= checkInterval)
        {
            lastCheckTime = Time.time;
            // 遮蔽割合を求める関数で出した割合をocclに入れる
            float occl = Occlusion_ratio();
            // スムージング（滑らかに現在値へ近づける）
            currentOcclusion = Mathf.SmoothDamp(currentOcclusion, occl, ref occlusionVel, smoothTime, Mathf.Infinity, checkInterval);
            // 0~1に収める
            currentOcclusion = Mathf.Clamp01(currentOcclusion);
            

            // 遮蔽割合に応じて音量も下げる
            // currentOcclusion = 0 → 音量 1.0
            // currentOcclusion = 1 → 音量 minOcclusion
            float targetVolume = Mathf.Lerp(1.0f, minOcclusion, currentOcclusion);  // 一度この変数で値を取得してから音量を変化する
            atomSource.volume = Mathf.Clamp01(targetVolume);
            
            // デバッグ 遮蔽割合と音量をだす
            Debug.Log($"Occlusion: {currentOcclusion}, Volume: {atomSource.volume}");
        }
    }

    // 遮蔽割合を求める関数
    // 仕組みはレイという見えない線を音源からリスナーに向けて飛ばして、壁などのオブジェクトに当たるかを調べる
    float Occlusion_ratio()
    {
        // 音源オブジェクトとリスナー（プレイヤーorカメラ）の位置を取る
        Vector3 srcPos = transform.position;        // 音源オブジェクトのポジションの取得
        Vector3 listenerPos = listener.position;    // リスナーのポジションの取得
        Vector3 dir = (listenerPos - srcPos);       // 音源からリスナーへの方向の取得
        float dist = dir.magnitude;                 // 音源からリスナーへの距離の取得
        if (dist <= 0.0001f) return 0f; // 距離がほぼ0なら計算をせず0（遮蔽なし）を返す

        // 中心と 4 方向にレイを飛ばす (上/下/left/right offset on source)
        int blocked = 0;
        int total = sampleRays;

        // 中心にレイを飛ばし、あたればblokedの値を増やす
        if (Physics.Raycast(srcPos, dir.normalized, dist, occlusionLayer))
        {
            blocked++;
        }
            

        if (sampleRays > 1)
        {
            // 周辺サンプル（スクリーン上での分布やソースのサイズに応じて調整）
            Vector3 right = transform.right * sampleSpread;
            Vector3 up = transform.up * sampleSpread;

            // 音源の少し横や上下にもずらして、複数のレイを飛ばす
            Vector3[] offsets = {
                right,
                -right,
                up,
                -up
            };

            int extraSamples = Mathf.Min(sampleRays - 1, offsets.Length);
            for (int i = 0; i < extraSamples; i++)
            {
                Vector3 start = srcPos + offsets[i];
                if (Physics.Raycast(start, (listenerPos - start).normalized, dist, occlusionLayer))
                {
                    blocked++;
                }
                    
            }
        }

        // 遮蔽割合
        float ratio = (float)blocked / (float)total;
        // 任意に曲線を入れる（例：小さな遮蔽でも強めに効かせたい場合）
        // ratio = Mathf.Pow(ratio, 1.0f);

        return ratio;
    }
}
