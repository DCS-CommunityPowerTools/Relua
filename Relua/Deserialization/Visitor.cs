using System;
using Relua.Deserialization;
using Relua.Deserialization.Definitions;
using Relua.Deserialization.Expressions;
using Relua.Deserialization.Literals;
using Relua.Deserialization.Statements;

namespace Relua {
    public interface IVisitor {
        void Visit(VariableExpression node);
        void Visit(NilLiteral node);
        void Visit(VarargsLiteral node);
        void Visit(BoolLiteral node);
        void Visit(UnaryOp node);
        void Visit(BinaryOp node);
        void Visit(StringLiteral node);
        void Visit(NumberLiteral node);
        void Visit(LuaJitLongLiteral node);
        void Visit(TableAccessExpression node);
        void Visit(FunctionCall node);
        void Visit(TableConstructor node);
        void Visit(TableConstructor.Entry node);
        void Visit(Break node);
        void Visit(Return node);
        void Visit(Block node);
        void Visit(ConditionalBlock node);
        void Visit(If node);
        void Visit(While node);
        void Visit(Repeat node);
        void Visit(FunctionDefinition node);
        void Visit(Assignment node);
        void Visit(NumericFor node);
        void Visit(GenericFor node);
    }

    public abstract class Visitor : IVisitor {
        public virtual void Visit(VariableExpression node) { }
        public virtual void Visit(NilLiteral node) { }
        public virtual void Visit(VarargsLiteral node) { }
        public virtual void Visit(BoolLiteral node) { }
        public virtual void Visit(UnaryOp node) { }
        public virtual void Visit(BinaryOp node) { }
        public virtual void Visit(StringLiteral node) { }
        public virtual void Visit(NumberLiteral node) { }
        public virtual void Visit(LuaJitLongLiteral node) { }
        public virtual void Visit(TableAccessExpression node) { }
        public virtual void Visit(FunctionCall node) { }
        public virtual void Visit(TableConstructor node) { }
        public virtual void Visit(TableConstructor.Entry node) { }
        public virtual void Visit(Break node) { }
        public virtual void Visit(Return node) { }
        public virtual void Visit(Block node) { }
        public virtual void Visit(ConditionalBlock node) { }
        public virtual void Visit(If node) { }
        public virtual void Visit(While node) { }
        public virtual void Visit(Repeat node) { }
        public virtual void Visit(FunctionDefinition node) { }
        public virtual void Visit(Assignment node) { }
        public virtual void Visit(NumericFor node) { }
        public virtual void Visit(GenericFor node) { }
    }

}
