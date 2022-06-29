﻿using System;
using System.Collections.Generic;

namespace SassParser
{
    internal sealed class SelectorConstructor
    {
        private readonly Stack<Combinator> _combinators;
        private State _state;
        private ISelector _temp;
        private ListSelector _group;
        private ComplexSelector _complex;
        private string _attrName;
        private string _attrValue;
        private string _attrOp;
        private string _attrNs;
        private bool _valid;
        private bool _ready;
        private AttributeSelectorFactory _attributeSelector;
        private PseudoElementSelectorFactory _pseudoElementSelector;
        private PseudoClassSelectorFactory _pseudoClassSelector;

        public SelectorConstructor(AttributeSelectorFactory attributeSelector,
            PseudoClassSelectorFactory pseudoClassSelector, PseudoElementSelectorFactory pseudoElementSelector)
        {
            _combinators = new Stack<Combinator>();
            Reset(attributeSelector, pseudoClassSelector, pseudoElementSelector);
        }

        private enum State : byte
        {
            Data,
            Attribute,
            AttributeOperator,
            AttributeValue,
            AttributeEnd,
            Class,
            PseudoClass,
            PseudoElement
        }
        private static readonly Dictionary<string, Func<SelectorConstructor, FunctionState>> PseudoClassFunctions =
            new Dictionary<string, Func<SelectorConstructor, FunctionState>>(StringComparer.OrdinalIgnoreCase)
            {
                {PseudoClassNames.NthChild, ctx => new ChildFunctionState<FirstChildSelector>(ctx)},
                {PseudoClassNames.NthLastChild, ctx => new ChildFunctionState<LastChildSelector>(ctx)},
                {PseudoClassNames.NthOfType, ctx => new ChildFunctionState<FirstTypeSelector>(ctx, false)},
                {PseudoClassNames.NthLastOfType, ctx => new ChildFunctionState<LastTypeSelector>(ctx, false)},
                {PseudoClassNames.NthColumn, ctx => new ChildFunctionState<FirstColumnSelector>(ctx, false)},
                {PseudoClassNames.NthLastColumn, ctx => new ChildFunctionState<LastColumnSelector>(ctx, false)},
                {PseudoClassNames.Not, ctx => new NotFunctionState(ctx)},
                {PseudoClassNames.Dir, ctx => new DirFunctionState()},
                {PseudoClassNames.Lang, ctx => new LangFunctionState()},
                {PseudoClassNames.Contains, ctx => new ContainsFunctionState()},
                {PseudoClassNames.Has, ctx => new HasFunctionState(ctx)},
                {PseudoClassNames.Matches, ctx => new MatchesFunctionState(ctx)},
                {PseudoClassNames.HostContext, ctx => new HostContextFunctionState(ctx)}
            };
       
        public bool IsValid => _valid && _ready;
        public bool IsNested { get; private set; }

        public ISelector GetResult()
        {
            if (!IsValid)
            {
                var selector = new UnknownSelector(null);
                return selector;
            }

            if (_complex != null)
            {
                _complex.ConcludeSelector(_temp);
                _temp = _complex;
                _complex = null;
            }

            if ((_group == null) || (_group.Length == 0))
            {
                return _temp ?? SimpleSelector.All;
            }

            if ((_temp == null) && (_group.Length == 1))
            {
                return _group[0];
            }

            if (_temp != null)
            {
                _group.Add(_temp);
                _temp = null;
            }
            return _group;
        }

        public void Apply(Token token)
        {
            if (token.Type == TokenType.Comment) { return; }

            switch (_state)
            {
                case State.Data:
                    OnData(token);
                    break;
                case State.Class:
                    OnClass(token);
                    break;
                case State.Attribute:
                    OnAttribute(token);
                    break;
                case State.AttributeOperator:
                    OnAttributeOperator(token);
                    break;
                case State.AttributeValue:
                    OnAttributeValue(token);
                    break;
                case State.AttributeEnd:
                    OnAttributeEnd(token);
                    break;
                case State.PseudoClass:
                    OnPseudoClass(token);
                    break;
                case State.PseudoElement:
                    OnPseudoElement(token);
                    break;
                default:
                    _valid = false;
                    break;
            }
        }

        public SelectorConstructor Reset(AttributeSelectorFactory attributeSelector,
            PseudoClassSelectorFactory pseudoClassSelector, PseudoElementSelectorFactory pseudoElementSelector)
        {
            _attrName = null;
            _attrValue = null;
            _attrNs = null;
            _attrOp = string.Empty;
            _state = State.Data;
            _combinators.Clear();
            _temp = null;
            _group = null;
            _complex = null;
            _valid = true;
            IsNested = false;
            _ready = true;
            _attributeSelector = attributeSelector;
            _pseudoClassSelector = pseudoClassSelector;
            _pseudoElementSelector = pseudoElementSelector;
            return this;
        }

        private void OnData(Token token)
        {
            switch (token.Type)
            {
                case TokenType.SquareBracketOpen:
                    _attrName = null;
                    _attrValue = null;
                    _attrOp = string.Empty;
                    _attrNs = null;
                    _state = State.Attribute;
                    _ready = false;
                    break;
                case TokenType.Colon:
                    _state = State.PseudoClass;
                    _ready = false;
                    break;
                case TokenType.Hash:
                    Insert(SimpleSelector.Id(token.Data, token), token);
                    _ready = true;
                    break;
                case TokenType.Ident:
                    Insert(SimpleSelector.Type(token.Data, token), token);
                    _ready = true;
                    break;
                case TokenType.Whitespace:
                    Insert(Combinator.Descendent);
                    break;
                case TokenType.Delim:
                    OnDelim(token);
                    break;
                case TokenType.Comma:
                    InsertOr(token);
                    _ready = false;
                    break;
                default:
                    _valid = false;
                    break;
            }
        }

        private void OnAttribute(Token token)
        {
            if (token.Type == TokenType.Whitespace)
            {
                return;
            }

            if ((token.Type == TokenType.Ident) || (token.Type == TokenType.String))
            {
                _state = State.AttributeOperator;
                _attrName = token.Data;
            }
            else if ((token.Type == TokenType.Delim) && token.Data.Is(Combinators.Pipe))
            {
                _state = State.Attribute;
                _attrNs = string.Empty;
            }
            else if ((token.Type == TokenType.Delim) && token.Data.Is(Keywords.Asterisk))
            {
                _state = State.AttributeOperator;
                _attrName = token.ToValue();
            }
            else
            {
                _state = State.Data;
                _valid = false;
            }
        }

        private void OnAttributeOperator(Token token)
        {
            if (token.Type == TokenType.Whitespace)
            {
                return;
            }

            if (token.Type == TokenType.SquareBracketClose)
            {
                _state = State.AttributeValue;
                OnAttributeEnd(token);
            }
            else if ((token.Type == TokenType.Match) || (token.Type == TokenType.Delim))
            {
                _state = State.AttributeValue;
                _attrOp = token.ToValue();

                if (_attrOp != Combinators.Pipe)
                {
                    return;
                }
                _attrNs = _attrName;
                _attrName = null;
                _attrOp = string.Empty;
                _state = State.Attribute;
            }
            else
            {
                _state = State.AttributeEnd;
                _valid = false;
            }
        }

        private void OnAttributeValue(Token token)
        {
            if (token.Type == TokenType.Whitespace)
            {
                return;
            }

            if ((token.Type == TokenType.Ident) || (token.Type == TokenType.String) ||
                (token.Type == TokenType.Number))
            {
                _state = State.AttributeEnd;
                _attrValue = token.Data;
            }
            else
            {
                _state = State.Data;
                _valid = false;
            }
        }

        private void OnAttributeEnd(Token token)
        {
            if (token.Type == TokenType.Whitespace)
            {
                return;
            }
            _state = State.Data;
            _ready = true;
            if (token.Type == TokenType.SquareBracketClose)
            {
                var selector = _attributeSelector.Create(_attrOp, _attrName, _attrValue, _attrNs, token);
                Insert(selector, token);
            }
            else
            {
                _valid = false;
            }
        }

        private void OnPseudoClass(Token token)
        {
            _state = State.Data;
            _ready = true;
            switch (token.Type)
            {
                case TokenType.Colon:
                    _state = State.PseudoElement;
                    return;
                case TokenType.Function:
                {
                    var sel = GetPseudoFunction(token as FunctionToken);
                    if (sel != null)
                    {
                        Insert(sel, token);
                        return;
                    }
                }
                    break;
                case TokenType.Ident:
                {
                    var sel = _pseudoClassSelector.Create(token.Data);
                    if (sel != null)
                    {
                        Insert(sel, token);
                        return;
                    }
                }
                    break;
            }
            _valid = false;
        }

        private void OnPseudoElement(Token token)
        {
            _state = State.Data;
            _ready = true;

            if (token.Type == TokenType.Ident)
            {
                var sel = _pseudoElementSelector.Create(token.Data);

                if (sel != null)
                {
                    _valid = _valid && !IsNested;
                    Insert(sel, token);
                    return;
                }
            }

            _valid = false;
        }

        private void OnClass(Token token)
        {
            _state = State.Data;
            _ready = true;
            if (token.Type == TokenType.Ident)
            {
                Insert(SimpleSelector.Class(token.Data, token), token);
            }
            else
            { _valid = false; }

        }

        private void InsertOr(Token token)
        {
            if (_temp == null)
            {
                return;
            }

            if (_group == null)
            {
                _group = new ListSelector(token);
            }

            if (_complex != null)
            {
                _complex.ConcludeSelector(_temp);
                _group.Add(_complex);
                _complex = null;
            }
            else
            {
                _group.Add(_temp);
            }
            _temp = null;
        }

        private void Insert(ISelector selector, Token token)
        {
            if (_temp != null)
            {
                if (_combinators.Count == 0)
                {
                    var compound = _temp as CompoundSelector ?? new CompoundSelector(token) {_temp};

                    compound.Add(selector);
                    _temp = compound;
                }
                else
                {
                    if (_complex == null)
                    {
                        _complex = new ComplexSelector(token);
                    }

                    var combinator = GetCombinator();
                    _complex.AppendSelector(_temp, combinator, token);
                    _temp = selector;
                }
            }
            else
            {
                _combinators.Clear();
                _temp = selector;
            }
        }

        private Combinator GetCombinator()
        {
            while ((_combinators.Count > 1) && (_combinators.Peek() == Combinator.Descendent))
            {
                _combinators.Pop();
            }

            if (_combinators.Count <= 1)
            {
                return _combinators.Pop();
            }

            var last = _combinators.Pop();
            var previous = _combinators.Pop();
            if ((last == Combinator.Child) && (previous == Combinator.Child))
            {
                if ((_combinators.Count == 0) || (_combinators.Peek() != Combinator.Child))
                {
                    last = Combinator.Descendent;
                }
                else if (_combinators.Pop() == Combinator.Child)
                {
                    last = Combinator.Deep;
                }
            }
            else if ((last == Combinator.Namespace) && (previous == Combinator.Namespace))
            {
                last = Combinator.Column;
            }
            else
            {
                _combinators.Push(previous);
            }
            while (_combinators.Count > 0)
            {
                _valid = (_combinators.Pop() == Combinator.Descendent) && _valid;
            }

            return last;
        }

        private void Insert(Combinator combinator)
        {
            _combinators.Push(combinator);
        }

        private void OnDelim(Token token)
        {
            switch (token.Data[0])
            {
                case Symbols.Comma:
                    InsertOr(token);
                    _ready = false;
                    break;
                case Symbols.GreaterThan:
                    Insert(Combinator.Child);
                    _ready = false;
                    break;
                case Symbols.Plus:
                    Insert(Combinator.AdjacentSibling);
                    _ready = false;
                    break;
                case Symbols.Tilde:
                    Insert(Combinator.Sibling);
                    _ready = false;
                    break;
                case Symbols.Asterisk:
                    Insert(SimpleSelector.All, token);
                    _ready = true;
                    break;
                case Symbols.Dot:
                    _state = State.Class;
                    _ready = false;
                    break;
                case Symbols.Pipe:
                    if ((_combinators.Count > 0) && (_combinators.Peek() == Combinator.Descendent))
                    {
                        Insert(SimpleSelector.Type(string.Empty, token), token);
                    }
                    Insert(Combinator.Namespace);
                    _ready = false;
                    break;
                default:
                    _valid = false;
                    break;
            }
        }

        private ISelector GetPseudoFunction(FunctionToken arguments)
        {
            if (!PseudoClassFunctions.TryGetValue(arguments.Data, out Func<SelectorConstructor, FunctionState> creator))
            {
                return null;
            }

            using (var function = creator(this))
            {
                _ready = false;
                foreach (var token in arguments)
                    if (function.Finished(token))
                    {
                        var sel = function.Produce(token);
                        if (IsNested && function is NotFunctionState)
                            sel = null;
                        _ready = true;
                        return sel;
                    }
            }
            return null;
        }

        private SelectorConstructor CreateChild()
        {
            return Pool.NewSelectorConstructor(_attributeSelector, _pseudoClassSelector, _pseudoElementSelector);
        }

        private abstract class FunctionState : IDisposable
        {
            public virtual void Dispose()
            {
            }
            public bool Finished(Token token)
            {
                return OnToken(token);
            }
            public abstract ISelector Produce(Token token);
            protected abstract bool OnToken(Token token);
        }

        private sealed class NotFunctionState : FunctionState
        {
            private readonly SelectorConstructor _selector;
            public NotFunctionState(SelectorConstructor parent)
            {
                _selector = parent.CreateChild();
                _selector.IsNested = true;
            }
            protected override bool OnToken(Token token)
            {
                if ((token.Type != TokenType.RoundBracketClose) || (_selector._state != State.Data))
                {
                    _selector.Apply(token);
                    return false;
                }
                return true;
            }

            public override ISelector Produce(Token token)
            {
                var valid = _selector.IsValid;
                var sel = _selector.GetResult();
                if (valid)
                {
                    var code = PseudoClassNames.Not.StylesheetFunction(sel.Text);
                    return SimpleSelector.PseudoClass( /*el => !sel.Match(el),*/ code, token);
                }
                return null;
            }

            public override void Dispose()
            {
                base.Dispose();
                _selector.ToPool();
            }
        }

        private sealed class HasFunctionState : FunctionState
        {
            private readonly SelectorConstructor _nested;
            public HasFunctionState(SelectorConstructor parent)
            {
                _nested = parent.CreateChild();
            }
            protected override bool OnToken(Token token)
            {
                if ((token.Type != TokenType.RoundBracketClose) || (_nested._state != State.Data))
                {
                    _nested.Apply(token);
                    return false;
                }
                return true;
            }

            public override ISelector Produce(Token token)
            {
                var valid = _nested.IsValid;
                var sel = _nested.GetResult();
                if (valid)
                {
                    var code = PseudoClassNames.Has.StylesheetFunction(sel.Text);
                    return SimpleSelector.PseudoClass( /*el => el.ChildNodes.QuerySelector(sel) != null,*/ code, token);
                }
                return null;
            }

            public override void Dispose()
            {
                base.Dispose();
                _nested.ToPool();
            }
        }

        private sealed class MatchesFunctionState : FunctionState
        {
            private readonly SelectorConstructor _selector;
            public MatchesFunctionState(SelectorConstructor parent)
            {
                _selector = parent.CreateChild();
            }

            protected override bool OnToken(Token token)
            {
                if ((token.Type != TokenType.RoundBracketClose) || (_selector._state != State.Data))
                {
                    _selector.Apply(token);
                    return false;
                }
                return true;
            }

            public override ISelector Produce(Token token)
            {
                var valid = _selector.IsValid;
                var sel = _selector.GetResult();
                if (valid)
                {
                    var code = PseudoClassNames.Matches.StylesheetFunction(sel.Text);
                    return SimpleSelector.PseudoClass( /*el => sel.Match(el),*/ code, token);
                }
                return null;
            }
            public override void Dispose()
            {
                base.Dispose();
                _selector.ToPool();
            }
        }

        private sealed class DirFunctionState : FunctionState
        {
            private bool _valid;
            private string _value;
            public DirFunctionState()
            {
                _valid = true;
                _value = null;
            }

            protected override bool OnToken(Token token)
            {
                if (token.Type == TokenType.Ident)
                {
                    _value = token.Data;
                }
                else if (token.Type == TokenType.RoundBracketClose)
                {
                    return true;
                }
                else if (token.Type != TokenType.Whitespace)
                {
                    _valid = false;
                }
                return false;
            }
            public override ISelector Produce(Token token)
            {
                if (!_valid || (_value == null))
                {
                    return null;
                }

                var code = PseudoClassNames.Dir.StylesheetFunction(_value);
                return SimpleSelector.PseudoClass(code, token);
            }
        }
        private sealed class LangFunctionState : FunctionState
        {
            private bool valid;
            private string value;
            public LangFunctionState()
            {
                valid = true;
                value = null;
            }
            protected override bool OnToken(Token token)
            {
                if (token.Type == TokenType.Ident)
                {
                    value = token.Data;
                }
                else if (token.Type == TokenType.RoundBracketClose)
                {
                    return true;
                }
                else if (token.Type != TokenType.Whitespace)
                {
                    valid = false;
                }

                return false;
            }
            public override ISelector Produce(Token token)
            {
                if (valid && (value != null))
                {
                    var code = PseudoClassNames.Lang.StylesheetFunction(value);
                    return SimpleSelector.PseudoClass(code, token);
                }
                return null;
            }
        }

        private sealed class ContainsFunctionState : FunctionState
        {
            private bool _valid;
            private string _value;
            public ContainsFunctionState()
            {
                _valid = true;
                _value = null;
            }
            protected override bool OnToken(Token token)
            {
                if ((token.Type == TokenType.Ident) || (token.Type == TokenType.String))
                {
                    _value = token.Data;
                }
                else if (token.Type == TokenType.RoundBracketClose)
                {
                    return true;
                }
                else if (token.Type != TokenType.Whitespace)
                {
                    _valid = false;
                }

                return false;
            }
            public override ISelector Produce(Token token)
            {
                if (_valid && (_value != null))
                {
                    var code = PseudoClassNames.Contains.StylesheetFunction(_value);
                    return SimpleSelector.PseudoClass(code, token);
                }
                return null;
            }
        }

        private sealed class HostContextFunctionState : FunctionState
        {
            private readonly SelectorConstructor _selector;
            public HostContextFunctionState(SelectorConstructor parent)
            {
                _selector = parent.CreateChild();
            }
            protected override bool OnToken(Token token)
            {
                if ((token.Type != TokenType.RoundBracketClose) || (_selector._state != State.Data))
                {
                    _selector.Apply(token);
                    return false;
                }
                return true;
            }
            public override ISelector Produce(Token token)
            {
                var valid = _selector.IsValid;
                var sel = _selector.GetResult();
                if (valid)
                {
                    var code = PseudoClassNames.HostContext.StylesheetFunction(sel.Text);
                    return SimpleSelector.PseudoClass(code, token);
                }
                return null;
            }
            public override void Dispose()
            {
                base.Dispose();
                _selector.ToPool();
            }
        }

        private sealed class ChildFunctionState<T> : FunctionState
            where T : ChildSelector, ISelector
        {
            private readonly SelectorConstructor _parent;
            private readonly bool _allowOf;
            private SelectorConstructor _nested;
            private int _offset;
            private int _sign;
            private ParseState _state;
            private int _step;
            private bool _valid;

            public ChildFunctionState(SelectorConstructor parent, bool withOptionalSelector = true)
            {
                _parent = parent;
                _allowOf = withOptionalSelector;
                _valid = true;
                _sign = 1;
                _state = ParseState.Initial;
            }

            public override ISelector Produce(Token token)
            {
                var invalid = !_valid || ((_nested != null) && !_nested.IsValid);
                var sel = _nested?.ToPool() ?? SimpleSelector.All;
                if (invalid)
                    return null;
                return ((T) Activator.CreateInstance(typeof(T), token)).With(_step, _offset, sel);
            }

            protected override bool OnToken(Token token)
            {
                switch (_state)
                {
                    case ParseState.Initial:
                        return OnInitial(token);
                    case ParseState.AfterInitialSign:
                        return OnAfterInitialSign(token);
                    case ParseState.Offset:
                        return OnOffset(token);
                    case ParseState.BeforeOf:
                        return OnBeforeOf(token);
                    default:
                        return OnAfter(token);
                }
            }
            private bool OnAfterInitialSign(Token token)
            {
                if (token.Type == TokenType.Number)
                {
                    return OnOffset(token);
                }
                if (token.Type == TokenType.Dimension)
                {
                    var dim = (UnitToken) token;
                    _valid = _valid && dim.Unit.Isi("n") && int.TryParse(token.Data, out _step);
                    _step *= _sign;
                    _sign = 1;
                    _state = ParseState.Offset;
                    return false;
                }
                if ((token.Type == TokenType.Ident) && token.Data.Isi("n"))
                {
                    _step = _sign;
                    _sign = 1;
                    _state = ParseState.Offset;
                    return false;
                }
                if ((_state == ParseState.Initial) && (token.Type == TokenType.Ident) && token.Data.Isi("-n"))
                {
                    _step = -1;
                    _state = ParseState.Offset;
                    return false;
                }
                _valid = false;
                return token.Type == TokenType.RoundBracketClose;
            }

            private bool OnAfter(Token token)
            {
                if ((token.Type != TokenType.RoundBracketClose) || (_nested._state != State.Data))
                {
                    _nested.Apply(token);
                    return false;
                }
                return true;
            }

            private bool OnBeforeOf(Token token)
            {
                if (token.Type == TokenType.Whitespace)
                {
                    return false;
                }
                if (token.Data.Isi(Keywords.Of))
                {
                    _valid = _allowOf;
                    _state = ParseState.AfterOf;
                    _nested = _parent.CreateChild();
                    return false;
                }
                if (token.Type == TokenType.RoundBracketClose)
                {
                    return true;
                }
                _valid = false;
                return false;
            }
            private bool OnOffset(Token token)
            {
                if (token.Type == TokenType.Whitespace)
                {
                    return false;
                }
                if (token.Type == TokenType.Number)
                {
                    _valid = _valid && ((NumberToken) token).IsInteger && int.TryParse(token.Data, out _offset);
                    _offset *= _sign;
                    _state = ParseState.BeforeOf;
                    return false;
                }
                return OnBeforeOf(token);
            }
            private bool OnInitial(Token token)
            {
                if (token.Type == TokenType.Whitespace)
                {
                    return false;
                }
                if (token.Data.Isi(Keywords.Odd))
                {
                    _state = ParseState.BeforeOf;
                    _step = 2;
                    _offset = 1;
                    return false;
                }
                if (token.Data.Isi(Keywords.Even))
                {
                    _state = ParseState.BeforeOf;
                    _step = 2;
                    _offset = 0;
                    return false;
                }
                if ((token.Type == TokenType.Delim) && token.Data.IsOneOf("+", "-"))
                {
                    _sign = token.Data == "-" ? -1 : +1;
                    _state = ParseState.AfterInitialSign;
                    return false;
                }
                return OnAfterInitialSign(token);
            }
            private enum ParseState : byte
            {
                Initial,
                AfterInitialSign,
                Offset,
                BeforeOf,
                AfterOf
            }
        }
    }
}