namespace Gu.Wpf.ValidationScope
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    internal static class BindingHelper
    {
        private static readonly Dictionary<DependencyProperty, PropertyPath> PropertyPaths = new Dictionary<DependencyProperty, PropertyPath>();

        private static readonly MethodInfo CloneMethod = typeof(Binding).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.Any, new[] { typeof(BindingMode) }, null);

        internal static Binding Clone(this Binding binding, BindingMode mode)
        {
            return (Binding)CloneMethod.Invoke(binding, new object[] { mode });
        }

        internal static BindingBuilder Bind(
            this DependencyObject target,
            DependencyProperty targetProperty)
        {
            return new BindingBuilder(target, targetProperty);
        }

        internal static BindingExpression Bind(
            DependencyObject target,
            DependencyProperty targetProperty,
            object source,
            PropertyPath path)
        {
            var binding = new Binding
            {
                Path = path,
                Source = source,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            return (BindingExpression)BindingOperations.SetBinding(target, targetProperty, binding);
        }

        internal static PropertyPath GetPath(DependencyProperty property)
        {
            PropertyPath path;
            if (PropertyPaths.TryGetValue(property, out path))
            {
                return path;
            }

            path = new PropertyPath(property);
            PropertyPaths[property] = path;
            return path;
        }

        internal struct BindingBuilder
        {
            private readonly DependencyObject target;
            private readonly DependencyProperty targetProperty;

            internal BindingBuilder(DependencyObject target, DependencyProperty targetProperty)
            {
                this.target = target;
                this.targetProperty = targetProperty;
            }

            internal BindingExpression OneWayTo(object source, DependencyProperty sourceProperty)
            {
                var sourcePath = GetPath(sourceProperty);
                return this.OneWayTo(source, sourcePath);
            }

            internal BindingExpression TwoWayTo(object source, DependencyProperty sourceProperty)
            {
                var sourcePath = GetPath(sourceProperty);
                var binding = new Binding
                {
                    Source = source,
                    Path = sourcePath,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };

                return (BindingExpression)BindingOperations.SetBinding(this.target, this.targetProperty, binding);
            }

            internal BindingExpression OneWayTo(object source)
            {
                var binding = new Binding
                {
                    Source = source,
                    Mode = BindingMode.OneWay,
                };

                return (BindingExpression)BindingOperations.SetBinding(this.target, this.targetProperty, binding);
            }

            internal BindingExpression OneWayTo(object source, PropertyPath sourcePath)
            {
                var binding = new Binding
                {
                    Path = sourcePath,
                    Source = source,
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };

                return (BindingExpression)BindingOperations.SetBinding(this.target, this.targetProperty, binding);
            }
        }
    }
}