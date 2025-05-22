using System.Collections.Generic;
using UnityEngine;

// AI의 게임 계획
// 단순히 [x1,y1]에 있는 기물을 [x2,y2]로 움직이는 구조
public class Plan
{
    public int originX, originY; // 움직여야할 기물의 위치
    public int targetX, targetY; // 해당 기물을 옮길 위치

    public Plan(int ox, int oy, int tx, int ty)
    {
        originX = ox;
        originY = oy;
        targetX = tx;
        targetY = ty;
    }
}

// AI 스크립트
// 플레이어가 턴을 종료하면 정해진 대로 기물을 움직인다.
public class VirtualPlayer : MonoBehaviour
{
    // 플랜 리스트
    private List<Plan> planList = new List<Plan>();
    private ChessBoardManager chessBoardManager; // 체스 보드 매니저
    private void Start()
    {
        chessBoardManager = GameObject.FindAnyObjectByType<ChessBoardManager>();
        ActionManager.whenPlayerMoved += DoAction; // 액션 구독
        PlanInit(); // 플랜 초기화
    }

    // 기물을 움직인다.
    public void DoAction()
    {
        ChessPiece targetPiece; // 움직일 기물
        Plan curruntPlan; // 현재 턴의 플랜

        // 플랜 리스트에서 첫번째 플랜을 뽑는다.
        curruntPlan = planList[0];
        planList.RemoveAt(0); // 이미 뽑힌 플랜은 폐기

        // 플랜에 있는 대로 기물을 움직인다
        targetPiece = chessBoardManager.pieces[curruntPlan.originX, curruntPlan.originY];
        targetPiece.MoveTo(curruntPlan.targetX, curruntPlan.targetY);
    }

    // 이곳에서 플랜 리스트 생성 (하드코딩)
    protected virtual void PlanInit()
    {
        // planList.Add(new Plan(0,0,1,1)); (예시)
    }

    void OnDestroy()
    {
        ActionManager.whenPlayerMoved -= DoAction; // 액션 구독 취소
    }
}