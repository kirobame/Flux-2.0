using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Label = UnityEngine.UIElements.Label;

namespace Flux.Editor
{
    public class TimetableEditor : EditorWindow
    {
        #region Nested Types

        public class Segment
        {
            public Vector2 span;
        }

        #endregion
        
        private Color black;
        private Color softBlack;
        private Color darkGrey;
        private Color midGrey;
        private Color grey;

        private const float padding = 3.0f;
        private const float lineWidth = 0.5f;
        private const int timeStep = 5;
        private const float timelineHeight = 50.0f;
        private const float propertyWidth = 200.0f;
        private Rect windowRect => position;
        
        private GenericMenu menu;
        
        private Rect propertyRect;
        private Rect timelineRect;
        private Rect contentRect;

        private const float scrollPadding = 13.0f;
        private Vector2 scrollContent;

        private Segment[] segments;

        private bool isHovering;
        private bool isDragging;
        private bool isDragOnRight;
        private Segment draggedSegment;
        
        void OnEnable()
        {
            black = "#191919".ToColor();
            softBlack = "#272727".ToColor();
            darkGrey = "#2D2D2D".ToColor();
            midGrey = "#353535".ToColor();
            grey = "#383838".ToColor();
            
            minSize = new Vector2(500.0f, 550.0f);
            
            menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add effect"), false, () =>
            {
                
            });
            
            segments = new Segment[]
            {
                new Segment() { span = new Vector2(0.25f, 0.75f) },
                new Segment() { span = new Vector2(0.0f, 0.65f) },
                new Segment() { span = new Vector2(0.15f, 0.85f) },
                new Segment() { span = new Vector2(0.45f, 0.65f) },
                new Segment() { span = new Vector2(0.65f, 1.0f) },
                new Segment() { span = new Vector2(0.0f, 1.0f) },
                new Segment() { span = new Vector2(0.25f, 0.75f) },
            };
        }
        
        void OnInspectorUpdate() => Repaint();
        
        void OnGUI()
        {
            if (Event.current.OnMouseDown(1)) menu.ShowAsContext();

            propertyRect = new Rect(Vector2.zero, new Vector2(propertyWidth, windowRect.height));
            timelineRect = new Rect(new Vector2(propertyRect.xMax, 0.0f), new Vector2(windowRect.width - propertyWidth, timelineHeight));
            contentRect = new Rect(new Vector2(propertyRect.xMax, timelineRect.yMax), new Vector2(timelineRect.width, windowRect.height - timelineHeight));

            if (Event.current.OnMouseUp(0))
            {
                Debug.Log("STOP");
                
                isDragging = false;
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.Arrow);
            }
            
            DrawContent(contentRect);
            DrawTimeline(timelineRect);
            DrawProperties(propertyRect);
        }

        private void DrawProperties(Rect rect)
        {
            EditorGUI.DrawRect(rect, grey);
            rect.DrawRightBorder(lineWidth * 2.0f, black);
        }

        private void DrawTimeline(Rect rect)
        {
            EditorGUI.DrawRect(rect, darkGrey);
            rect.DrawBottomBorder(lineWidth, black);
            
            var scrollPaddingRect = new Rect(new Vector2(rect.xMax - scrollPadding, rect.yMin), new Vector2(scrollPadding, rect.height + 1.0f));
            EditorGUI.DrawRect(scrollPaddingRect, midGrey);
            scrollPaddingRect.DrawLeftBorder(lineWidth * 2.5f, softBlack);
            
            rect.width -= scrollPadding;

            var duration = 32.25f;
            var roundedDuration = Mathf.CeilToInt(duration);
            var horizontalStep = rect.width / duration;
            
            for (var i = 0; i < roundedDuration; i++)
            {
                var width = 1.5f;
                var height = timelineHeight * 0.25f;

                var x = rect.xMin + horizontalStep * i;
                x -= width * 0.5f;
                
                if (i % timeStep == 0 && i != 0)
                {
                    width = 2.5f;
                    height = timelineHeight * 0.4f;

                    var labelContent = new GUIContent(i < 10 ? $"0{i}" : i.ToString());
                    var labelSize = EditorStyles.label.CalcSize(labelContent);
                    var labelPosition = new Vector2(x - labelSize.x * 0.5f, rect.yMax - (height + 17.5f));
                    var labelRect = new Rect(labelPosition, labelSize);
                    EditorGUI.LabelField(labelRect, labelContent);
                }

                var position = new Vector2(x - width * 0.5f, rect.yMax - height);
                EditorGUI.DrawRect(new Rect(position, new Vector2(width, height)), black);
            }
        }

        private void DrawContent(Rect rect)
        {
            EditorGUI.DrawRect(rect, grey);
            
            var settingsRect = new Rect(rect.position, new Vector2(rect.width, EditorGUIUtility.singleLineHeight + padding));
            var spacing = 5.0f;
            var segmentHeight = 175.0f;

            var totalRect = new Rect(rect.position, new Vector2(rect.width - scrollPadding, settingsRect.height + segmentHeight * segments.Length + spacing));
            scrollContent = GUI.BeginScrollView(rect, scrollContent, totalRect, false, true);

            settingsRect.width -= scrollPadding;
            rect.width -= scrollPadding;

            if (isDragging)
            {
                if (Event.current.OnMouseMoveDrag())
                {
                    var position = Event.current.mousePosition - rect.position;
                    float ratio;
                    
                    if (Event.current.control)
                    {
                        var step = rect.width / 32.25f;
                        var roundedValue = Mathf.Round(position.x / step) * step;
                        ratio = Mathf.Clamp01(roundedValue / rect.width);
                    }
                    else ratio = Mathf.Clamp01(position.x / rect.width);
                    
                    if (isDragOnRight && ratio > draggedSegment.span.x + 0.1f) draggedSegment.span.y = ratio;
                    else if (!isDragOnRight && ratio < draggedSegment.span.y - 0.1f) draggedSegment.span.x = ratio;
                }
            }

            isHovering = false;
            var baseRect = new Rect(new Vector2(rect.xMin, settingsRect.yMax + spacing), new Vector2(rect.width, segmentHeight));

            settingsRect.y += spacing * 0.275f;
            settingsRect = settingsRect.Stretch(-padding, -padding - 2.5f, -padding, -padding);
            DrawSettings(settingsRect);

            for (var i = 0; i < segments.Length; i++)
            {
                var segmentRect = new Rect(new Vector2(rect.xMin, baseRect.yMin + segmentHeight * i), baseRect.size);
                DrawSegment(segments[i], segmentRect, i % 2 != 0 ? grey : darkGrey);
            }

            if (!isHovering) EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.Arrow);
            GUI.EndScrollView();
        }

        private void DrawSettings(Rect rect)
        {
            EditorGUI.FloatField(rect, new GUIContent("Duration"), 32.25f);
        }
        private void DrawSegment(Segment segment, Rect rect, Color backgroundColor)
        {
            EditorGUI.DrawRect(rect, backgroundColor);
            rect.DrawTopBorder(lineWidth, softBlack);

            var height = rect.height / 3.0f;
            var position = new Vector2(rect.x + rect.width * segment.span.x, rect.yMin + height);
            var size = new Vector2(rect.width * (segment.span.y - segment.span.x), height);
            var controlRect = new Rect(position, size);

            EditorGUI.DrawRect(controlRect, "#4EA0B0".ToColor().SetAlpha(0.85f));
            controlRect.DrawTopBorder(lineWidth, "#C1E5EC".ToColor());
            controlRect.DrawLeftBorder(lineWidth * 2.0f, "#C1E5EC".ToColor());
            controlRect.DrawRightBorder(lineWidth * 2.0f, "#C1E5EC".ToColor());
            controlRect.DrawBottomBorder(lineWidth, "#C1E5EC".ToColor());
            
            if (isDragging) return;
            
            var dragPadding = 7.5f;
            var leftDragRect = new Rect(new Vector2(controlRect.xMin - dragPadding, controlRect.yMin - dragPadding), Vector2.one * (dragPadding * 2.0f));
            leftDragRect.height += height;
            var rightDragRect = new Rect(new Vector2(controlRect.xMax - dragPadding,  leftDragRect.yMin),  leftDragRect.size);
            
            TryBeginDragFor(segment, rightDragRect, true);
            TryBeginDragFor(segment, leftDragRect, false);
        }

        private void TryBeginDragFor(Segment segment, Rect dragRect, bool isRight)
        {
            EditorGUI.DrawRect(dragRect, Color.red.SetAlpha(0.25f));

            if (!isHovering && dragRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.ResizeHorizontal);
                isHovering = true;
            }
            
            if (Event.current.OnMouseDown(dragRect, 0))
            {
                isDragging = true;
                draggedSegment = segment;

                isDragOnRight = isRight;
            }
        }
    }
}