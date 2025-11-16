using System;
using UnityEngine;

public class StatesTransition
{
    public float CurrentMovementSpeed { get; private set; }
    public float CurrentAnimationSpeed { get; private set; }
    
    public Action<float> OnMovementSpeedChanged { get; set; }
    public Action<float> OnAnimationSpeedChanged { get; set; }
    public StatesTransition() { }
    public void UpdateBlending(State targetState)
    {
        // движение к целевой скорости
        CurrentMovementSpeed = Mathf.MoveTowards(
            CurrentMovementSpeed,
            targetState.MovementSpeed,
            targetState.EnterBlendSpeed * Time.deltaTime
        );

        // вычисление коэффициента анимации
        float animationFactor;

        if (targetState.MovementSpeed == 0f)
        {
            // если целевая скорость = 0, считаем цель достигнутой
            animationFactor = (CurrentMovementSpeed == 0f) ? 1f : CurrentMovementSpeed;
        }
        else
        {
            // разрешаем значения > 1, если CurrentMovementSpeed > targetState.MovementSpeed
            animationFactor = CurrentMovementSpeed / targetState.MovementSpeed;
        }

        CurrentAnimationSpeed = animationFactor;

        OnMovementSpeedChanged?.Invoke(CurrentMovementSpeed);
        OnAnimationSpeedChanged?.Invoke(CurrentAnimationSpeed);
    }


}
