#ifndef FXWHIP_HLSL
#define FXWHIP_HLSL

#include "ParticlesUnlitInputNew.hlsl"

// ============================================
// FXWhip Vertex Offset Function
// ============================================
// 功能：根据起始/结束位置和方向数组，对顶点进行弯曲偏移
// 输入：WPos - 世界空间位置
// 返回：偏移后的世界空间位置
// ============================================

float3 FXWhip(float3 WPos)
{
    float instanceDistance = _FXWhip_Distance;
    float3 dir = normalize(_FXWhip_EndPosition - _FXWhip_StartPosition);
    float i = distance(WPos, _FXWhip_StartPosition) / instanceDistance; 
    
    int up_index = ceil(i);
    int down_index = floor(i);
    
    // 边界检查：确保索引在有效范围内（使用_FXWhip_Num作为数组数量）
    if(_FXWhip_Num > 0)
    {
        int maxIndex = (int)_FXWhip_Num - 1;
        up_index = min(up_index, maxIndex);
        down_index = max(0, min(down_index, maxIndex));
        
        float3 bendP = lerp(_FXWhip_TexDir[down_index].xyz, _FXWhip_TexDir[up_index].xyz, clamp(frac(i), 0, 1));
        
        i = distance(WPos, _FXWhip_StartPosition) / instanceDistance;
        up_index = ceil(i);
        down_index = floor(i);
        up_index = min(up_index, maxIndex);
        down_index = max(0, min(down_index, maxIndex));
        
        float3 bendP_start = lerp(_FXWhip_StartPosition + dir * down_index * _FXWhip_Distance, 
                                   _FXWhip_StartPosition + dir * up_index * instanceDistance, 
                                   clamp(frac(i), 0, 1));
        
        return bendP + (WPos - bendP_start);
    }
    else
    {
        // 如果数组为空，返回原始位置
        return WPos;
    }
}

#endif // FXWHIP_HLSL

