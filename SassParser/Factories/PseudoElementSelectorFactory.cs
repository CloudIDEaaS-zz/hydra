using System;
using System.Collections.Generic;

namespace SassParser
{
    public sealed class PseudoElementSelectorFactory
    {
        private static readonly Lazy<PseudoElementSelectorFactory> Lazy = new Lazy<PseudoElementSelectorFactory>(() => new PseudoElementSelectorFactory());
        internal static PseudoElementSelectorFactory Instance => Lazy.Value;

        private PseudoElementSelectorFactory()
        {
        }

        #region Selectors
        private readonly Dictionary<string, ISelector> _selectors =
            new Dictionary<string, ISelector>(StringComparer.OrdinalIgnoreCase)
            {
                //TODO some lack implementation (selection, content, ...)
                // some implementations are dubious (first-line, first-letter, ...)
                {
                    PseudoElementNames.Before,
                    SimpleSelector.PseudoElement(PseudoElementNames.Before, null)
                },
                {
                    PseudoElementNames.After,
                    SimpleSelector.PseudoElement(PseudoElementNames.After, null)
                },
                {
                    PseudoElementNames.Selection,
                    SimpleSelector.PseudoElement(PseudoElementNames.Selection, null)
                },
                {
                    PseudoElementNames.FirstLine,
                    SimpleSelector.PseudoElement(PseudoElementNames.FirstLine, null)
                },
                {
                    PseudoElementNames.FirstLetter,
                    SimpleSelector.PseudoElement( PseudoElementNames.FirstLetter, null)
                },
                {
                    PseudoElementNames.Content, 
                    SimpleSelector.PseudoElement( PseudoElementNames.Content, null)
                }
            };
        #endregion

        public ISelector Create(string name)
        {

            return _selectors.TryGetValue(name, out ISelector selector) ? selector : null;
        }
    }
}