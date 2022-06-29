using System;
using System.Collections.Generic;

namespace SassParser
{
    public sealed class PseudoClassSelectorFactory
    {
        private static readonly Lazy<PseudoClassSelectorFactory> Lazy =
            new Lazy<PseudoClassSelectorFactory>(() =>
                {
                    var factory = new PseudoClassSelectorFactory();
                    Selectors.Add(PseudoElementNames.Before, PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.Before));
                    Selectors.Add(PseudoElementNames.After, PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.After));
                    Selectors.Add(PseudoElementNames.FirstLine, PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.FirstLine));
                    Selectors.Add(PseudoElementNames.FirstLetter, PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.FirstLetter));
                    return factory;
                }
            );

        internal static PseudoClassSelectorFactory Instance => Lazy.Value;

        #region Selectors
        private static readonly Dictionary<string, ISelector> Selectors =
            new Dictionary<string, ISelector>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    PseudoClassNames.Root,
                    SimpleSelector.PseudoClass(PseudoClassNames.Root, null)
                },
                {
                    PseudoClassNames.Scope,
                    SimpleSelector.PseudoClass(PseudoClassNames.Scope, null)
                },
                {
                    PseudoClassNames.OnlyType,
                    SimpleSelector.PseudoClass( PseudoClassNames.OnlyType, null)
                },
                {
                    PseudoClassNames.FirstOfType,
                    SimpleSelector.PseudoClass( PseudoClassNames.FirstOfType, null)
                },
                {
                    PseudoClassNames.LastOfType,
                    SimpleSelector.PseudoClass(  PseudoClassNames.LastOfType, null)
                },
                {
                    PseudoClassNames.OnlyChild,
                    SimpleSelector.PseudoClass( PseudoClassNames.OnlyChild, null)
                },
                {
                    PseudoClassNames.FirstChild,
                    SimpleSelector.PseudoClass( PseudoClassNames.FirstChild, null)
                },
                {
                    PseudoClassNames.LastChild,
                    SimpleSelector.PseudoClass(PseudoClassNames.LastChild, null)
                },
                {
                    PseudoClassNames.Empty,
                    SimpleSelector.PseudoClass(PseudoClassNames.Empty, null)
                },
                {
                    PseudoClassNames.AnyLink,
                    SimpleSelector.PseudoClass(  PseudoClassNames.AnyLink, null)
                },
                {
                    PseudoClassNames.Link, 
                    SimpleSelector.PseudoClass(  PseudoClassNames.Link, null)},
                {
                    PseudoClassNames.Visited,
                    SimpleSelector.PseudoClass(  PseudoClassNames.Visited, null)
                },
                {
                    PseudoClassNames.Active,
                    SimpleSelector.PseudoClass( PseudoClassNames.Active, null)
                },
                {
                    PseudoClassNames.Hover, 
                    SimpleSelector.PseudoClass( PseudoClassNames.Hover, null)
                },
                {
                    PseudoClassNames.Focus, 
                    SimpleSelector.PseudoClass( PseudoClassNames.Focus, null)
                },
                {
                    PseudoClassNames.Target, 
                    SimpleSelector.PseudoClass( PseudoClassNames.Target, null)
                },
                {
                    PseudoClassNames.Enabled,
                    SimpleSelector.PseudoClass( PseudoClassNames.Enabled, null)
                },
                {
                    PseudoClassNames.Disabled,
                    SimpleSelector.PseudoClass( PseudoClassNames.Disabled, null)
                },
                {
                    PseudoClassNames.Default,
                    SimpleSelector.PseudoClass( PseudoClassNames.Default, null)
                },
                {
                    PseudoClassNames.Checked,
                    SimpleSelector.PseudoClass( PseudoClassNames.Checked, null)
                },
                {
                    PseudoClassNames.Indeterminate,
                    SimpleSelector.PseudoClass(  PseudoClassNames.Indeterminate, null)
                },
                {
                    PseudoClassNames.PlaceholderShown,
                    SimpleSelector.PseudoClass(  PseudoClassNames.PlaceholderShown, null)
                },
                {
                    PseudoClassNames.Unchecked,
                    SimpleSelector.PseudoClass( PseudoClassNames.Unchecked, null)
                },
                {
                    PseudoClassNames.Valid, 
                    SimpleSelector.PseudoClass( PseudoClassNames.Valid, null)
                },
                {
                    PseudoClassNames.Invalid,
                    SimpleSelector.PseudoClass(  PseudoClassNames.Invalid, null)
                },
                {
                    PseudoClassNames.Required,
                    SimpleSelector.PseudoClass( PseudoClassNames.Required, null)
                },
                {
                    PseudoClassNames.ReadOnly,
                    SimpleSelector.PseudoClass( PseudoClassNames.ReadOnly, null)
                },
                {
                    PseudoClassNames.ReadWrite,
                    SimpleSelector.PseudoClass( PseudoClassNames.ReadWrite, null)
                },
                {
                    PseudoClassNames.InRange,
                    SimpleSelector.PseudoClass( PseudoClassNames.InRange, null)
                },
                {
                    PseudoClassNames.OutOfRange,
                    SimpleSelector.PseudoClass(  PseudoClassNames.OutOfRange, null)
                },
                {
                    PseudoClassNames.Optional,
                    SimpleSelector.PseudoClass( PseudoClassNames.Optional, null)
                },
                {
                    PseudoClassNames.Shadow, 
                    SimpleSelector.PseudoClass( PseudoClassNames.Shadow, null)
                },
            };
        #endregion

        public ISelector Create(string name)
        {
            if (Selectors.TryGetValue(name, out ISelector selector))
            {
                return selector;
            }

            return null;
        }
    }
}