using Godot;
using System;

public partial class Highlight : Control
{
    SelectableField selectedField;

    public void SetSelectedField(SelectableField node)
    {
        selectedField = node;
        Size = node.Size + new Vector2(10, 10);
        SetPositionToFollowingNode();
    }

    public SelectableField GetSelectedField()
    {
        return selectedField;
    }

    private void SetPositionToFollowingNode()
    {
        GlobalPosition = selectedField.GlobalPosition - new Vector2(10, 8);
    }

    public void UnsetFollowingNode()
    {
        selectedField = null;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (selectedField != null)
        {
            SetPositionToFollowingNode();
        }
    }
}
