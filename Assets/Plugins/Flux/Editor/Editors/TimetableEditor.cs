using System;
using System.Globalization;
using System.Linq;
using Flux.Feedbacks;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Flux.Editor
{
    public class TimetableEditor : EditorWindow
    {
        #region Nested Types

        private enum Interaction
        {
            None = 0,
            DragLeft = 1,
            DragRight = 2,
            Slide = 4,
        }
        #endregion
        
        //---[Drawing data]---------------------------------------------------------------------------------------------/
        
        private Color black;
        private Color softBlack;
        private Color darkGrey;
        private Color midGrey;
        private Color grey;
        
        private GUIStyle inactiveStyle;
        private GUIStyle activeStyle;
        private GUIStyle interactiveStyle;
        
        //---[Layout data]----------------------------------------------------------------------------------------------/
        
        private const float dragHandle = 7.5f;
        private const float scrollPadding = 13.0f;
        
        private const float timeStep = 0.025f;
        private const int timeMark = 4;
        
        private const float padding = 3.0f;
        private const float lineWidth = 0.5f;
        private const float timelineHeight = 50.0f;
        private const float propertyWidth = 300.0f;
        private Rect windowRect => position;
        
        private Rect propertyRect;
        private Rect timelineRect;
        private Rect contentRect;
        private Vector2 scrollContent;
        
        //---[Behaviour data]-------------------------------------------------------------------------------------------/
        
        private GenericMenu menu;

        private SerializedObject serializedObject;
        private SerializedProperty arrayProperty;
        private bool hasChanged;

        private Vector2 cachedPosition;
        private TypeSearchWindow searchWindow;
        
        private Interaction interaction;
        private int activeSegmentIndex;
        private int interactiveSegmentIndex;
        
        private bool drawLine;
        private float lineX;
        
        //---[Initialization]-------------------------------------------------------------------------------------------/
        
        public void Initialize(SerializedProperty property)
        {
            serializedObject = new SerializedObject(property.serializedObject.targetObject);
            arrayProperty = serializedObject.FindProperty(property.propertyPath);
            arrayProperty.Next(true);

            black = "#191919".ToColor();
            softBlack = "#272727".ToColor();
            darkGrey = "#2D2D2D".ToColor();
            midGrey = "#353535".ToColor();
            grey = "#383838".ToColor();
            
            inactiveStyle = new GUIStyle("flow node 1") {alignment = TextAnchor.MiddleCenter, clipping = TextClipping.Clip};
            activeStyle = new GUIStyle("flow node 5") {alignment = TextAnchor.MiddleCenter, clipping = TextClipping.Clip};
            interactiveStyle = new GUIStyle("flow node 5 on") {alignment = TextAnchor.MiddleCenter, clipping = TextClipping.Clip};
            
            activeSegmentIndex = -1;
            interactiveSegmentIndex = -1;
            
            searchWindow = ScriptableObject.CreateInstance<TypeSearchWindow>();
            searchWindow.Initialize("Create segment", typeof(Segment), this);
            searchWindow.onSelectEntry += AddSegment;
            
            menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add"), false, () => SearchWindow.Open(new SearchWindowContext(cachedPosition), searchWindow));
            menu.AddItem(new GUIContent("Remove"), false, RemoveSegment);
            
            minSize = new Vector2(850.0f, 500.0f);
            Undo.undoRedoPerformed += OnUndo;
        }
        void OnDestroy() => Undo.undoRedoPerformed -= OnUndo;
        
        void OnUndo()
        {
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
            
            serializedObject?.Update();
            Repaint();
        }

        private void OnFocus() => serializedObject?.Update();
        void OnLostFocus()
        {
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }

        //---[Drawing]--------------------------------------------------------------------------------------------------/
        
        void OnGUI()
        {
            if (UnityEngine.Event.current.OnMouseDown(1))
            {
                cachedPosition = windowRect.position + UnityEngine.Event.current.mousePosition;
                menu.ShowAsContext();
            }

            propertyRect = new Rect(Vector2.zero, new Vector2(propertyWidth, windowRect.height));
            timelineRect = new Rect(new Vector2(propertyRect.xMax, 0.0f), new Vector2(windowRect.width - propertyWidth, timelineHeight));
            contentRect = new Rect(new Vector2(propertyRect.xMax, timelineRect.yMax), new Vector2(timelineRect.width, windowRect.height - timelineHeight));

            DrawContent(contentRect);
            DrawTimeline(timelineRect);
            DrawProperties(propertyRect);
            
            if (hasChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
                
                hasChanged = false;
            }
            
            if (UnityEngine.Event.current.OnMouseUp(0) && interaction != Interaction.None)
            {
                interaction = Interaction.None;
                interactiveSegmentIndex = -1;

                drawLine = false;
                hasChanged = true;
            }
            
            if (drawLine)
            {
                var lineRect = new Rect(new Vector2(lineX - 1.0f, contentRect.yMin), new Vector2(1.0f, contentRect.height));
                EditorGUI.DrawRect(lineRect, Color.white.SetAlpha(0.25f));
                
                var playHeadRect = new Rect(new Vector2(lineX - 8.0f, contentRect.yMin - 6.0f), Vector2.one);
                var style = (GUIStyle)"MeTransPlayHead";
                GUI.Box(playHeadRect, GUIContent.none, style);
            }
        }
        
        //------<Properties>--------------------------------------------------------------------------------------------/

        private void DrawProperties(Rect rect)
        {
            EditorGUI.DrawRect(rect, grey);
            rect.DrawRightBorder(lineWidth * 2.0f, black);
            
            if (activeSegmentIndex == -1) return;
            
            EditorGUI.BeginChangeCheck();
            
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            var spacing = EditorGUIUtility.singleLineHeight + 4.0f;
            
            var subRect = rect.Stretch(-8.0f, -8.0f, -8.0f, -8.0f);
            subRect.height = EditorGUIUtility.singleLineHeight;

            var property = arrayProperty.GetArrayElementAtIndex(activeSegmentIndex);
            var name = property.GetName();
            
            var holdingRect = new Rect(rect.position, new Vector2(rect.width, spacing + 8.0f));
            EditorGUI.DrawRect(holdingRect, midGrey);
            holdingRect.DrawBottomBorder(lineWidth, black);
            holdingRect.DrawRightBorder(lineWidth * 2.0f, black);
            
            EditorGUI.LabelField(subRect, new GUIContent(GetNameFor(activeSegmentIndex, property)), EditorStyles.boldLabel);

            subRect.y += spacing + 8.0f;
            property.Next(true);
            
            var span = property.vector2Value;
            var splitRect = subRect.Split(4.0f);
            
            Undo.RecordObject(serializedObject.targetObject, $"{property.propertyPath} - Setting X");
            span.x = EditorGUI.FloatField(splitRect.left, GUIContent.none, span.x);
            span.x = Mathf.Clamp(span.x, 0.0f, span.y - 0.05f);
            
            Undo.RecordObject(serializedObject.targetObject, $"{property.propertyPath} - Setting Y");
            span.y = EditorGUI.FloatField(splitRect.right, GUIContent.none, span.y);
            span.y = Mathf.Clamp(span.y, span.x + 0.05f, 1.0f);
            
            property.vector2Value = span;
            
            subRect.y += spacing;
            property.Next(false);
            
            Undo.RecordObject(serializedObject.targetObject, $"{property.propertyPath} - Setting value");
            property.animationCurveValue = EditorGUI.CurveField(subRect, GUIContent.none, property.animationCurveValue);
            
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = subRect.width * 0.35f;

            while (property.NextVisible(false))
            {
                if (property.GetParentName() != name) break;

                subRect.y += subRect.height + 4.0f;
                Undo.RecordObject(serializedObject.targetObject, $"{property.propertyPath} - Setting value");
                
                if (property.TryGetDrawer(out var drawer))
                {
                    var label = new GUIContent(property.displayName);
                    subRect.height = property.GetHeight(label);
                    drawer.OnGUI(subRect, property, label);
                }
                else
                {
                    var drawCopy = property.Copy();
                    
                    subRect.height = EditorGUI.GetPropertyHeight(drawCopy);
                    EditorGUI.PropertyField(subRect, drawCopy);
                }
            }

            if (EditorGUI.EndChangeCheck()) hasChanged = true;
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }
        
        //------<Timeline>----------------------------------------------------------------------------------------------/

        private void DrawTimeline(Rect rect)
        {
            EditorGUI.DrawRect(rect, darkGrey);
            
            var scrollPaddingRect = new Rect(new Vector2(rect.xMax - scrollPadding, rect.yMin), new Vector2(scrollPadding, rect.height));
            EditorGUI.DrawRect(scrollPaddingRect, midGrey);
            scrollPaddingRect.DrawBottomBorder(lineWidth, softBlack);
            
            rect.width -= scrollPadding;
            rect.DrawRightBorder(lineWidth, black);
            rect.DrawBottomBorder(lineWidth, black);
            
            var roundedDuration = Mathf.CeilToInt(1.0f / timeStep);
            var horizontalStep = rect.width * timeStep;
            
            for (var i = 0; i < roundedDuration; i++)
            {
                var width = 1.5f;
                var height = timelineHeight * 0.25f;

                var x = rect.xMin + horizontalStep * i;
                x -= width * 0.5f;
                
                if (i % timeMark == 0 && i != 0)
                {
                    width = 2.5f;
                    height = timelineHeight * 0.4f;

                    var time = timeStep * i;
                    var labelContent = new GUIContent(time.ToString(CultureInfo.InvariantCulture));
                    
                    var labelSize = EditorStyles.label.CalcSize(labelContent);
                    var labelPosition = new Vector2(x - labelSize.x * 0.5f, rect.yMax - (height + 17.5f));
                    var labelRect = new Rect(labelPosition, labelSize);
                    EditorGUI.LabelField(labelRect, labelContent);
                }

                var position = new Vector2(x - width * 0.5f, rect.yMax - height);
                EditorGUI.DrawRect(new Rect(position, new Vector2(width, height)), black);
            }
        }

        //------<Content>-----------------------------------------------------------------------------------------------/
        
        private void DrawContent(Rect rect)
        {
            EditorGUI.DrawRect(rect, grey);
            
            var segmentHeight = 75.0f;
            var spacing = 20.0f;

            var height = segmentHeight * arrayProperty.arraySize + spacing * (arrayProperty.arraySize - 1) + spacing * 0.5f;
            var totalRect = new Rect(rect.position, new Vector2(rect.width - scrollPadding, height));
            
            scrollContent = GUI.BeginScrollView(rect, scrollContent, totalRect, false, true);
            rect.width -= scrollPadding;

            var baseRect = new Rect(new Vector2(rect.xMin, rect.yMin + spacing * 0.5f), new Vector2(rect.width, segmentHeight));
            var divisions = Mathf.RoundToInt(rect.width / 15.0f);
            
            for (var i = 0; i < arrayProperty.arraySize; i++)
            {
                var segmentRect = new Rect(new Vector2(rect.xMin, baseRect.yMin + (segmentHeight + spacing) * i), baseRect.size);
                var segmentProperty = arrayProperty.GetArrayElementAtIndex(i);
                
                DrawSegment(i, segmentProperty, segmentRect, darkGrey);

                var spacingRect = new Rect(new Vector2(segmentRect.xMin, segmentRect.yMax), new Vector2(rect.width, spacing));
                DrawDottedLine(spacingRect, divisions, 0.35f, 1.0f, black);
            }
            
            if (interaction != Interaction.None) HandleInteraction(rect);
            GUI.EndScrollView();
        }
        private void DrawSegment(int index, SerializedProperty segmentProperty, Rect rect, Color backgroundColor)
        {
            EditorGUI.DrawRect(rect, backgroundColor);
            rect.DrawTopBorder(lineWidth, softBlack);
            rect.DrawBottomBorder(lineWidth, softBlack);

            var name = GetNameFor(index, segmentProperty);
            segmentProperty.Next(true);
            var span = segmentProperty.vector2Value;
            
            var position = new Vector2(rect.x + rect.width * span.x, rect.yMin);
            var size = new Vector2(rect.width * (span.y - span.x), rect.height);
            var controlRect = new Rect(position, size);

            var style = inactiveStyle;
            if (index == activeSegmentIndex) style = activeStyle;
            if (index == interactiveSegmentIndex) style = interactiveStyle;

            GUI.color = Color.white.SetAlpha(0.8f);
            GUI.Box(controlRect, new GUIContent(name), style);
            GUI.color = Color.white;
            
            if (interaction != Interaction.None) return;

            var leftDragRect = new Rect(new Vector2(controlRect.xMin - dragHandle * 0.5f, controlRect.yMin), new Vector2(dragHandle, controlRect.height));
            if (TryBeginInteractionFor(index, leftDragRect, Interaction.DragLeft, MouseCursor.ResizeHorizontal))
            {
                drawLine = true;
                lineX = rect.xMin + rect.width * span.x;
                
                return;
            }
            
            var rightDragRect = new Rect(new Vector2(controlRect.xMax - dragHandle * 0.5f,  leftDragRect.yMin),  leftDragRect.size);
            if (TryBeginInteractionFor(index, rightDragRect, Interaction.DragRight, MouseCursor.ResizeHorizontal))
            {
                drawLine = true;
                lineX = rect.xMin + rect.width * span.y;
                
                return;
            }
            
            controlRect.xMin += padding;
            controlRect.width -= padding;
            TryBeginInteractionFor(index, controlRect, Interaction.Slide, MouseCursor.Pan);
        }
        
        //---[Interactivity]--------------------------------------------------------------------------------------------/

        private void HandleInteraction(Rect rect)
        {
            if (UnityEngine.Event.current.OnMouseMoveDrag())
            {
                var draggedSegmentProperty = arrayProperty.GetArrayElementAtIndex(interactiveSegmentIndex);
                draggedSegmentProperty.Next(true);
                var span = draggedSegmentProperty.vector2Value;
                
                if (interaction == Interaction.Slide)
                {
                    var ratio = UnityEngine.Event.current.delta.x / rect.width;

                    if (span.x + ratio < 0.0f)
                    {
                        span.x = 0.0f;
                        return;
                    }
                    else if (span.y + ratio > 1.0f)
                    {
                        span.y = 1.0f;
                        return;
                    }
                    
                    span.x += ratio;
                    span.y += ratio;
                }
                else
                {
                    drawLine = true;
                    var position = UnityEngine.Event.current.mousePosition - rect.position;    
                    
                    float ratio;
                    if (UnityEngine.Event.current.control)
                    {
                        var step = rect.width * timeStep;
                        var roundedValue = Mathf.Round(position.x / step) * step;
                        ratio = Mathf.Clamp01(roundedValue / rect.width);
                    }
                    else ratio = Mathf.Clamp01(position.x / rect.width);

                    if (interaction == Interaction.DragRight && ratio > span.x + 0.05f)
                    {
                        span.y = ratio;
                        lineX = rect.xMin + rect.width * span.y;
                    }
                    else if (interaction == Interaction.DragLeft && ratio < span.y - 0.05f)
                    {
                        span.x = ratio;
                        lineX = rect.xMin + rect.width * span.x;
                    }
                }
                
                draggedSegmentProperty.vector2Value = span;
                hasChanged = true;
            }
        }
        private bool TryBeginInteractionFor(int index, Rect rect, Interaction interaction, MouseCursor cursor)
        {
            EditorGUIUtility.AddCursorRect(rect, cursor);

            if (UnityEngine.Event.current.OnMouseDown(rect, 0))
            {
                this.interaction = interaction;

                activeSegmentIndex = index;
                interactiveSegmentIndex = index;
                
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive, rect);;
                GUIUtility.keyboardControl = 0;
                
                return true;
            }

            return false;
        }

        //---[Callbacks]------------------------------------------------------------------------------------------------/
        
        void AddSegment(object userData, Vector2 mousePosition)
        {
            var subProperty = arrayProperty.NewElementAtEnd();
            subProperty.managedReferenceValue = (Segment)Activator.CreateInstance((Type) userData);

            arrayProperty.serializedObject.ApplyModifiedProperties();
        }
        void RemoveSegment()
        {
            if (activeSegmentIndex < 0) return;
            
            arrayProperty.DeleteArrayElementAtIndex(activeSegmentIndex);
            arrayProperty.serializedObject.ApplyModifiedProperties();

            activeSegmentIndex = -1;
            interactiveSegmentIndex = -1;
        }
        
        //---[Utilities]------------------------------------------------------------------------------------------------/

        private void DrawDottedLine(Rect rect, int divisions, float spacing, float thickness, Color color)
        {
            rect = new Rect(new Vector2(rect.xMin, rect.yMin + rect.height * 0.5f - thickness * 0.5f), new Vector2(rect.width, thickness));

            var width = rect.width / divisions;
            var separation = width * spacing;
            width -= separation;
            
            var step = new Rect(rect.position, new Vector2(width, thickness));
            for (var i = 0; i <= divisions; i++)
            {
                EditorGUI.DrawRect(step, color);
                step.x += width + separation;
            }
        }
        private string GetNameFor(int index, SerializedProperty property)
        {
            index++;
            var prefix = index > 10 ? index.ToString() : $"0{index}";
            
            var type = property.managedReferenceFullTypename.Split(' ').Last();
            if (type.Contains('.')) type = type.Split('.').Last();
            
            return $"{prefix} - {type}";
        }
    }
}