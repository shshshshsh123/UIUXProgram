using NUnit.Framework;
using System.Collections.Generic;
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
    public int playerFailedCount = 0; // 플레이어 실패 횟수 (스테이지당)
    List<List<string>> stage4Hints = new List<List<string>>
    {
        new List<string>
        {
            "상대 킹의 숨을 막을 방법을 고민해보세요!",
            "다른기물보다 폰을 먼저 움직여보세요!",
            "(7, 5) 폰을 이용하여 체크를 걸어보세요!"
        },
        new List<string>
        {
            "당신이 다시 리드할 기회입니다!",
            "다시 한번 더 킹의 숨을 막을 방법을 고민해보세요!",
            "폰을 이용하여 다시 체크를 걸어보세요!"
        },
        new List<string>
        {
            "다시 상대를 체크하도록 시도해보세요!",
            "룩의 직선이동을 활용해보세요!",
            "룩을 이용하여 상대에게 체크를 걸어보세요!"
        },
        new List<string>
        {
            "마지막 마무리를 할 차례입니다!",
            "상대의 방해를 제거하면서 체크해보세요!",
            "룩을 이용하여 상대방의 기물을 제거하세요!"
        }
    };

    List<List<string>> stage5Hints = new List<List<string>>
    {
        new List<string>
        {
            "상대 킹의 숨을 막을 방법을 고민해보세요!",
            "상대의 기물을 줄이면서 체크를 노려보세요!",
            "퀸을 이용하여 체크를 걸어보세요!"
        },
        new List<string>
        {
            "계속해서 체크를 이어나가도록 해보세요!",
            "퀸을 이용하여 체크를 걸어보세요!",
            "한번 더 상대의 기물을 잡으며 체크하도록 하세요!"
        },
        new List<string>
        {
            "퀸을 이용하여 체크를 걸어보세요!",
            "킹에게 붙으면서 체크를 걸어보세요!",
            "상대 킹이 반격할 수 없는 곳으로 이동하여 체크하세요!"
        },
        new List<string>
        {
            "퀸을 이용하여 체크를 걸어보세요!",
            "킹에게 붙으면서 체크를 걸어보세요!",
            "상대 킹이 반격할 수 없는 곳으로 이동하여 체크하세요!"
        }
    };

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
        ChessBoardManager.instance.SetChessBoard(currentStage);
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
        gameStartButton.gameObject.SetActive(false); // 게임 시작 버튼 숨김
    }

    public void OnStageClear()
    {
        playerFailedCount = 0; // 스테이지 클리어 시 실패 횟수 초기화
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
        playerFailedCount++; // 플레이어 실패 횟수 증가
        gameOverPopUp.SetActive(true);
        gameOverText.text = "Stage " + currentStage + " Failed!"; // 게임 오버 텍스트 업데이트
        stageText.gameObject.SetActive(false); // 스테이지 텍스트 숨김

        showFeedback = currentStage >= 4 ? true : false; // 4단계 이상이면 피드백 표시
        feedbackText.gameObject.SetActive(showFeedback); // 피드백 텍스트 활성화여부
        VirtualPlayer.instance.userMoveIndex = 0; // 유저 이동 인덱스 초기화

        // 피드백 텍스트 내용 설정
        int playerMoveCount = ChessBoardManager.instance.playerMoveCount; // 플레이어가 움직인 횟수
        ChessBoardManager.instance.playerMoveCount = 0; // 플레이어 움직임 횟수 초기화

        if (playerFailedCount > 3) playerFailedCount = 3; // 실패 횟수는 최대 3으로 제한
        if (playerMoveCount > 4) playerMoveCount = 4; // 플레이어 움직임 횟수는 최대 4으로 제한
        if (currentStage == 4) feedbackText.text = stage4Hints[playerMoveCount-1][playerFailedCount - 1];
        else if (currentStage == 5) feedbackText.text = stage5Hints[playerMoveCount - 1][playerFailedCount - 1];
    }

    void OnRetryButton()
    {
        gameOverPopUp.SetActive(false); // 게임 오버 팝업 숨김
        ChessBoardManager.instance.SetChessBoard(currentStage); // 현재 스테이지 재설정
        stageText.gameObject.SetActive(true); // 스테이지 텍스트 활성화
        stageText.text = "Stage " + currentStage; // 스테이지 텍스트 업데이트
    }
}
