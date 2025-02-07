﻿using ImGuiNET;
using System;
using System.Collections.Generic;

namespace BossMod
{
    public class UITree
    {
        public struct NodeProperties
        {
            public string Text;
            public bool Leaf;
            public uint Color;

            public NodeProperties(string text, bool leaf = false, uint color = 0xffffffff)
            {
                Text = text;
                Leaf = leaf;
                Color = color;
            }
        }

        private uint _selectedID;

        // contains 0 elements (if node is closed) or single null (if node is opened)
        // expected usage is 'foreach (_ in Node(...)) { draw subnodes... }'
        public IEnumerable<object?> Node(string text, bool leaf = false, uint color = 0xffffffff, Action? contextMenu = null, Action? doubleClick = null)
        {
            if (RawNode(text, leaf, color, contextMenu, doubleClick))
            {
                yield return null;
                ImGui.TreePop();
            }
            ImGui.PopID();
        }

        // draw a node for each element in collection
        public IEnumerable<T> Nodes<T>(IEnumerable<T> collection, Func<T, NodeProperties> map, Action<T>? contextMenu = null, Action<T>? doubleClick = null)
        {
            foreach (var t in collection)
            {
                var props = map(t);
                if (RawNode(props.Text, props.Leaf, props.Color, contextMenu != null ? () => contextMenu(t) : null, doubleClick != null ? () => doubleClick(t) : null))
                {
                    yield return t;
                    ImGui.TreePop();
                }
                ImGui.PopID();
            }
        }

        public void LeafNode(string text, uint color = 0xffffffff, Action? contextMenu = null, Action? doubleClick = null)
        {
            if (RawNode(text, true, color, contextMenu, doubleClick))
                ImGui.TreePop();
            ImGui.PopID();
        }

        // draw leaf nodes for each element in collection
        public void LeafNodes<T>(IEnumerable<T> collection, Func<T, string> map, Action<T>? contextMenu = null, Action<T>? doubleClick = null)
        {
            foreach (var t in collection)
            {
                if (RawNode(map(t), true, 0xffffffff, contextMenu != null ? () => contextMenu(t) : null, doubleClick != null ? () => doubleClick(t) : null))
                    ImGui.TreePop();
                ImGui.PopID();
            }
        }

        // handle selection & id scopes
        private bool RawNode(string text, bool leaf, uint color, Action? contextMenu, Action? doubleClick)
        {
            var id = ImGui.GetID(text);
            var flags = ImGuiTreeNodeFlags.None;
            if (id == _selectedID)
                flags |= ImGuiTreeNodeFlags.Selected;
            if (leaf)
                flags |= ImGuiTreeNodeFlags.Leaf;

            ImGui.PushID((int)id);
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            bool open = ImGui.TreeNodeEx(text, flags);
            ImGui.PopStyleColor();
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                _selectedID = id;
            if (doubleClick != null && ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                doubleClick();
            if (contextMenu != null && ImGui.BeginPopupContextItem())
            {
                contextMenu();
                ImGui.EndPopup();
            }
            return open;
        }
    }
}
