﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BossMod
{
    // attribute that specifies how config node should be shown in the UI
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigDisplayAttribute : Attribute
    {
        public string? Name;
        public int Order;
        public Type? Parent;
    }

    // attribute that specifies how config node field or enumeration value is shown in the UI
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyDisplayAttribute : Attribute
    {
        public string Label;

        public PropertyDisplayAttribute(string label)
        {
            Label = label;
        }
    }

    // attribute that specifies slider should be used for displaying property
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertySliderAttribute : Attribute
    {
        public float Speed = 1;
        public float Min;
        public float Max;
        public bool Logarithmic;

        public PropertySliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    // base class for configuration nodes
    public abstract class ConfigNode
    {
        public event EventHandler? Modified;

        // notify that configuration node was modified; should be called by derived classes whenever they make any modifications
        // implementation dispatches modification event and forwards request to parent
        // root should subscribe to modification event to save updated configuration
        public void NotifyModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }

        // draw custom contents; override this for complex config nodes
        public virtual void DrawCustom(UITree tree, WorldState ws) { }

        // deserialize fields from json; default implementation should work fine for most cases
        public virtual void Deserialize(JObject j, JsonSerializer ser)
        {
            var t = GetType();
            foreach (var (f, data) in j)
            {
                var field = t.GetField(f);
                if (field == null)
                    continue;

                var value = data?.ToObject(field.FieldType, ser);
                if (value == null)
                    continue;

                field.SetValue(this, value);
            }
        }

        // serialize node to json; default implementation should work fine for most cases
        public virtual JObject Serialize(JsonSerializer ser)
        {
            return JObject.FromObject(this, ser);
        }
    }
}
