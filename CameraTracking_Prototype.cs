using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float trackingSpeed = 5f; 

    [Header("Forward View Settings")]
    // 캐릭터가 바라보는 전방 시야를 얼마나 넓게 보여줄지 결정하는 거리 (오프셋)
    [SerializeField] private float forwardOffset = 4.0f;
    // 방향이 바뀔 때 카메라가 좌우로 화면을 전환하는 속도
    [SerializeField] private float flipSmoothSpeed = 3.0f;

    private PlayerController playerController;
    private float currentXOffset;

    private void Start()
    {
        if (target != null)
        {
            // 타겟(Aster)으로부터 방향 상태를 가져오기 위해 컴포넌트를 미리 연결합니다.
            playerController = target.GetComponent<PlayerController>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 cameraPos = transform.position;
        bool isFacingRight = true;

        // PlayerController 스크립트에서 public 변수나 프로퍼티로 방향을 참조합니다.
        if (playerController != null)
        {
            // 주의: PlayerController의 isFacingRight 변수를 기존 private에서 'public'으로 변경해 주어야 합니다!
            isFacingRight = playerController.isFacingRight; 
        }

        // 1. 가로축: 바라보는 방향에 따른 목표 오프셋 계산 (수식 보정)
        // 오른쪽을 볼 때는 +forwardOffset, 왼쪽을 볼 때는 -forwardOffset
        float targetXOffset = isFacingRight ? forwardOffset : -forwardOffset;

        // 오프셋 값 자체를 부드럽게 보간하여 방향 전환 시 카메라가 슥 움직이게 만듭니다.
        currentXOffset = Mathf.Lerp(currentXOffset, targetXOffset, flipSmoothSpeed * Time.deltaTime);

        // 최종 카메라의 가로 목표 좌표 = 플레이어 위치 + 계산된 방향 오프셋
        float targetXPos = target.position.x + currentXOffset;
        cameraPos.x = Mathf.Lerp(cameraPos.x, targetXPos, trackingSpeed * Time.deltaTime);


        // 2. 세로축: 기존에 작성하신 시간 독립적 점근적 평균 Lerp 수식 유지
        if (target.position.y != cameraPos.y)
        {
            cameraPos.y = Mathf.Lerp(cameraPos.y, target.position.y, trackingSpeed * Time.deltaTime);
        }

        transform.position = cameraPos;
    }

    // 기존 Gizmos 사각형 대신 현재 카메라가 타겟팅하는 중심을 선으로 시각화합니다.
    private void OnDrawGizmos()
    {
        if (target == null) return;
        Gizmos.color = Color.green;
        Vector3 targetGoal = new Vector3(target.position.x + currentXOffset, target.position.y, transform.position.z);
        Gizmos.DrawWireSphere(targetGoal, 0.5f);
    }
}
