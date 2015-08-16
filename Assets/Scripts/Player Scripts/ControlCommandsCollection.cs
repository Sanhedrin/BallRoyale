using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct ControlCommandsCollection
{
    public float HorizontalMovement { get; set; }
    public float VerticalMovement { get; set; }
    public bool Jump { get; set; }
    public bool Break { get; set; }

    public bool IsNewInput
    {
        get
        {
            return !this.Equals(default(ControlCommandsCollection));
        }
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", HorizontalMovement, VerticalMovement, Jump, Break);
    }
}
