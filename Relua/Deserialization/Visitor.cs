using Relua.Deserialization.Definitions;
using Relua.Deserialization.Expressions;
using Relua.Deserialization.Literals;
using Relua.Deserialization.Statements;




namespace Relua {

	public interface IVisitor {

		void Visit(FunctionDefinition node);

		void Visit(BinaryExpression node);
		void Visit(FunctionCallExpression node);
		void Visit(TableAccessExpression node);
		void Visit(TableConstructorExpression node);
		void Visit(TableConstructorExpression.Entry node);
		void Visit(UnaryExpression node);
		void Visit(VariableExpression node);

		void Visit(BoolLiteral node);
		void Visit(LuaJitLongLiteral node);
		void Visit(NilLiteral node);
		void Visit(NumberLiteral node);
		void Visit(StringLiteral node);
		void Visit(VarargsLiteral node);

		void Visit(AssignmentStatement node);
		void Visit(BlockStatement node);
		void Visit(BreakStatement node);
		void Visit(ConditionalBlockStatement node);
		void Visit(GenericForStatement node);
		void Visit(IfStatement node);
		void Visit(NumericForStatement node);
		void Visit(RepeatStatement node);
		void Visit(ReturnStatement node);
		void Visit(WhileStatement node);

	}


	public abstract class Visitor : IVisitor {

		public virtual void Visit(FunctionDefinition node) { }

		public virtual void Visit(BinaryExpression node) { }
		public virtual void Visit(FunctionCallExpression node) { }
		public virtual void Visit(TableAccessExpression node) { }
		public virtual void Visit(TableConstructorExpression node) { }
		public virtual void Visit(TableConstructorExpression.Entry node) { }
		public virtual void Visit(UnaryExpression node) { }
		public virtual void Visit(VariableExpression node) { }

		public virtual void Visit(BoolLiteral node) { }
		public virtual void Visit(LuaJitLongLiteral node) { }
		public virtual void Visit(NilLiteral node) { }
		public virtual void Visit(NumberLiteral node) { }
		public virtual void Visit(StringLiteral node) { }
		public virtual void Visit(VarargsLiteral node) { }

		public virtual void Visit(AssignmentStatement node) { }
		public virtual void Visit(BlockStatement node) { }
		public virtual void Visit(BreakStatement node) { }
		public virtual void Visit(ConditionalBlockStatement node) { }
		public virtual void Visit(GenericForStatement node) { }
		public virtual void Visit(IfStatement node) { }
		public virtual void Visit(NumericForStatement node) { }
		public virtual void Visit(RepeatStatement node) { }
		public virtual void Visit(ReturnStatement node) { }
		public virtual void Visit(WhileStatement node) { }

	}

}
