using UnityEngine;
using CriWare;

public class Soude_Burner : MonoBehaviour
{
    private CriAtomSource atomSource;

    // 後述にもあるのだがCRIのツールアトムクラフトにはブロックという再生部分を分けているものがある。
    // 今回はガスバーナーの着火から燃焼中の音と賞かの音を分けて再生するためにそれを利用する。
    // インスペクターで設定できるようにしているのはこのブロックの名前やキュー（音）の名前を入れる必要があるため
    // コードではなく、インスペクターで編集できるようにするため（名前がわからない場合は聞いて）
    [Header("CRI Block Settings")] [SerializeField]
    private string cueSheetName = "SE";
    [SerializeField] private string cueName = "burner";
    [SerializeField] private string startBlockName = "ignition";
    [SerializeField] private string stopBlockName = "digestion";

    private CriAtomExAcb acb; // キューシートからACB（拡張子がacb）を取得するための変数
    private CriAtomExPlayback playback; // 再生中の音を制御するための変数。再生した後にこの変数に再生情報が入るので、これを使って次のブロックに切り替えるなどの処理をする
    private bool isBurnerPlaying = false; // 追加：再生状態フラグ


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // コンポーネントの獲得　コンポーネントが取れなかった際のエラーチェック
        atomSource = GetComponent<CriAtomSource>();
        if (atomSource == null)
        {
            Debug.LogError("CriAtomSource が見つかりません");
        }

        // ACBの獲得　キューシートの名前を引数にしてACBを取得する。もし見つからない場合はnullが返ってくるのでエラーチェックをする
        // acbはキューシートの拡張子
        acb = CriAtom.GetAcb(cueSheetName);
        if (acb == null)
        {
            Debug.LogError("ACB が見つかりません: " + cueSheetName);
        }
        
        // サウンドの再生フラグ　trueで再生中、falseで停止中
        isBurnerPlaying = false;
        

    }

    // Update is called once per frame
    void Update()
    {
        // デバッグ用で操作で音の出したりできるようにする
        // 統合作業時には削除必須

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BurnerSounde_Start();
            Debug.Log("BurnerSounde_Start");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
           BurnerSounde_Stop();
           Debug.Log("BurnerSounde_Stop");
        }
    }

    // 以下にサウンドの処理を関数にして書いておく。統合作業の際にはこの関数を張り付けれるだけで済ませれるようにする
    // もし、ほかに必要なことがあればその都度コメントしておく

    // ガスバーナーの着火から燃焼中の音の再生関数
    void BurnerSounde_Start()
    {
        // すでに再生中なら何もしない
        if (isBurnerPlaying)
        {
            Debug.Log("再生中なので開始処理はスキップ");
            return;
        }

        // エラーチェック
        if (atomSource == null || acb == null)
        {
            Debug.LogError("サウンドの再生に必要なコンポーネントが見つかりません");
            return;
        }

        // CRIのツールアトムクラフトにはブロックという再生部分を分けている第一引数には音の名前、第二引数はそのブロックの名前を書く
        // （もしわからなかったら聞いて）
        int startBlockIndex = acb.GetBlockIndex(cueName, startBlockName);

        // エラーチェック　ブロックが見つからない場合は0xFFFFFFFFが返ってくるので、それと比較してエラー処理をする
        if (startBlockIndex == unchecked((int)0xFFFFFFFF))
        {
            Debug.LogError("開始ブロックが見つかりません: " + startBlockName);
            return;
        }


        atomSource.player.SetFirstBlockIndex(startBlockIndex); // どのブロックから始まるのかの指定（変更したい場合はインスペクターから）
        playback = atomSource.Play(cueName); // 指定されたキューの音を再生する。再生した後にこの変数に再生情報が入るので、これを使って次のブロックに切り替えるなどの処理をする
        
        // 再生に成功したかどうかを判定してフラグを更新
        // playback.id != CriAtomExPlayback.invalidIdで再生が成功しているかどうかを判定できる。
        // 再生に失敗した場合はinvalidIdが返ってくるので、それと比較して成功していればフラグをtrueにする
        if (playback.id != CriAtomExPlayback.invalidId)
        {
            isBurnerPlaying = true;
        }
        else
        {
            Debug.LogWarning("Play() は呼ばれたが再生インスタンスが取得できませんでした");
            isBurnerPlaying = false;
        }
    }

    // バーナーの消化音に切り替える関数（即座に切り替え）
    void BurnerSounde_Stop()
    {
        // すでに止まっているなら何もしない
        if (!isBurnerPlaying)
        {
            Debug.Log("既に停止状態なので停止処理をスキップ");
            return;
        }

        // エラーチェック
        if (atomSource == null || acb == null)
        {
            Debug.LogError("サウンドの停止に必要なコンポーネントが見つかりません");
            return;
        }

        // 消化ブロックのインデックスを取得
        int stopBlockIndex = acb.GetBlockIndex(cueName, stopBlockName);

        // エラーチェック　ブロックが見つからない場合は0xFFFFFFFFが返ってくるので、それと比較してエラー処理をする
        if (stopBlockIndex == unchecked((int)0xFFFFFFFF))
        {
            Debug.LogError("終了ブロックが見つかりません: " + stopBlockName);
            isBurnerPlaying = false;
            return;
        }

        // 今の再生を止める
        atomSource.Stop();
        
        // 消化ブロックから再生し直す（即座に切り替わる）
        atomSource.player.SetFirstBlockIndex(stopBlockIndex);
        playback = atomSource.Play(cueName);
        
        // 再生中かの確認　こっちは再生終了後にフラグをfalseにする必要があるのでStart関数とは少し違う書き方をしている
        if (playback.id == CriAtomExPlayback.invalidId)
        {
            Debug.LogWarning("消化音の再生に失敗しました");
        }
        
        // 判定のリセット
        isBurnerPlaying = false;

        
    }
}