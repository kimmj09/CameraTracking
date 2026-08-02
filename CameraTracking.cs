using UnityEngine;

public class MarioStyleCamera : MonoBehaviour
{
    [Header("Target & Offset Settings")]
    [SerializeField] private Transform target;           // 플레이어 Transform
    [SerializeField] private float lookAheadDistance = 3f; // 전방 시야 확보 거리 (Look-Ahead)
    [SerializeField] private float smoothSpeed = 5f;        // 카메라 이동 부드러움 계수 (Lerp Speed)

    [Header("Mario World Turn Logic")]
    [SerializeField] private float turnThreshold = 2f;    // 방향 전환 후 카메라가 반응하기 위해 더 이동해야 하는 거리

    private float currentFacingDir = 1f; // 현재 카메라가 바라보고 있는 방향 (1: 오른쪽, -1: 왼쪽)
    private float lastFacingDir = 1f;    // 플레이어의 이전 바라보는 방향
    private Vector3 turnStartPosition;   // 방향을 바꾼 시점의 플레이어 위치
    private bool isWaitingForThreshold = false; // 방향 전환 유예 상태 여부

    private void Start()
    {
        if (target != null)
        {
            // 시작 시 플레이어가 바라보는 방향 초기화 (오른쪽: 1, 왼쪽: -1)
            lastFacingDir = target.localScale.x >= 0 ? 1f : -1f;
            currentFacingDir = lastFacingDir;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 현재 플레이어의 바라보는 방향 확인 (Scale.x 또는 이동 입력 기준)
        float playerFacingDir = GetPlayerFacingDirection();

        // 2. 플레이어가 바라보는 방향을 뒤집었는지 체크
        if (playerFacingDir != lastFacingDir)
        {
            isWaitingForThreshold = true;
            turnStartPosition = target.position; // 방향을 바꾼 '기점' 좌표 기록
            lastFacingDir = playerFacingDir;
        }

        // 3. 슈퍼마리오 월드 로직: 방향을 바꾼 후 일정 거리를 더 걸어갔는지 판정
        if (isWaitingForThreshold)
        {
            // 방향을 바꾼 기점(turnStartPosition)으로부터의 X축 이동 거리 연산
            float movedDistance = Mathf.Abs(target.position.x - turnStartPosition.x);

            // 유예 거리(turnThreshold)를 넘어서면 비로소 카메라의 타겟 방향을 뒤집음
            if (movedDistance >= turnThreshold)
            {
                currentFacingDir = playerFacingDir;
                isWaitingForThreshold = false; // 유예 상태 해제
            }
        }

        // 4. 최종 카메라 목표 위치(Target Position) 계산
        float targetOffsetX = currentFacingDir * lookAheadDistance;
        Vector3 targetPosition = new Vector3(target.position.x + targetOffsetX, target.position.y, transform.position.z);

        // 5. SmoothDamp 또는 Lerp를 이용해 부드럽게 카메라 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // 플레이어의 방향을 판별하는 보조 함수 (프로젝트 방식에 맞게 수정 가능)
    private float GetPlayerFacingDirection()
    {
        // 예시: 플레이어의 Scale.x 가 양수면 오른쪽(1), 음수면 왼쪽(-1)
        return target.localScale.x >= 0 ? 1f : -1f;
    }
}
