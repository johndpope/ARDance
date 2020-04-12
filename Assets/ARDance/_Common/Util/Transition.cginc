#ifndef Transition_INCLUDED
#define Transition_INCLUDED

float4 _TransitionColor;
float _TransitionThreshold;

float4 TransitWithColor (float4 originColor)
{
    return lerp(originColor, _TransitionColor, _TransitionThreshold);
}
#endif
