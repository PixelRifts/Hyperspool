using System;

namespace Hyperspool
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string _name, bool _isReadOnly, Type _type)
        {
            Name = _name;
            IsReadOnly = _isReadOnly;
            Type = _type;
        }

        public string Name { get; }
        public bool IsReadOnly { get; }
        public Type Type { get; }
    }
}
