using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private int currentStage = 1; // 현재 스테이지 번호

    [Header("# UI")]
    public Button gameStartButton;
    public TMP_Text stageText;
    public GameObject stageClearPopUp;
    public Button nextStageButton;

    private void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartButton);
        nextStageButton.onClick.AddListener(OnNextStageButton);
    }

    void OnGameStartButton()
    {
        ChessBoardManager.instance.SetChessBoard(currentStage);
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
        gameStartButton.gameObject.SetActive(false); // 게임 시작 버튼 숨김
    }

    public void OnStageClear()
    {
        stageClearPopUp.SetActive(true);
        currentStage++; // 스테이지 증가
        stageText.gameObject.SetActive(false);
    }

    void OnNextStageButton()
    {
        stageClearPopUp.SetActive(false); // 스테이지 클리어 팝업 숨김
        ChessBoardManager.instance.SetChessBoard(currentStage); // 다음 스테이지 설정
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
    }
}
