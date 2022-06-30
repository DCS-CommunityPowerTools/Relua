using Relua.Deserialization.Definitions;
using Relua.Deserialization.Expressions;
using Relua.Deserialization.Literals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Statements {

	/// <summary>
	/// Assignment statement. Note that Lua allows for multiple elements on
	/// both sides of the statement.
	/// 
	/// This node is responsible not only for representing syntax in the form of
	/// `x = y` and `local x = y`, but also function definitions with names
	/// (`function name() end` or `local function name() end`).
	/// 
	/// If there is only one target, only one value, the value is a `FunctionDefinition`
	/// and the target is a `StringLiteral` that is a valid identifier, then
	/// this node will be written using the named function style as opposed to
	/// assigning an anonymous function to the appropriate variable.
	/// 
	/// If `IsLocal` is `true`, the size of both `Targets` and `Values` is the
	/// same and all entries in `Values` are `NilLiteral`s, a local declaration
	/// will be emitted that in the Lua language implicitly assigns the value
	/// `nil` (i.e. `local a` instead of `local a = nil`). The same syntax will
	/// be used if `IsLocal` is `true` and `Values` has length 0. If
	/// `ForceExplicitLocalNil` is `true`, this will not happen and `nil`s will
	/// always be explicitly assigned.
	/// 
	/// If `ForceExplicitLocalNil` is set to `true`, values emitted will always
	/// match the amount of targets, filling in the missing entries with `nil`s
	/// if necessary.
	/// 
	/// If the parser setting `AutofillValuesInLocalDeclaration` is set to `false`
	/// (it's set to `true` by default), then in the case of a local declaration
	/// like above, the `Values` list will be empty. As mentioned above, the
	/// local declaration will still be written in the fancy syntax, unless the
	/// `ForceEXplicitLocalNil` override is used.
	/// 
	/// ```
	/// x = y
	/// x, y = a, b
	/// a, b, c = (f()), 1, "hi"
	/// function a()
	///     print("hi")
	/// end
	/// local function b()
	///     print("hi")
	/// end
	/// ```
	/// </summary>
	public class Assignment : Node, IStatement {
		public bool IsLocal;
		public bool ForceExplicitLocalNil;
		public List<IAssignable> Targets = new List<IAssignable>();
		public List<IExpression> Values = new List<IExpression>();

		public void WriteNamedFunctionStyle(IndentAwareTextWriter writer, string name, FunctionDefinition func) {
			writer.Write("function ");
			writer.Write(name);
			func.Write(writer, from_named: true);
		}

		public void WriteGenericStyle(IndentAwareTextWriter writer) {
			// note: local declaration is never named function style (because
			// that automatically implies there's a value assigned)

			for (var i = 0; i < Targets.Count; i++) {
				var target = Targets[i] as Node;
				target.Write(writer);
				if (i != Targets.Count - 1) writer.Write(", ");
			}

			if (IsLocalDeclaration) return;

			writer.Write(" = ");
			for (var i = 0; i < Values.Count; i++) {
				var value = Values[i] as Node;
				value.Write(writer);
				if (i != Values.Count - 1) writer.Write(", ");
			}

			if (ForceExplicitLocalNil && Values.Count < Targets.Count) {
				// match with nils if ForceExplicitLocalNil is set
				if (Values.Count > 0) writer.Write(", ");
				var fill_in_count = Targets.Count - Values.Count;
				for (var i = 0; i < fill_in_count; i++) {
					writer.Write("nil");
					if (i < fill_in_count - 1) writer.Write(", ");
				}
			}
		}

		// Please see explanation in the class summary.
		public bool IsLocalDeclaration {
			get {
				if (ForceExplicitLocalNil) return false;
				if (IsLocal && Values.Count == 0) return true;
				if (IsLocal && Targets.Count == Values.Count) {
					for (var i = 0; i < Values.Count; i++) {
						if (!(Values[i] is NilLiteral)) return false;
					}
					return true;
				}
				return false;
			}
		}

		public override void Write(IndentAwareTextWriter writer) {
			if (IsLocal) writer.Write("local ");

			if (Targets.Count == 1 && Values.Count == 1 && Values[0] is FunctionDefinition) {
				string funcname = null;

				if (Targets[0] is VariableExpression && ((VariableExpression)Targets[0]).Name.IsIdentifier()) {
					funcname = ((VariableExpression)Targets[0]).Name;
					WriteNamedFunctionStyle(writer, funcname, Values[0] as FunctionDefinition);
				} else if (Targets[0] is TableAccessExpression) {
					var func = Values[0] as FunctionDefinition;
					funcname = ((TableAccessExpression)Targets[0]).GetIdentifierAccessChain(is_method_access: func.ImplicitSelf);
					if (funcname != null) WriteNamedFunctionStyle(writer, funcname, func);
					else WriteGenericStyle(writer);
				} else WriteGenericStyle(writer);
			} else {
				WriteGenericStyle(writer);
			}
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}
