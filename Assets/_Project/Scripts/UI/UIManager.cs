using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform screenParent;

    private readonly Dictionary<Type, UIScreen> screens = new();
    private readonly Stack<UIScreen> screenStack = new();

    private void Awake()
    {
        var parent = screenParent != null ? screenParent : transform;
        var allScreens = parent.GetComponentsInChildren<UIScreen>(true);

        foreach (var screen in allScreens)
        {
            var type = screen.GetType();

            if (!screens.ContainsKey(type))
            {
                //screen.Initialize();
                screens[type] = screen;
            }
        }

        Debug.Log($"[UI] Registered {screens.Count} screens.");
    }

    #region Screen Management
    public T GetScreen<T>() where T : UIScreen
    {
        screens.TryGetValue(typeof(T), out var screen);
        return screen as T;
    }

    public void ShowScreen<T>() where T : UIScreen
    {
        if (screens.TryGetValue(typeof(T), out var screen))
            screen.Show();

        else
            Debug.LogWarning($"[UI] Screen {typeof(T).Name} not found.");
    }

    public void HideScreen<T>() where T : UIScreen
    {
        if (screens.TryGetValue(typeof(T), out var screen))
            screen.Hide();
    }

    public void HideAllScreens()
    {
        foreach (var screen in screens.Values)
            screen.Hide();

        screenStack.Clear();
    }

    public void PushScreen<T>() where T : UIScreen
    {
        if (screens.TryGetValue(typeof(T), out var screen))
        {
            if (screenStack.Count > 0)
                screenStack.Peek().Hide();

            screenStack.Push(screen);
            screen.Show();
        }

        else
            Debug.LogWarning($"[UI] Screen {typeof(T).Name} not found.");
    }

    public UIScreen PopScreen()
    {
        if (screenStack.Count == 0)
            return null;

        var top = screenStack.Pop();
        top.Hide();

        if (screenStack.Count > 0)
            screenStack.Peek().Show();

        return top;
    }

    public void ClearStack()
    {
        while (screenStack.Count > 0)
            screenStack.Pop().Hide();
    }
    #endregion
}