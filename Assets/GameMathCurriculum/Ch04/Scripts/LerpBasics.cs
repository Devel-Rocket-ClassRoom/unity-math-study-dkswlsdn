// =============================================================================
// LerpBasics.cs
// -----------------------------------------------------------------------------
// Lerp를 활용한 HP 바 보간, 위치 추적, 색상 전환 데모
// =============================================================================

using UnityEngine;
using TMPro;
using NUnit.Framework.Interfaces;

public class LerpBasics : MonoBehaviour
{
    [Header("=== HP 보간 설정 ===")]
    [SerializeField] private float maxHP = 100f;
    [Tooltip("현재 실제 HP (데미지 입음)")]
    [SerializeField] private float currentHP = 100f;
    [Tooltip("표시할 HP (UI에 실시간으로 보간됨)")]
    [SerializeField] private float displayHP = 100f;
    [Tooltip("HP 감소 속도 (지수 감쇠 모드) — 클수록 빠름")]
    [Range(1f, 20f)]
    [SerializeField] private float hpLerpSpeed = 5f;
    [Tooltip("절대적 Lerp 모드: 고정 시간으로 선형 감소")]
    [SerializeField] private float absoluteLerpDuration = 1f;

    [Space(10)]
    [Header("=== HP 보간 모드 ===")]
    [Tooltip("체크: 절대적 Lerp(시간 기반) | 해제: 지수 감쇠 (Time.deltaTime 기반)")]
    [SerializeField] private bool useAbsoluteLerp = false;

    private float lerpElapsedTime = 0f;
    private float lerpStartHP = 0f;
    private float lerpTargetHP = 0f;

    [Space(10)]
    [Header("=== 위치 추적 설정 (카메라 Follow 효과) ===")]
    [Tooltip("따라갈 대상 Transform")]
    [SerializeField] private Transform targetTransform = null;
    [Tooltip("위치 보간 속도")]
    [Range(1f, 20f)]
    [SerializeField] private float positionLerpSpeed = 5f;
    [Tooltip("위치 추적 모드: 체크=Lerp(가속도감) | 해제=MoveTowards(일정속도)")]
    [SerializeField] private bool usePositionLerp = true;

    [Space(10)]
    [Header("=== 색상 Lerp 설정 ===")]
    [Tooltip("Lerp 시작 색상")]
    [SerializeField] private Color colorStart = Color.white;
    [Tooltip("Lerp 목표 색상")]
    [SerializeField] private Color colorEnd = Color.red;
    [Tooltip("색상 변환 속도")]
    [Range(0.1f, 5f)]
    [SerializeField] private float colorLerpSpeed = 1f;

    [Space(10)]
    [Header("=== UI 요소 ===")]
    [Tooltip("정보 출력 텍스트 (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI uiInfoText = null;

    [Space(10)]
    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private float debugHPDifference = 0f;
    [SerializeField] private float debugLerpT = 0f;
    [SerializeField] private float debugColorT = 0f;
    [SerializeField] private float debugPositionDistance = 0f;

    // 시작 딜레이
    [Header("=== 시작 딜레이 ===")]
    [Tooltip("플레이 시작 후 대기 시간 (초)")]
    [Range(0f, 5f)]
    [SerializeField] private float startDelay = 1f;
    private float startDelayTimer = 0f;
    private bool hasStarted = false;

    private float colorLerpTime = 0f;

    private Renderer meshRenderer = null;
    private Material materialInstance = null;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
        }

        displayHP = currentHP;
        lerpStartHP = currentHP;
        lerpTargetHP = currentHP;
    }

    private void Update()
    {
        // 1초 딜레이 후 시작
        if (!hasStarted)
        {
            startDelayTimer += Time.deltaTime;
            if (startDelayTimer < startDelay) return;
            hasStarted = true;
        }

        // 키 입력: Q = 데미지 20, E = 회복 20
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            TakeDamage(20f);
        }
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Heal(20f);
        }

        UpdateHPLerp();
        UpdatePositionTracking();
        UpdateColorLerp();
        UpdateDebugInfo();
        UpdateUI();
    }

    private void UpdateHPLerp()
    {
        if (useAbsoluteLerp)
        {
            lerpElapsedTime = Time.deltaTime;
            float t = Mathf.Clamp01(lerpElapsedTime / absoluteLerpDuration);

            displayHP = Mathf.Lerp(lerpStartHP, lerpTargetHP, t);

            if (t >= 1)
            {
                displayHP = lerpStartHP = lerpTargetHP;
                lerpElapsedTime = 0f;
            }
        }
        else
        {
            displayHP = Mathf.Lerp(displayHP, currentHP, Time.deltaTime * hpLerpSpeed);

            if (Mathf.Abs(displayHP - currentHP) < 0.01f) displayHP = currentHP;
        }
    }

    private void UpdatePositionTracking()
    {
        if (usePositionLerp)
        {
            transform.position = Vector3.Lerp(
                transform.position, targetTransform.position, positionLerpSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetTransform.position, positionLerpSpeed * Time.deltaTime);
        }
    }

    private void UpdateColorLerp()
    {
        colorLerpTime = (colorLerpTime + Time.deltaTime) % (2f / colorLerpSpeed);
        float colorT = Mathf.PingPong(colorLerpTime * colorLerpSpeed, 1f);

        materialInstance.color = Color.Lerp(colorStart, colorEnd, colorT);
        debugColorT = colorT;
    }

    private void UpdateDebugInfo()
    {
        debugHPDifference = Mathf.Abs(displayHP - currentHP);

        if (useAbsoluteLerp)
        {
            debugLerpT = Mathf.Clamp01(lerpElapsedTime / absoluteLerpDuration);
        }
        else
        {
            debugLerpT = 1f - (Mathf.Abs(displayHP - currentHP) / maxHP);
        }

        if (targetTransform != null)
        {
            debugPositionDistance = Vector3.Distance(transform.position, targetTransform.position);
        }
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        string modeText = useAbsoluteLerp ? $"절대적 Lerp ({absoluteLerpDuration}초)" : "지수 감쇠";
        string positionModeText = usePositionLerp ? "Lerp(가속도감)" : "MoveTowards(일정속도)";

        uiInfoText.text =
            $"<b>[LerpBasics] 선형 보간 기초</b>\n\n" +
            $"<b>1. HP 보간</b>\n" +
            $"  현재 HP: <color=yellow>{currentHP:F1}</color> / {maxHP:F0}\n" +
            $"  표시 HP: <color=green>{displayHP:F1}</color>\n" +
            $"  차이: {debugHPDifference:F2}  |  t값: {debugLerpT:F3}\n" +
            $"  모드: <color=cyan>{modeText}</color>\n\n" +
            $"<b>2. 위치 추적</b>\n" +
            $"  거리: <color=yellow>{debugPositionDistance:F2}</color>m\n" +
            $"  모드: <color=cyan>{positionModeText}</color>\n\n" +
            $"<b>3. 색상 Lerp</b>\n" +
            $"  t값: {debugColorT:F3}";
    }

    public void TakeDamage(float damageAmount)
    {
        currentHP = Mathf.Max(0f, currentHP - damageAmount);

        if (useAbsoluteLerp)
        {
            lerpStartHP = displayHP;
            lerpTargetHP = currentHP;
            lerpElapsedTime = 0f;
        }
    }

    public void Heal(float healAmount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);

        if (useAbsoluteLerp)
        {
            lerpStartHP = displayHP;
            lerpTargetHP = currentHP;
            lerpElapsedTime = 0f;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        if (targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetTransform.position);

            DrawArrowGizmo(transform.position, direction * distance, Color.cyan);

            Vector3 midpoint = Vector3.Lerp(transform.position, targetTransform.position, 0.5f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetTransform.position, 0.2f);
        }
    }

    private void DrawArrowGizmo(Vector3 start, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        Vector3 end = start + direction;
        Gizmos.DrawLine(start, end);

        float arrowHeadSize = 0.3f;
        Vector3 right = Vector3.Cross(direction.normalized, Vector3.up) * arrowHeadSize;
        Vector3 up = Vector3.Cross(direction.normalized, right) * arrowHeadSize;

        Gizmos.DrawLine(end, end - direction.normalized * arrowHeadSize + right);
        Gizmos.DrawLine(end, end - direction.normalized * arrowHeadSize - right);
        Gizmos.DrawLine(end, end - direction.normalized * arrowHeadSize + up);
        Gizmos.DrawLine(end, end - direction.normalized * arrowHeadSize - up);
    }

#endif
}
