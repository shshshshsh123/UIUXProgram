using UnityEngine;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    public int x, y;
    public System.Action<int, int> onClick;
    public GameObject canMoveImage;
    public GameObject canAttackImage;

    private void Start()
    {
        canMoveImage.SetActive(false);
        canAttackImage.SetActive(false);
    }

    public void Init(int x, int y, System.Action<int, int> clickCallback)
    {
        this.x = x;
        this.y = y;
        onClick = clickCallback;
        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(x, y));

        // 색
        gameObject.GetComponent<Image>().color = (x + y) % 2 == 1 ? Color.white : Color.gray;
    }

    public void ShowCanMove(bool show)
    {
        canMoveImage.SetActive(show);
    }

    public void ShowCanAttack(bool show)
    {
        canAttackImage.SetActive(show);
    }
}
