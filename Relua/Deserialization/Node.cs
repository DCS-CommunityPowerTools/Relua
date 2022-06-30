using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization {

	/// <summary>
	/// Base class of all Lua AST node.
	/// </summary>
	public abstract class Node {

		public abstract void Write(IndentAwareTextWriter writer);
		public abstract void Accept(IVisitor visitor);


		public override string ToString() {
			return ToString(false);
		}


		public virtual string ToString(bool one_line) {
			var s = new StringBuilder();
			var sw = new StringWriter(s);
			var iw = new IndentAwareTextWriter(sw);
			iw.ForceOneLine = one_line;
			Write(iw);
			return s.ToString();
		}

	}

}
