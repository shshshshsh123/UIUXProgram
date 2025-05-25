using UnityEngine;
using UnityEngine.UI;

public class TileButton : MonoBehaviour
{
    private int x, y;
    private System.Action<int, int> onClick;

    public void Init(int x, int y, System.Action<int, int> clickCallback)
    {
        this.x = x;
        this.y = y;
        onClick = clickCallback;
        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(x, y));

        // 색
        gameObject.GetComponent<Image>().color = (x + y) % 2 == 1 ? Color.white : new Color(0,0,0,0.5f);
    }
}
