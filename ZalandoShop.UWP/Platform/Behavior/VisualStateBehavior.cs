using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MetroLog;
using Microsoft.Xaml.Interactivity;

namespace ZalandoShop.UWP.Platform.Behavior
{
    public class VisualStateBehavior : DependencyObject, IBehavior
    {
        private static readonly ILogger Logger = LogManagerFactory.DefaultLogManager.GetLogger(nameof(VisualStateBehavior));
        public bool IgnoreCallback { get; set; }
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(string),
            typeof(VisualStateBehavior),
            new PropertyMetadata(string.Empty, PropertyChangedCallback));

        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string Group { get; set; }

        public FrameworkElement StatefulElement { get; set; }

        public void Attach(DependencyObject associatedObject)
        {
            StatefulElement = (FrameworkElement)associatedObject;
            var visualGroup = VisualStateUtilities.GetVisualStateGroups(StatefulElement).First(group => group.Name == Group);
            visualGroup.CurrentStateChanged += OnVisualStateChanged;
        }

        public void Detach()
        {
            var visualGroup = VisualStateUtilities.GetVisualStateGroups(StatefulElement).First(group => group.Name == Group);
            visualGroup.CurrentStateChanged -= OnVisualStateChanged;
            AssociatedObject = null;
        }

        public DependencyObject AssociatedObject { get; set; }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            IgnoreCallback = true;
            State = e.NewState.Name;
            IgnoreCallback = false;
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behaviour = (VisualStateBehavior)dependencyObject;
            if (behaviour.IgnoreCallback) return;

            if (behaviour.AssociatedObject == null)
            {
                behaviour.AssociatedObject = VisualStateUtilities.FindNearestStatefulControl(behaviour.StatefulElement);
            }

            var element = behaviour.AssociatedObject as Control;
            if (element == null)
            {
                Logger.Warn($"Root control not found for {behaviour.Group} group");
                return;
            }

            var state = dependencyPropertyChangedEventArgs.NewValue as string;
            if (element == null)
                throw new InvalidOperationException("There are not stateful elements");

            if (state == null || !VisualStateManager.GoToState(element, state, false))
                Debug.WriteLine($"Can't go to state: {state}");
            else
            {
                Logger.Info($"State changed to {state}");
            }
        }
    }
}