using System;

namespace Hyperspool
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string _name, Type _type)
        {
            Name = _name;
            Type = _type;
        }

        public string Name { get; }
        public Type Type { get; }
    }
}
