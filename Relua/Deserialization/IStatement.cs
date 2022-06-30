using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization {

	/// <summary>
	/// Interface (used as a sort of "type tag") for Lua AST nodes that are statements.
	/// </summary>
	public interface IStatement {
		void Write(IndentAwareTextWriter writer);
		void Accept(IVisitor visitor);
		string ToString(bool one_line);
	}

}
