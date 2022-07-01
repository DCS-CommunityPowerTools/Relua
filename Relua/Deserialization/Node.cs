using System.IO;
using System.Text;

namespace Relua.Deserialization {

	/// <summary>
	/// Base class of all Lua AST node.
	/// </summary>
	public abstract class Node {

		public abstract void Write(IndentAwareTextWriter writer);
		public abstract void Accept(IVisitor visitor);


		public override string ToString() {
			return this.ToString(false);
		}


		public virtual string ToString(bool one_line) {
			StringBuilder s = new StringBuilder();
			StringWriter sw = new StringWriter(s);
			IndentAwareTextWriter iw = new IndentAwareTextWriter(sw);
			iw.ForceOneLine = one_line;
			this.Write(iw);
			return s.ToString();
		}

	}

}
