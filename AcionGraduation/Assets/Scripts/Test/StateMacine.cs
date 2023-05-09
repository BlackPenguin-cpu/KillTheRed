using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    private delegate void StateMachineDelegate();

    private struct StateCallbacks
    {
        public StateMachineDelegate enter;
        public StateMachineDelegate update;
        public StateMachineDelegate exit;
    }

    Dictionary<T, StateCallbacks> stateCallbackMap;

    public StateMachine(MonoBehaviour component)
    {
        const BindingFlags includeFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var methods = component.GetType().GetMethods(includeFlags);

        foreach (var method in methods)
        {
            if (!ParseName(method, component, out var stateCallbacks))
            {
                continue;
            }
        }
    }

    private bool ParseName(MethodInfo methodInfo, MonoBehaviour component, out StateCallbacks stateCallbacks)
    {
        int i = 0;

        string enumName = string.Empty;

        stateCallbacks = new StateCallbacks();

        while (enumName != null)
        {
            if (methodInfo.Name.StartsWith($"{enumName}_Enter"))
            {
                stateCallbacks.enter = (StateMachineDelegate)methodInfo.CreateDelegate(typeof(StateMachineDelegate), component);
            }
            if (methodInfo.Name.StartsWith($"{enumName}_Update"))
            {
                stateCallbacks.update = (StateMachineDelegate)methodInfo.CreateDelegate(typeof(StateMachineDelegate), component);
            }
            if (methodInfo.Name.StartsWith($"{enumName}_Exit"))
            {
                stateCallbacks.exit = (StateMachineDelegate)methodInfo.CreateDelegate(typeof(StateMachineDelegate), component);
            }

            enumName = Enum.GetName(typeof(T), i++);
        }

        return stateCallbacks.enter != null || stateCallbacks.update != null || stateCallbacks.exit != null;
    }
}