using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization {

    /// <summary>
    /// Interface (used as a sort of "type tag") for Lua AST nodes that are "assignable" expressions
    /// (variables and table access).
    /// </summary>
    public interface IAssignable : IExpressionBase {
    }

}
