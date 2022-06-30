using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization.Expressions {

	/// <summary>
	/// Binary operation expression. Please note that the parser uses precedence
	/// rules that favors combining the left side for operations that are
	/// order independent. For example, if parsing an expression like `a + b + c`,
	/// the structure of the AST will *always* look like `((a + b) + c)`, even
	/// though `(a + (b + c))` would yield the same result mathematically (but
	/// not necessarily in Lua if you consider the use of metatables!).
	/// 
	/// ```
	/// a + b
	/// a - b
	/// a * b
	/// a / b
	/// a ^ b
	/// a % b
	/// a .. b
	/// a > b
	/// a >= b
	/// a &lt; b
	/// a &lt;= b
	/// a == b
	/// a ~= b
	/// a and b
	/// a or b
	/// ```
	/// </summary>
	public class BinaryOp : Node, IExpression {
		public enum OpType {
			Add,
			Subtract,
			Multiply,
			Divide,
			Power,
			Modulo,
			Concat,
			GreaterThan,
			GreaterOrEqual,
			LessThan,
			LessOrEqual,
			Equal,
			NotEqual,
			And,
			Or
		}

		public static void WriteBinaryOp(OpType type, IndentAwareTextWriter writer) {
			switch (type) {
				case OpType.Add: writer.Write("+"); break;
				case OpType.Subtract: writer.Write("-"); break;
				case OpType.Multiply: writer.Write("*"); break;
				case OpType.Divide: writer.Write("/"); break;
				case OpType.Power: writer.Write("^"); break;
				case OpType.Modulo: writer.Write("%"); break;
				case OpType.Concat: writer.Write(".."); break;
				case OpType.GreaterThan: writer.Write(">"); break;
				case OpType.GreaterOrEqual: writer.Write(">="); break;
				case OpType.LessThan: writer.Write("<"); break;
				case OpType.LessOrEqual: writer.Write("<="); break;
				case OpType.Equal: writer.Write("=="); break;
				case OpType.NotEqual: writer.Write("~="); break;
				case OpType.And: writer.Write("and"); break;
				case OpType.Or: writer.Write("or"); break;
			}
		}

		public BinaryOp() { }

		public BinaryOp(OpType type, IExpression left, IExpression right) {
			Type = type;
			Left = left;
			Right = right;
		}

		public OpType Type;
		public IExpression Left;
		public IExpression Right;

		public override void Write(IndentAwareTextWriter writer) {
			writer.Write("(");
			(Left as Node).Write(writer);
			writer.Write(" ");
			WriteBinaryOp(Type, writer);
			writer.Write(" ");
			(Right as Node).Write(writer);
			writer.Write(")");
		}

		public override void Accept(IVisitor visitor) => visitor.Visit(this);
	}

}
