using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ExtrusionUI.Toolkits.Wpf
{
    /// <summary>
    ///     Helper methods for WPF.
    /// </summary>
    public static class WpfHelper
    {
        /// <summary>
        ///     Gets DPI scaling for given visual. This is typically around 1 for 96 ppi.
        /// </summary>
        public static Vector GetDpiScaling(Visual visual)
        {
            var presentationSource = PresentationSource.FromVisual(visual);

            if (presentationSource?.CompositionTarget == null)
                throw new NullReferenceException(nameof(presentationSource));

            var m = presentationSource.CompositionTarget.TransformToDevice;

            var dx = m.M11;
            var dy = m.M22;

            return new Vector(dx, dy);
        }

        /// <summary>
        /// Gets DPI for given visual.
        /// </summary>
        public static Vector GetDpi(Visual visual)
        {
            return GetDpiScaling(visual) * 96.0;
        }

        /// <summary>
        /// Gets DPI scaling of the main application window.
        /// </summary>
        /// <returns></returns>
        public static Vector DpiScaling
        {
            get
            {
                return GetDpiScaling(Application.Current.MainWindow);
            }
        }

        /// <summary>
        /// Gets DPI scaling of the main application window.
        /// </summary>
        /// <returns></returns>
        public static Vector Dpi
        {
            get
            {
                return DpiScaling * 96.0;
            }
        }

        /// <summary>
        ///     Finds first parent anchestor of type T
        /// </summary>
        /// <typeparam name="T">Anchestor Type</typeparam>
        /// <param name="current">Original source (descendant)</param>
        /// <returns>List of DependencyObjects</returns>
        public static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            if (current == null)
                return null;

            do
            {
                var anchestor = current as T;
                if (anchestor != null)
                {
                    return anchestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        /// <summary>
        ///     Finds all descendants of a certain type and optionally filters by property value.
        /// </summary>
        /// <typeparam name="T">Only these types and derived types are found.</typeparam>
        /// <param name="root">Root element of the search</param>
        /// <param name="lookUpProperty">If specified used as filter property which must match lookUpValue</param>
        /// <param name="lookUpValue">Compared value must match with Property</param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject root, DependencyProperty lookUpProperty = null, object lookUpValue = null) where T : DependencyObject
        {
            if (root == null) yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i) as T;
                if (child != null && (lookUpProperty == null || Equals(child.GetValue(lookUpProperty), lookUpValue)))
                    yield return child;

                foreach (var childOfChild in FindVisualChildren<T>(child, lookUpProperty, lookUpValue))
                    yield return childOfChild;
            }
        }

        /// <summary>
        ///     Returns all <see cref="DependencyProperty"/>s of a <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="element">the <see cref="DependencyObject"/> with the <see cref="DependencyProperty"/>s which are returned</param>
        /// <param name="findAttached">true is attached properties (Grid.Row etc.) should also be found</param>
        /// <param name="filter"><see cref="PropertyFilterOptions"/> which is by default <see cref="PropertyFilterOptions.All"/></param>
        /// <returns></returns>
        public static List<DependencyProperty> GetDependencyProperties(DependencyObject element, bool findAttached = false, PropertyFilterOptions filter = PropertyFilterOptions.All)
        {
            var properties = new List<DependencyProperty>();

            foreach (var pd in TypeDescriptor.GetProperties(element, new Attribute[] { new PropertyFilterAttribute(filter) }).OfType<PropertyDescriptor>())
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(pd);
                if (dpd == null) continue;

                if (!findAttached && dpd.IsAttached) continue;
                properties.Add(dpd.DependencyProperty);
            }

            return properties;
        }

        /// <summary>
        ///     Forces update targets for binding to re-evaluate
        /// </summary>
        /// <param name="updateControl">The <see cref="FrameworkElement"/> with bound <see cref="DependencyProperty"/>s</param>
        /// <param name="property">The <see cref="DependencyProperty"/> to update</param>
        public static void UpdateBindingTarget(FrameworkElement updateControl, DependencyProperty property)
        {
            // Updates normal bindings
            updateControl.GetBindingExpression(property)?.UpdateTarget();

            // Updates multi bindings
            var multiBindingExpressions = BindingOperations.GetMultiBindingExpression(updateControl, property)?.BindingExpressions;
            if (multiBindingExpressions == null) return;
            foreach (var binding in multiBindingExpressions)
                binding.UpdateTarget();
        }

        public static string GetDescription(Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            return attribute?.Description;
        }
    }
}
