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
        currentStage = 1; // ���� ���� �� �������� �ʱ�ȭ

        ChessBoardManager.instance.SetChessBoard(1);
        stageText.gameObject.SetActive(true); // �������� �ؽ�Ʈ Ȱ��ȭ
        stageText.text = "Stage " + currentStage; // �������� �ؽ�Ʈ ������Ʈ
        gameStartButton.gameObject.SetActive(false); // ���� ���� ��ư ����
    }

    public void OnStageClear()
    {
        stageClearPopUp.SetActive(true);
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
}
