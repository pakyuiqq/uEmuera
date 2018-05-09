﻿using UnityEngine;
using UnityEngine.UI;
using MinorShift.Emuera;
using MinorShift._Library;

public class EmueraMain : MonoBehaviour
{
    public void Run()
    {
        EmueraThread.instance.Start(debug, use_coroutine);
        working_ = true;
    }

    void Start()
    {
        canvas_ = GetComponent<Canvas>();
        canvas_scaler_ = GetComponent<CanvasScaler>();
        default_resolution_ = canvas_scaler_.referenceResolution;

        size_delta_ = (transform as RectTransform).sizeDelta;
        float w = size_delta_.x;
        float h = size_delta_.y;
        size_delta_.x = Mathf.Max(w, h);
        size_delta_.y = Mathf.Min(w, h);
    }

    void Update()
    {
        if(!working_)
            return;

        if(dirty_flag_)
        {
            EmueraContent.instance.SetDirty();
            dirty_flag_ = false;
        }

#if UNITY_EDITOR
        if(Input.anyKey)
            EmueraThread.instance.Input("", false);
        if(!string.IsNullOrEmpty(input))
        {
            EmueraThread.instance.Input(input, true);
            input = null;
        }
        if(MinorShift.Emuera.GlobalStatic.Console != null)
            console_lines = MinorShift.Emuera.GlobalStatic.Console.GetDisplayLinesCount();
#endif
        if(GlobalStatic.MainWindow != null)
            GlobalStatic.MainWindow.Update();

        if(last_orientation_ != Input.deviceOrientation)
        {
            last_orientation_ = Input.deviceOrientation;
            dirty_flag_ = true;
        }
        ApplyScale();
    }
    bool working_ = false;

    public bool debug = false;
    public bool use_coroutine = false;

#if UNITY_EDITOR
    public string input;
    public int console_lines;
#endif
    Canvas canvas_ = null;
    CanvasScaler canvas_scaler_ = null;

    public float default_width { get { return default_resolution_.x; } }
    public float default_height { get { return default_resolution_.y; } }
    public float default_display_width { get { return size_delta_.x; } }
    public float default_display_height { get { return size_delta_.y; } }

    public float match_factor { get { return canvas_scaler_.matchWidthOrHeight; } }
    Vector2 default_resolution_;
    Vector2 size_delta_;

    public void OnePerOne()
    {
        SetScaleValue(1);
    }
    public void AutoFit()
    {
        var emuera_width = Config.DrawableWidth + 3;
        var value = 1.0f;
        if(Screen.width > Screen.height)
            value = default_display_width / emuera_width;
        else
            value = default_display_height / emuera_width;
        SetScaleValue(value);
    }
    public void SetScaleValue(float value)
    {
        next_scale_value_ = value;
    }
    void ApplyScale()
    {
        if(next_scale_value_ == last_scale_value_)
            return;

        last_scale_value_ = next_scale_value_;
        if(last_scale_value_ < 0)
            last_scale_value_ = 1;

        canvas_scaler_.referenceResolution = default_resolution_ / last_scale_value_;
        dirty_flag_ = true;
    }
    public float scale_value { get { return next_scale_value_; } }
    bool dirty_flag_ = false;
    float next_scale_value_ = 1;
    float last_scale_value_ = -1.0f;

    DeviceOrientation last_orientation_ = DeviceOrientation.Unknown;
}
