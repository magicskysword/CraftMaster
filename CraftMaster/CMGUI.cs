using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Items;
using Kingmaker.Localization;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using ModKit;
using UnityEngine;

namespace CraftMaster;

public static class CMGUI
{
    static CMGUI()
    {
        ButtonNormalStyle = new GUIStyle(GUI.skin.button);
        ButtonPressStyle = new GUIStyle(GUI.skin.button);
        BoxStyle = new GUIStyle(GUI.skin.box);
        CenterLabelStyle = new GUIStyle(GUI.skin.label);
        TooltipStyle = new GUIStyle(GUI.skin.box);

        ButtonNormalStyle.normal = GUI.skin.box.normal;
        ButtonPressStyle.normal = GUI.skin.button.normal;
        CenterLabelStyle.alignment = TextAnchor.MiddleCenter;
        TooltipStyle.normal.textColor = Color.white;
        TooltipStyle.alignment = TextAnchor.UpperLeft;
        TooltipStyle.wordWrap = true;
    }
    
    public static GUIStyle ButtonNormalStyle;
    public static GUIStyle ButtonPressStyle;
    public static GUIStyle BoxStyle;
    public static GUIStyle CenterLabelStyle;
    public static GUIStyle LeftLabelStyle;
    public static GUIStyle TooltipStyle;

    public static GUILayoutOption[] WidthArray = new GUILayoutOption[1];

    
    private static string _tooltipText = string.Empty;
    private static int _tooltipMaxWidth = 600;

    public static void OnTooltip()
    {
        if (!string.IsNullOrEmpty(_tooltipText) && Event.current.type == EventType.Repaint)
        {
            var oldEnabled = GUI.enabled;
            GUI.enabled = true;
            
            // 估算最大宽度
            var calcSize = TooltipStyle.CalcSize(new GUIContent(_tooltipText));
            // 限制宽度
            var width = Math.Min(calcSize.x, _tooltipMaxWidth);
            var calcHeight = TooltipStyle.CalcHeight(new GUIContent(_tooltipText), width);
            var height = calcHeight + 10;
            var rect = new Rect(Event.current.mousePosition.x + 20, Event.current.mousePosition.y + 20, width, height);
            
            // 使用Window绘制Tooltip
            GUI.Box(rect, _tooltipText, TooltipStyle);
        
            GUI.enabled = oldEnabled;
        }
        
        _tooltipText = string.Empty;
    }
    
    public static void SetTooltip(string text)
    {
        if (Event.current.type == EventType.Repaint &&
            GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            _tooltipText = text;
        }
    }

    /// <summary>
    /// 获取长度设置
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    public static GUILayoutOption[] GetWidthOptions(int? width)
    {
        var array = Array.Empty<GUILayoutOption>();
        if (width != null)
        {
            WidthArray[0] = GUILayout.Width(width.Value);
            array = WidthArray;
        }

        return array;
    }
    
    public static void NTitle(string stringKey, params GUILayoutOption[] options)
    {
        NTitleRaw(GameUtils.GetString(stringKey), 14, options);
    }
    
    public static void NTitleRaw(string title, int fontSize = 14,params GUILayoutOption[] guiLayoutOptions)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b><size={fontSize}>{title}</size></b>", guiLayoutOptions);
        GUILayout.EndHorizontal();
    }
    
    public static void NTitleCenter(string stringKey, params GUILayoutOption[] options)
    {
        NTitleCenterRaw(GameUtils.GetString(stringKey), 14, options);
    }
    
    public static void NTitleCenterRaw(string title, int fontSize = 14,params GUILayoutOption[] guiLayoutOptions)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b><size={fontSize}>{title}</size></b>", CenterLabelStyle, guiLayoutOptions);
        GUILayout.EndHorizontal();
    }

    public static void NLabel(string stringKey, params GUILayoutOption[] options)
    {
        NLabelRaw(GameUtils.GetString(stringKey), options);
    }
    
    public static void NLabelRaw(string title, params GUILayoutOption[] guiLayoutOptions)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, guiLayoutOptions);
        GUILayout.EndHorizontal();
    }
    
    public static bool NButton(string stringKey, int? width = null)
    {
        return NButtonRaw(GameUtils.GetString(stringKey), width);
    }

    public static bool NButtonRaw(string title, int? width = null)
    {
        return GUILayout.Button(title, GetWidthOptions(width));
    }
    
    public static bool NButtonWithTooltip(string stringKey, string tooltip, int? width = null)
    {
        return NButtonWithTooltipRaw(GameUtils.GetString(stringKey), tooltip, width);
    }

    public static bool NButtonWithTooltipRaw(string title, string tooltip, int? width = null)
    {
        var isClick = GUILayout.Button(title, GetWidthOptions(width));
        SetTooltip(tooltip);
        return isClick;
    }

    /// <summary>
    /// 与Toggle的区别是，按下只会触发一次
    /// </summary>
    /// <param name="stringKey"></param>
    /// <param name="isPress"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static bool NToggleButton(string stringKey, bool isPress,params GUILayoutOption[] options)
    {
        return NToggleButtonRaw(GameUtils.GetString(stringKey), isPress, options);
    }
    
    public static bool NToggleButtonRaw(string title, bool isPress,params GUILayoutOption[] options)
    {
        bool isClick;
        if (isPress)
        {
            isClick = GUILayout.Button(title, ButtonPressStyle, options);
        }
        else
        {
            isClick = GUILayout.Button(title, ButtonNormalStyle, options);
        }
        return isClick;
    }
    
    public static bool NToggle(string stringKey, bool value, int? width = null)
    {
        return NToggleRaw(GameUtils.GetString(stringKey), value, width);
    }
    
    public static bool NToggleRaw(string title, bool value, int? width = null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(140));
        value = GUILayout.Toggle(value, "", GetWidthOptions(width));
        GUILayout.EndHorizontal();
        return value;
    }

    public static string NInput(string stringKey, string value, int? width = 400)
    {
        return NInputRaw(GameUtils.GetString(stringKey), value, width);
    }

    public static string NInputRaw(string title, string value, int? width)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(140));
        value = GUILayout.TextField(value, GetWidthOptions(width));
        GUILayout.EndHorizontal();
        return value;
    }

    public static void Horizontal(Action action)
    {
        GUILayout.BeginHorizontal();
        action.Invoke();
        GUILayout.EndHorizontal();
    }
    
    public static void Vertical(Action action, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
        action.Invoke();
        GUILayout.EndVertical();
    }

    public static void Grid<T>(IEnumerable<T> enumerable ,Action<T, int, GUILayoutOption[]> render,
        int? numPerRow = null, int? perWidth = null)
    {
        bool closeHorizontal = true;

        GUILayout.BeginVertical();
        if (perWidth == null)
        {
            perWidth = 180;
        }
        
        if (numPerRow == null)
        {
            var width = UI.ummWidth - 200;
            if (width <= perWidth)
            {
                numPerRow = 1;
            }
            else
            {
                numPerRow = Math.Max(1, (int) (width / perWidth));
            }
        }
        
        var options = new GUILayoutOption[] { GUILayout.Width(perWidth.Value) };
        var index = 0;
        foreach (var data in enumerable)
        {
            // 换行渲染
            if (index % numPerRow == 0)
            {
                if (!closeHorizontal)
                {
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                closeHorizontal = false;
            }
            
            render.Invoke(data,index, options);
            
            index += 1;
        }

        if (!closeHorizontal)
        {
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();
    }
    
    public static T ToggleGroup<T>(IEnumerable<T> enumerable, T selectedItem, Func<T, string> nameGetter,Func<T, string> tooltipGetter = null,
        int? numPerRow = null, int? perWidth = null) where T : class
    {
        var current = selectedItem;
        var list = enumerable.ToArray();
        if (!list.Contains(current))
        {
            if(list.Length > 0)
                current = list[0];
            else
                current = null;
        }
        Grid(list, (data, index, options) =>
        {
            if(current == null)
                current = data;
            
            if (NToggleButtonRaw(nameGetter.Invoke(data),data.Equals(selectedItem) ,options))
            {
                current = data;
            }
            if(tooltipGetter != null)
            {
                SetTooltip(tooltipGetter.Invoke(data));
            }
        }, numPerRow, perWidth);
        
        return current;
    }
    
    public static int ToggleGroupNumber(IEnumerable<int> enumerable, int selectedItem, Func<int, string> nameGetter, 
        Func<int, string> tooltipGetter = null,
        int? numPerRow = null, int? perWidth = null)
    {
        var current = selectedItem;
        var list = enumerable.ToArray();
        if (!list.Contains(current))
        {
            if(list.Length > 0)
                current = list[0];
            else
                current = 0;
        }
        Grid(list, (data, index, options) =>
        {
            if (NToggleButtonRaw(nameGetter.Invoke(data),data.Equals(selectedItem) ,options))
            {
                current = data;
            }
            if(tooltipGetter != null)
            {
                SetTooltip(tooltipGetter.Invoke(data));
            }
        }, numPerRow, perWidth);
        
        return current;
    }

    private static List<GUILayoutOption> _scrollListOptions = new List<GUILayoutOption>();
    public static Vector2 VerticalScrollList<T>(Vector2 scrollPos,IEnumerable<T> loadItems, Action<T, int, GUILayoutOption[]> render,
        int? height = null, int? width = null)
    {
        if(height == null)
        {
            height = 1200;
        }
        
        _scrollListOptions.Clear();
        _scrollListOptions.Add(GUILayout.Height(height.Value));
        var retPos = GUILayout.BeginScrollView(scrollPos, _scrollListOptions.ToArray());
        foreach (var loadItem in loadItems)
        {
            render.Invoke(loadItem, 0, GetWidthOptions(width));
        }
        GUILayout.EndScrollView();
        
        return retPos;
    }
    
    public static Vector2 VerticalScrollPanel(Vector2 scrollPos,Action render, int? height = null)
    {
        if(height == null)
        {
            height = 600;
        }
        
        _scrollListOptions.Clear();
        _scrollListOptions.Add(GUILayout.Height(height.Value));
        var retPos = GUILayout.BeginScrollView(scrollPos, _scrollListOptions.ToArray());
        render.Invoke();
        GUILayout.EndScrollView();
        
        return retPos;
    }

    public static void VerticalSpace(int space)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(space);
        GUILayout.EndVertical();
    }
    
    public static void HorizontalSpace(int space)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(space);
        GUILayout.EndHorizontal();
    }

    public static void Box(string stringKey, Action action = null, int fontSize = 12)
    {
        BoxRaw(GameUtils.GetString(stringKey), action, fontSize);
    }
    
    public static void Box(Action action = null, int fontSize = 12)
    {
        BoxRaw(null, action, fontSize);
    }

    private static void BoxRaw(string title, Action action, int fontSize)
    {
        if (!string.IsNullOrEmpty(title))
        {
            GUILayout.Label($"<b><size={fontSize}>{title}</size></b>");
        }
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical(BoxStyle);
        action?.Invoke();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public static void FlexibleSpace()
    {
        GUILayout.FlexibleSpace();
    }

    public static int NSliderInt(string title, int curValue, int minValue, int maxValue)
    {
        curValue = NSliderIntRaw(GameUtils.GetString(title), curValue, minValue, maxValue);
        return curValue;
    }

    private static int NSliderIntRaw(string title, int curValue, int minValue, int maxValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(80));
        curValue = (int) GUILayout.HorizontalSlider(curValue, minValue, maxValue, GUILayout.Width(100));
        var intStr = GUILayout.TextField(curValue.ToString(), GUILayout.Width(50));
        if (int.TryParse(intStr, out var value))
        {
            curValue = value;
        }
        GUILayout.EndHorizontal();
        curValue = Mathf.Clamp(curValue, minValue, maxValue);
        return curValue;
    }

    public static float NSliderFloat(string title, float curValue, float minValue, float maxValue)
    {
        curValue = NSliderFloatRaw(GameUtils.GetString(title), curValue, minValue, maxValue);
        return curValue;
    }

    private static float NSliderFloatRaw(string title, float curValue, float minValue, float maxValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, GUILayout.Width(80));
        curValue = GUILayout.HorizontalSlider(curValue, minValue, maxValue, GUILayout.Width(100));
        var intStr = GUILayout.TextField(curValue.ToString("F"), GUILayout.Width(50));
        if (int.TryParse(intStr, out var value))
        {
            curValue = value;
        }
        GUILayout.EndHorizontal();
        curValue = Mathf.Clamp(curValue, minValue, maxValue);
        return curValue;
    }
}