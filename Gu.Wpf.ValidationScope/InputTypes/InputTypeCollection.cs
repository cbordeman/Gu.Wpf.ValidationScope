namespace Gu.Wpf.ValidationScope
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>A collection of controls that will be subscribed to for validation errors.</summary>
    [TypeConverter(typeof(InputTypeCollectionConverter))]
    public sealed class InputTypeCollection : Collection<Type>
    {
        /// <summary>Contains Scope, TextBoxBase, ComboBox, ToggleButton and Slider.</summary>
        public static readonly InputTypeCollection Default = new InputTypeCollection
        {
            typeof(Scope),
            typeof(TextBoxBase),
            typeof(Selector),
            typeof(ToggleButton),
            typeof(Slider),
        };

        public InputTypeCollection()
        {
        }

        public InputTypeCollection(IEnumerable<Type> types)
        {
            this.AddRange(types);
        }

        /// <summary>
        /// Check if <paramref name="type"/> can be an input type.
        /// Valid input types are { Scope, UIElement, ContentElement }.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>True if <paramref name="type"/> is a possible input type.</returns>
        public static bool IsCompatibleType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type == typeof(Scope) ||
                   typeof(UIElement).IsAssignableFrom(type) ||
                   typeof(ContentElement).IsAssignableFrom(type);
        }

        /// <summary>
        /// Check if the collection contains <paramref name="dependencyObject"/> or a type that is a subclass of <paramref name="dependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/>.</param>
        /// <returns>True if <paramref name="dependencyObject"/> is an input type registered for the collection.</returns>
        public bool Contains(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return false;
            }

            return this.Any(x => x.IsInstanceOfType(dependencyObject));
        }

        /// <summary>See <see cref="List{T}.AddRange"/>.</summary>
        /// <param name="types">The <see cref="IEnumerable{Type}"/>.</param>
        public void AddRange(IEnumerable<Type> types)
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var type in types)
            {
                this.Add(type);
            }
        }

        protected override void InsertItem(int index, Type item)
        {
            this.VerifyCompatible(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, Type item)
        {
            this.VerifyCompatible(item);
            base.SetItem(index, item);
        }

        private void VerifyCompatible(Type type)
        {
            if (!IsCompatibleType(type))
            {
                throw new ArgumentException($"Type {type} is not compatible. Must be a type that works with Validation.");
            }
        }
    }
}
