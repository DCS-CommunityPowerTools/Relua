using Relua.Deserialization.Definitions;
using Relua.Deserialization.Expressions;
using Relua.Deserialization.Literals;
using System.Collections.Generic;




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
	public class AssignmentStatement : Node, IStatement {

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

			for (int i = 0; i < this.Targets.Count; i++) {
				Node target = this.Targets[i] as Node;
				target.Write(writer);
				if (i != this.Targets.Count - 1) {
					writer.Write(", ");
				}
			}

			if (this.IsLocalDeclaration) {
				return;
			}

			writer.Write(" = ");
			for (int i = 0; i < this.Values.Count; i++) {
				Node value = this.Values[i] as Node;
				value.Write(writer);
				if (i != this.Values.Count - 1) {
					writer.Write(", ");
				}
			}

			if (this.ForceExplicitLocalNil && this.Values.Count < this.Targets.Count) {
				// match with nils if ForceExplicitLocalNil is set
				if (this.Values.Count > 0) {
					writer.Write(", ");
				}

				int fill_in_count = this.Targets.Count - this.Values.Count;
				for (int i = 0; i < fill_in_count; i++) {
					writer.Write("nil");
					if (i < fill_in_count - 1) {
						writer.Write(", ");
					}
				}
			}
		}


		// Please see explanation in the class summary.
		public bool IsLocalDeclaration {
			get {
				if (this.ForceExplicitLocalNil) {
					return false;
				}

				if (this.IsLocal && this.Values.Count == 0) {
					return true;
				}

				if (this.IsLocal && this.Targets.Count == this.Values.Count) {
					for (int i = 0; i < this.Values.Count; i++) {
						if (!(this.Values[i] is NilLiteral)) {
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}


		public override void Write(IndentAwareTextWriter writer) {
			if (this.IsLocal) {
				writer.Write("local ");
			}

			if (this.Targets.Count == 1 && this.Values.Count == 1 && this.Values[0] is FunctionDefinition) {
				string funcname;

				if (this.Targets[0] is VariableExpression && ((VariableExpression)this.Targets[0]).Name.IsIdentifier()) {
					funcname = ((VariableExpression)this.Targets[0]).Name;
					this.WriteNamedFunctionStyle(writer, funcname, this.Values[0] as FunctionDefinition);
				} else if (this.Targets[0] is TableAccessExpression) {
					FunctionDefinition func = this.Values[0] as FunctionDefinition;
					funcname = ((TableAccessExpression)this.Targets[0]).GetIdentifierAccessChain(is_method_access: func.ImplicitSelf);
					if (funcname != null) {
						this.WriteNamedFunctionStyle(writer, funcname, func);
					} else {
						this.WriteGenericStyle(writer);
					}
				} else {
					this.WriteGenericStyle(writer);
				}
			} else {
				this.WriteGenericStyle(writer);
			}
		}


		public override void Accept(IVisitor visitor)
			=> visitor.Visit(this);

	}

}
