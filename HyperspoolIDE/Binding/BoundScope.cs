using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hyperspool
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope _parent)
        {
            Parent = _parent;
        }

        public BoundScope Parent { get; }
        
        public bool TryDeclare(VariableSymbol _variable)
        {
            if (variables.ContainsKey(_variable.Name))
                return false;

            variables.Add(_variable.Name, _variable);
            return true;
        }

        public bool TryLookup(string _name, out VariableSymbol _variable)
        {
            if (variables.TryGetValue(_name, out _variable))
                return true;
            if (Parent == null)
                return false;

            return Parent.TryLookup(_name, out _variable);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables.Values.ToImmutableArray();
        }

        public void Clear()
        {
            variables.Clear();
        }
    }
}
