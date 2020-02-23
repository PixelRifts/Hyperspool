using System.Collections.Generic;
using System.Reflection;

namespace Hyperspool
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var _properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var _property in _properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(_property.PropertyType))
                {
                    var _child = (SyntaxNode)_property.GetValue(this);
                    yield return _child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(_property.PropertyType))
                {
                    var _children = (IEnumerable<SyntaxNode>)_property.GetValue(this);
                    foreach (var _child in _children)
                        yield return _child;
                }
            }
        }
    }
}
