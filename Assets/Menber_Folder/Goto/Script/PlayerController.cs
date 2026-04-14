using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] float moveSpeed = 5f;
    [Header("ダッシュ倍率（シフトキーで移動速度にこの値を掛ける）")]
    [SerializeField] float dashMultiplier = 1.3f;
    [Header("マウス感度（3D時）")]
    [SerializeField] float mouseSensitivity = 100f;
    [Header("上下の視点制限角度（90で真上・真下まで）")]
    [SerializeField] float verticalLookLimit = 90f;
    [Header("カメラのTransform（3D一人称視点）")]
    [SerializeField] Transform firstPersonCamera;
    [Header("俯瞰カメラのGameObject（2D時にONにする）")]
    [SerializeField] GameObject topDownCameraObject;

    private CharacterController controller;  // CharacterControllerの参照
    private float xRotation = 0f;            // 上下の回転角度を累積する変数
    private bool isTopDown = false;          // 現在の視点モード（falseで3D、trueで2D）

    private void Start()
    {
        // CharacterControllerコンポーネントを取得
        controller = GetComponent<CharacterController>();

        // 起動時は3Dモードで始める
        SetTopDownMode(false);
    }

    private void Update()
    {
        // スペースキーが押されたら視点を切り替える
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 現在の視点モードを反転して切り替える
            SetTopDownMode(!isTopDown);
        }

        if (isTopDown)
        {
            // ーーー2D俯瞰モードの移動処理ーーー
            TopDownMove();
        }
        else
        {
            // ーーー3D一人称モードの移動・視点処理ーーー
            FirstPersonMove();
            FirstPersonLook();
        }
    }

    // シフトキーを押しているかどうかで移動速度を計算して返す
    private float GetCurrentSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // シフトキーを押している間はダッシュ速度を返す
            return moveSpeed * dashMultiplier;
        }
        else
        {
            // 通常速度を返す
            return moveSpeed;
        }
    }

    // 3D一人称モードの移動処理
    private void FirstPersonMove()
    {
        // WASDキーの入力を取得（-1〜1の値）
        float x = Input.GetAxis("Horizontal"); // 左右
        float z = Input.GetAxis("Vertical");   // 前後

        // 自分の向きを基準に移動方向を計算
        Vector3 move = transform.right * x + transform.forward * z;

        // 現在の速度（ダッシュ中かどうかで変わる）をかけてキャラクターを動かす
        controller.Move(move * GetCurrentSpeed() * Time.deltaTime);
    }

    // 3D一人称モードの視点移動処理
    private void FirstPersonLook()
    {
        // マウスの移動量を取得し、感度と時間で補正
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // 左右
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // 上下

        // 上下の回転角度を更新
        xRotation -= mouseY;

        // verticalLookLimitの角度を超えて回らないように制限
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        // カメラの上下回転を適用
        firstPersonCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // プレイヤー本体をY軸で左右に回転
        transform.Rotate(Vector3.up * mouseX);
    }

    // 2D俯瞰モードの移動処理
    private void TopDownMove()
    {
        // WASDキーの入力を取得（-1〜1の値）
        float x = Input.GetAxis("Horizontal"); // 左右
        float z = Input.GetAxis("Vertical");   // 上下（ワールド座標のZ軸方向）

        // ワールド座標基準で左右・上下に移動（プレイヤーの向きに関係なく動く）
        Vector3 move = new Vector3(x, 0f, z);

        // 現在の速度（ダッシュ中かどうかで変わる）をかけてキャラクターを動かす
        controller.Move(move * GetCurrentSpeed() * Time.deltaTime);
    }

    // 外部から視点モードを切り替える関数
    // yRotation：3Dに戻る時のプレイヤーのY軸回転角度
    public void SetTopDownMode(bool topDown, float yRotation = 0f)
    {
        isTopDown = topDown;

        // 3Dカメラ・2Dカメラを切り替える
        firstPersonCamera.gameObject.SetActive(!topDown);
        topDownCameraObject.SetActive(topDown);

        if (topDown)
        {
            // 2Dモード切り替え時にプレイヤーのY軸回転を0に固定する
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            // 2Dモード時はカーソルを表示・解放する
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // 3Dに戻る時、指定された方向にプレイヤーを向かせる
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            // 上下の視点もリセットする
            xRotation = 0f;
            firstPersonCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);

            // 3Dモード時はカーソルを非表示・固定する
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}