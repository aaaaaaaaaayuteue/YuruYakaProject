using UnityEngine;
using CriWare;
using System.Collections;


public class Soude_Angel : MonoBehaviour
{
    
    [Header("CRI Source")]
    [SerializeField] private CriAtomSource SE_Source;
    [SerializeField] private CriAtomSource BGM_Source;

    [Header("クロスフェード（曲の入れ替え）の秒数設定")]
    [SerializeField] private float crossfadeDuration = 2.0f; // クロスフェードの秒数

    [Header("初期再生設定")]
    [SerializeField] private bool playOnStart = true; // ゲーム開始時に再生するかどうか
    [SerializeField] private bool startWithSparkling = true;// どっちの曲から始めるか　trueでキラキラ、falseで天使曲
    
    // クロスフェードのコルーチンを管理するための変数。これが null でない場合はクロスフェードが進行中であることを示す
    // Coroutineとはunityが返すコルーチンの実行状態を管理するための型。
    // これを使うことで、現在実行中のコルーチンを停止したり、状態を確認したりできる。
    private Coroutine _crossfadeCoroutine;

    // デバッグ用　trueでキラキラ音、falseで天使曲
    private bool sounde = true;
    
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (sounde)
            {
                sounde = false;
                CrossFadeToBgm();
                Debug.Log("BGMに切り替え");
            }
            else
            {
                sounde = true;
                CrossFadeToSparkling();
                Debug.Log("キラキラに切り替え");
            }
        }
    }
    
    
    void Awake()
    {
        // コンポーネントがあたっちされてなかったらエラーを出す
        if (SE_Source == null || BGM_Source == null)
        {
            Debug.LogError("CriAtomSource が見つかりません。インスペクターで SE_Source と BGM_Source を設定してください");
        }
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!playOnStart)
        {
            return;
        }

        if (startWithSparkling)
        {
            PlaySparklingInstant();
            
            // のちに消す
            sounde = true;
        }
        else
        {
            PlayBgmInstant();
            
            // のちに消す
            sounde = false;
        }
    }

   
    
    // 条件が付く前提で、外部から呼ぶ繰り替え口を用意しておく
    public void CrossFadeToBgm()
    {
        StartCrossFade(SE_Source, BGM_Source);
        
    }

    public void CrossFadeToSparkling()
    {
        StartCrossFade(BGM_Source, SE_Source);
        sounde = true;
    }
    
    // 先頭からそのまま鳴らしたいとき用
    // 初期化処理でどっちを先に鳴らすかで使う
    public void PlayBgmInstant()
    {
        SwitchInstantly(SE_Source, BGM_Source);
    }

    public void PlaySparklingInstant()
    {
        SwitchInstantly(BGM_Source, SE_Source);
    }
    
    
    
    // クロスフェードの処理を開始する関数。引数でどちらからどちらにクロスフェードするかを指定する
    // fromが今なっている音でtoが切り替える音
    private void StartCrossFade(CriAtomSource from, CriAtomSource to)
    {
        // nullチェック
        if (from == null || to == null)
        {
            Debug.LogError("クロスフェードに必要な CriAtomSource が足りません");
            return;
        }

        // クロスフェードのコルーチンがすでに動いている場合は停止してから新しいコルーチンを開始する
        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
        }

        // クロスフェードのコルーチンを開始する
        _crossfadeCoroutine = StartCoroutine(CrossFadeRoutine(from, to));
    }
    
    // クロスフェード処理のメインの部分を担当するコルーチン。fromからtoへのクロスフェードを実行する
    private IEnumerator CrossFadeRoutine(CriAtomSource from, CriAtomSource to)
    {
        // フェード時間が0以下（つまり指定した時間を超えた時）には、即座に切り替える
        if (crossfadeDuration <= 0f)
        {
            //  nullチェックをしてから、fromを止めてtoを再生する
            if (from != null)
            {
                from.Stop();
            }

            to.volume = 1f;     // 音量の設定
            to.Play();          // 再生の開始
            _crossfadeCoroutine = null;     // コルーチンの状態をリセット
            yield break;        // コルーチンを終了する　yield breakはコルーチンの実行を途中で終了させるための命令。これ以降のコードは実行されない
        }

        // クロスフェードの処理を行う。（これは変数の初期化）
        // elapsedは経過時間を追跡するための変数。
        // fromStartVolumeとtoStartVolumeは、クロスフェード開始時のそれぞれの音量を保存するための変数。
        float elapsed = 0f;

        // fromStartVolume: クロスフェード開始時の from の音量を保存
        float fromStartVolume;
        if (from != null)
        {
            // Mathf.Clamp01(from.volume)は、from.volumeの値を0から1の範囲に制限するための関数。
            // これにより、音量が0未満や1を超えることがないようにする。
            fromStartVolume = Mathf.Clamp01(from.volume);
        }
        else
        {
            fromStartVolume = 0f;
        }

        // toStartVolume: クロスフェード開始時の to の音量を保存
        float toStartVolume;
        if (to != null)
        {
            toStartVolume = Mathf.Clamp01(to.volume);
        }
        else
        {
            toStartVolume = 0f;
        }

        // toの音を再生
        if (to != null)
        {
            // to（これから鳴らす音）が現在鳴っていない、もしくは止まる寸前（ボリュームが0）なら、
            // 新しく再生ボタンを押す。
            if (to.status != CriAtomSource.Status.Playing || toStartVolume <= 0.01f)
            {
                to.volume = toStartVolume; // 0からではなく、現在の音量をセット
                to.Play();
            }
        }

        
        // 設定した秒数までクロスフェードの処理を続ける
        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;// 経過時間の蓄積
            float progress = Mathf.Clamp01(elapsed / crossfadeDuration);// 進捗度の正規化（0から1の範囲に制限）

            if (from != null)
            {
                // 進捗度に基づいて from の音量を線形補間する。progressが0のときはfromStartVolume、progressが1のときは0になるようにする
                from.volume = Mathf.Lerp(fromStartVolume, 0f, progress);
            }

            if (to != null)
            {
                // 進捗度に基づいて to の音量を線形補間する。progressが0のときはtoStartVolume、progressが1のときは1になるようにする
                to.volume = Mathf.Lerp(toStartVolume, 1f, progress);
            }

            yield return null;// 次のフレームまで待機して、そこから処理を続ける
        }

        // フェード処理が終わったらfromの音量を0にしてとめる
        if (from != null)
        {
            from.volume = 0f;
            from.Stop();
        }

        // toの音量を1にして再生を確実にする
        if (to != null)
        {
            to.volume = 1f;
        }

        _crossfadeCoroutine = null;
    }
    
    // フェードなしの曲の切り替え処理
    private void SwitchInstantly(CriAtomSource from, CriAtomSource to)
    {
        // nullチェック
        if (from == null || to == null)
        {
            Debug.LogError("切り替えに必要な CriAtomSource が足りません");
            return;
        }

        // クロスフェードのコルーチンがすでに動いている場合は停止する
        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
            _crossfadeCoroutine = null;
        }

        // fromをストップ
        from.Stop();
        from.volume = 0f;

        // toを再生（二重再生対策で一度stopをしている）
        to.Stop();
        to.volume = 1f;
        to.Play();
    }
    
    // スクリプトが無効になったときに、もしクロスフェードのコルーチンが動いていたら停止する。
    // これをしないと、スクリプトが無効になっている間もクロスフェードの処理が続いてしまう可能性がある
    private void OnDisable()
    {
        if (_crossfadeCoroutine != null)
        {
            StopCoroutine(_crossfadeCoroutine);
            _crossfadeCoroutine = null;
        }
    }
    
}
