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

    public int currentStage = 1; // 현재 스테이지 번호
    public bool showFeedback = false; // 피드백 표시 여부

    [Header("# UI")]
    public Button gameStartButton;
    public TMP_Text stageText;
    // 클리어 팝업
    public GameObject stageClearPopUp;
    public TMP_Text stageClearText;
    public Button nextStageButton;
    // 실패 팝업
    public GameObject gameOverPopUp;
    public TMP_Text gameOverText;
    public Button retryButton;
    public TMP_Text feedbackText;

    private void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartButton);
        nextStageButton.onClick.AddListener(OnNextStageButton);
        retryButton.onClick.AddListener(OnRetryButton);
    }

    void OnGameStartButton()
    {
        currentStage = 1; // 게임 시작 시 스테이지 초기화

        ChessBoardManager.instance.SetChessBoard(1);
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
        gameStartButton.gameObject.SetActive(false); // 게임 시작 버튼 숨김
    }

    public void OnStageClear()
    {
        stageClearPopUp.SetActive(true);
        stageClearText.text = "Stage " + currentStage + " Clear!"; // 스테이지 클리어 텍스트 업데이트
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

    public void OnGameFailed()
    {
        gameOverPopUp.SetActive(true);
        gameOverText.text = "Stage " + currentStage + " Failed!"; // 게임 오버 텍스트 업데이트
        stageText.gameObject.SetActive(false); // 스테이지 텍스트 숨김

        if (showFeedback) feedbackText.gameObject.SetActive(true); // 피드백 텍스트 활성화
        else feedbackText.gameObject.SetActive(false); // 피드백 텍스트 비활성화

        // 피드백 텍스트 내용 설정
        feedbackText.text = "아아아아아아아아아";
    }

    void OnRetryButton()
    {
        gameOverPopUp.SetActive(false); // 게임 오버 팝업 숨김
        ChessBoardManager.instance.SetChessBoard(currentStage); // 현재 스테이지 재설정
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
    }
}
