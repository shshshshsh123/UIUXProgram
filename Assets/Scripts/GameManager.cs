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

    public int currentStage = 1; // ���� �������� ��ȣ
    public bool showFeedback = false; // �ǵ�� ǥ�� ����

    [Header("# UI")]
    public Button gameStartButton;
    public TMP_Text stageText;
    // Ŭ���� �˾�
    public GameObject stageClearPopUp;
    public TMP_Text stageClearText;
    public Button nextStageButton;
    // ���� �˾�
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
        currentStage = 1; // ���� ���� �� �������� �ʱ�ȭ

        ChessBoardManager.instance.SetChessBoard(1);
        stageText.gameObject.SetActive(true); // �������� �ؽ�Ʈ Ȱ��ȭ
        stageText.text = "Stage " + currentStage; // �������� �ؽ�Ʈ ������Ʈ
        gameStartButton.gameObject.SetActive(false); // ���� ���� ��ư ����
    }

    public void OnStageClear()
    {
        stageClearPopUp.SetActive(true);
        stageClearText.text = "Stage " + currentStage + " Clear!"; // �������� Ŭ���� �ؽ�Ʈ ������Ʈ
        currentStage++; // �������� ����
        stageText.gameObject.SetActive(false);
    }

    void OnNextStageButton()
    {
        stageClearPopUp.SetActive(false); // �������� Ŭ���� �˾� ����
        ChessBoardManager.instance.SetChessBoard(currentStage); // ���� �������� ����
        stageText.gameObject.SetActive(true); // �������� �ؽ�Ʈ Ȱ��ȭ
        stageText.text = "Stage " + currentStage; // �������� �ؽ�Ʈ ������Ʈ
    }

    public void OnGameFailed()
    {
        gameOverPopUp.SetActive(true);
        gameOverText.text = "Stage " + currentStage + " Failed!"; // ���� ���� �ؽ�Ʈ ������Ʈ
        stageText.gameObject.SetActive(false); // �������� �ؽ�Ʈ ����

        if (showFeedback) feedbackText.gameObject.SetActive(true); // �ǵ�� �ؽ�Ʈ Ȱ��ȭ
        else feedbackText.gameObject.SetActive(false); // �ǵ�� �ؽ�Ʈ ��Ȱ��ȭ

        // �ǵ�� �ؽ�Ʈ ���� ����
        feedbackText.text = "�ƾƾƾƾƾƾƾƾ�";
    }

    void OnRetryButton()
    {
        gameOverPopUp.SetActive(false); // ���� ���� �˾� ����
        ChessBoardManager.instance.SetChessBoard(currentStage); // ���� �������� �缳��
        stageText.gameObject.SetActive(true); // �������� �ؽ�Ʈ Ȱ��ȭ
        stageText.text = "Stage " + currentStage; // �������� �ؽ�Ʈ ������Ʈ
    }
}
