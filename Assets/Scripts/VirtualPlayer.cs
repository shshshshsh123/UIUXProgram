using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

// 유저가 움직여야할 기물의 위치와 이동할 위치를 저장하는 클래스
public class UserMove
{
    public int originX, originY; // 움직여야할 기물의 위치
    public int targetX, targetY; // 해당 기물을 옮길 위치

    public UserMove(int ox, int oy, int tx, int ty)
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
    public static VirtualPlayer instance; // 싱글톤 인스턴스

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // 플랜 리스트
    private List<Plan> planList = new List<Plan>();
    private List<UserMove> userMoveList = new List<UserMove>();

    public int userMoveIndex = 0; // 유저의 움직임 횟수

    private void Start()
    {
        ActionManager.whenPlayerMoved += DoAction; // 액션 구독
    }

    // 기물을 움직인다.
    public void DoAction()
    {
        if (userMoveIndex > planList.Count)
        {
            // 클리어
            Debug.Log("게임 클리어!");
            ChessBoardManager.instance.OnStageClear();
        }

        ChessPiece targetPiece; // 움직일 기물
        Plan curruntPlan; // 현재 턴의 플랜

        // 플랜 리스트에서 플랜 가져오기
        curruntPlan = planList[userMoveIndex - 1];

        // 플랜에 있는 대로 기물을 움직인다
        targetPiece = ChessBoardManager.instance.pieces[curruntPlan.originX, curruntPlan.originY];
        if( targetPiece == null )
        {
            Debug.Log("null: " + curruntPlan.originX + "," + curruntPlan.originY);
        }
        targetPiece.MoveTo(curruntPlan.targetX, curruntPlan.targetY);

        // 목표 지점에 이미 말이 있는 경우 잡는다.
        ChessPiece enemyPiece = ChessBoardManager.instance.pieces[curruntPlan.targetX, curruntPlan.targetY];
        if( enemyPiece != null ) ChessBoardManager.instance.pieces[enemyPiece.x, enemyPiece.y].gameObject.SetActive(false); // 적 기물 제거

        ChessBoardManager.instance.pieces[curruntPlan.originX, curruntPlan.originY] = null; // 기존 위치에 있던 기물 제거
        ChessBoardManager.instance.pieces[curruntPlan.targetX, curruntPlan.targetY] = targetPiece; // 새로운 위치로 기물 이동        
    }

    public bool CheckUserMove(Vector2Int start, Vector2Int end)
    {
        if (userMoveIndex >= userMoveList.Count)
        {
            Debug.Log("모든 이동 완료!");
            return false;
        }

        if (start.x == end.x && start.y == end.y)
        {
            Debug.Log("이동하지 않음: " + start + " -> " + end);
            return true; // 이동하지 않으면 취소
        }

        UserMove correctMove = userMoveList[userMoveIndex];

        if (correctMove.originX == start.x && correctMove.originY == start.y
            && correctMove.targetX == end.x && correctMove.targetY == end.y)
        {
            Debug.Log("정답 이동: " + start + " -> " + end);
            userMoveIndex++;
            return true;
        }
        else
        {
            Debug.Log("오답! 정답은: " + start + " -> " + end);
            return false;
        }

    }

    // 이곳에서 플랜 리스트 생성 (하드코딩)
    public void PlanInit(int stage)
    {
        planList.Clear();
        userMoveList.Clear();
        userMoveIndex = 0; // 유저 이동 인덱스 초기화

        if ( stage == 1 )
        {
            planList.Add(new Plan(7, 6, 6, 5));
            planList.Add(new Plan(6, 5, 5, 4));
            planList.Add(new Plan(6, 1, 6, 4));

            userMoveList.Add(new UserMove(2, 7, 7, 7));
            userMoveList.Add(new UserMove(1, 7, 6, 7));
            userMoveList.Add(new UserMove(7, 7, 7, 4));
            userMoveList.Add(new UserMove(6, 7, 6, 4));
        }

        if( stage == 2 )
        {
            planList.Add(new Plan(2, 7, 2, 6));
            planList.Add(new Plan(1, 7, 0, 6));

            userMoveList.Add(new UserMove(5, 5, 4, 4));
            userMoveList.Add(new UserMove(1, 5, 3, 6));
            userMoveList.Add(new UserMove(4, 4, 0, 4));
        }

        if( stage == 3 )
        {
            planList.Add(new Plan(3, 2, 2, 1));
            planList.Add(new Plan(2, 1, 1, 2));
            planList.Add(new Plan(1, 2, 0, 3));

            userMoveList.Add(new UserMove(0, 5, 3, 5));
            userMoveList.Add(new UserMove(4, 6, 2, 6));
            userMoveList.Add(new UserMove(3, 5, 3, 2));
            userMoveList.Add(new UserMove(2, 6, 0, 6));
        }

        if( stage == 4 )
        {
            planList.Add(new Plan(5, 6, 6, 5));
            planList.Add(new Plan(7, 6, 7, 7));
            planList.Add(new Plan(1, 2, 1, 7));

            userMoveList.Add(new UserMove(6, 4, 6, 5));
            userMoveList.Add(new UserMove(5, 4, 6, 5));
            userMoveList.Add(new UserMove(0, 5, 0, 7));
            userMoveList.Add(new UserMove(0, 7, 1, 7));
        }

        if( stage == 5 )
        {
            planList.Add(new Plan(1, 5, 0, 4));
            planList.Add(new Plan(0, 4, 0, 3));
            planList.Add(new Plan(0, 3, 0, 2));

            userMoveList.Add(new UserMove(6, 5, 3, 5));
            userMoveList.Add(new UserMove(3, 5, 2, 4));
            userMoveList.Add(new UserMove(2, 4, 1, 4));
            userMoveList.Add(new UserMove(1, 4, 1, 2));
        }

        if( stage == 6 )
        {
            planList.Add(new Plan(6, 6, 6, 7));
            planList.Add(new Plan(6, 7, 7, 7));
            planList.Add(new Plan(7, 7, 6, 7));

            userMoveList.Add(new UserMove(5, 3, 4, 5));
            userMoveList.Add(new UserMove(7, 2, 7, 7));
            userMoveList.Add(new UserMove(3, 1, 7, 5));
            userMoveList.Add(new UserMove(7, 5, 6, 6));
        }
    }

    void OnDestroy()
    {
        ActionManager.whenPlayerMoved -= DoAction; // 액션 구독 취소
    }
}