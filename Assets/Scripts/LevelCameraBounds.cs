using Unity.Cinemachine; // Cinemachine 3. For v2: "using Cinemachine;"
using UnityEngine;

[ExecuteAlways]
public class LevelCameraBounds : CinemachineExtension
{
    [SerializeField] private LevelManager level;
    [SerializeField] private float bottomPadding = 0f; // extra gap above the barrier

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body || level == null) return;

        float halfH = state.Lens.OrthographicSize;
        float halfW = halfH * state.Lens.Aspect;

        Vector3 pos = state.RawPosition;

        // Bottom: camera's lower edge never goes below the barrier
        pos.y = Mathf.Max(pos.y, level.barrierY + halfH + bottomPadding);

        // Sides: assumes level is centred on x = 0
        float limit = level.levelSettings.levelWidth * 0.5f - halfW;
        pos.x = limit > 0f ? Mathf.Clamp(pos.x, -limit, limit) : 0f;

        state.RawPosition = pos;
    }
}