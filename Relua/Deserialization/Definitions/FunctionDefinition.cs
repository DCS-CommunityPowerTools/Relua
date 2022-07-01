using Relua.Deserialization.Statements;
using System.Collections.Generic;




namespace Relua.Deserialization.Definitions {

	/// <summary>
	/// Function definition expression and statement. Do note that parameter
	/// names are stored as strings, since they are not actual expressions.
	/// 
	/// If the function takes variable arguments, the `...` string will not be
	/// included in `ArgumentNames`, but `AcceptsVarargs` will be set to `true`.
	/// 
	/// If the function is empty, it will be shortened to one line (`function()
	/// end`).
	/// 
	/// It is important to note that `FunctionDefinition` is not responsible
	/// for reading or writing functions defined in the named style (e.g.
	/// `function something() ... end`). That operation is interpreted as
	/// an assignment of `FunctionDefinition` to `Variable`, and it is the
	/// `Assignment` node that is responsible both for representing this syntax
	/// and also correctly writing it (avoiding the verbose `something = function()
	/// ... end` if unnecessary).
	/// 
	/// If the function was parsed from the `function a:b() end` syntax, it will
	/// have `ImplicitSelf` set to `true` (and the extra `self` argument). If
	/// this field is `true` while writing a named function Assignment, no
	/// `self` argument will be emitted and the same method definition
	/// syntax will be used. In any other case, a `FunctionDefinition` with
	/// `ImplicitSelf` will emit arguments normally (including the `self`).
	/// 
	/// ```
	/// function() end
	/// function()
	///     print("a")
	///     print("b")
	/// end
	/// ```
	/// </summary>
	public class FunctionDefinition : Node, IExpression, IStatement {

		public List<string> ArgumentNames = new List<string>();
		public BlockStatement Block;
		public bool AcceptsVarargs = false;
		public bool ImplicitSelf = false;


		public void Write(IndentAwareTextWriter writer, bool from_named) {
			if (!from_named) {
				writer.Write("function");
			}

			writer.Write("(");

			int arg_start_idx = 0;

			if (this.ImplicitSelf && from_named) {
				arg_start_idx += 1;
			}
			// Skips the self for method defs

			for (int i = arg_start_idx; i < this.ArgumentNames.Count; i++) {
				string arg = this.ArgumentNames[i];
				writer.Write(arg);
				if (i < this.ArgumentNames.Count - 1) {
					writer.Write(", ");
				}
			}
			if (this.AcceptsVarargs) {
				if (this.ArgumentNames.Count > 0) {
					writer.Write(", ");
				}

				writer.Write("...");
			}
			writer.Write(")");
			if (this.Block.IsEmpty) {
				writer.Write(" ");
			} else {
				writer.IncreaseIndent();
				writer.WriteLine();
				this.Block.Write(writer, false);
				writer.DecreaseIndent();
				writer.WriteLine();
			}
			writer.Write("end");
		}


		public override void Write(IndentAwareTextWriter writer)
			=> this.Write(writer, false);


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}
