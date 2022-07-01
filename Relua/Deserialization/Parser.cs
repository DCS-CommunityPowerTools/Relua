using Relua.Deserialization;
using Relua.Deserialization.Definitions;
using Relua.Deserialization.Exceptions;
using Relua.Deserialization.Expressions;
using Relua.Deserialization.Literals;
using Relua.Deserialization.Statements;
using Relua.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;




namespace Relua {

	public class Parser {

		/// <summary>
		/// Settings which control certain behavior of the parser.
		/// </summary>
		public class Settings {

			/// <summary>
			/// Automatically creates NumberLiterals for sequential elements in
			/// a table constructor (ones that do not have a key specified).
			/// 
			/// Note that if this option is `false`, `AST.TableConstructor.Entry`'s
			/// `Key` field may be `null`. That field will never be `null` if this
			/// option is set to `true`.
			/// </summary>
			public bool AutofillSequentialKeysInTableConstructor = true;

			/// <summary>
			/// Automatically creates NilLiterals for all values of empty local
			/// assignments (in the style of `local a`).
			/// 
			/// Note that if this option is `false`, `AST.Assignment`'s `Values`
			/// list will be empty for local declarations. If it is set to the
			/// default `true`, the `Values` list will always match the `Targets`
			/// list in size in that case with all entries being `NilLiteral`s.
			/// </summary>
			public bool AutofillValuesInLocalDeclaration = true;

			/// <summary>
			/// Automatically fills in the `Step` field of `AST.NumericFor` with
			/// a `NumberLiteral` of value `1` if the statement did not specify
			/// the step expression.
			/// </summary>
			public bool AutofillNumericForStep = true;

			/// <summary>
			/// If `true`, will parse LuaJIT long numbers (in the form `0LL`)
			/// into the special AST node `AST.LuaJitLongLiteral`.
			/// </summary>
			public bool EnableLuaJitLongs = true;

			/// <summary>
			/// There are certain syntax quirks such as accessing the fields of
			/// a string literal (e.g. "abc":match(...)) which Lua will throw a
			/// syntax error upon seeing, but the Relua parser will happily accept
			/// (and correctly write). If this option is enabled, all Lua behavior
			/// is imitated, including errors where they are not strictly necessary.
			/// </summary>
			public bool MaintainSyntaxErrorCompatibility = false;

		}




		public Tokenizer Tokenizer;
		public Settings ParserSettings;

		public Token CurToken;




		public Token PeekToken => this.Tokenizer.PeekToken;




		public Parser(string data, Settings settings = null) : this(new Tokenizer(data, settings), settings) { }

		public Parser(StreamReader r, Settings settings = null) : this(new Tokenizer(r.ReadToEnd(), settings), settings) { }

		public Parser(Tokenizer tokenizer, Settings settings = null) {
			this.ParserSettings = settings ?? new Settings();
			this.Tokenizer = tokenizer;
			this.CurToken = tokenizer.NextToken();
		}




		public void Throw(string msg, Token tok) {
			throw new ParserException(msg, tok.Region);
		}

		public void ThrowExpect(string expected, Token tok) {
			throw new ParserException($"Expected {expected}, got {tok.Type} ({tok.Value.Inspect()})", tok.Region);
		}




		public void Move() {
			if (this.CurToken.Type == TokenType.EOF) {
				return;
			}

			this.CurToken = this.Tokenizer.NextToken();
		}




		public NilLiteral ReadNilLiteral() {
			if (this.CurToken.Value != "nil") {
				this.ThrowExpect("nil", this.CurToken);
			}

			this.Move();
			return NilLiteral.Instance;
		}


		public VarargsLiteral ReadVarargsLiteral() {
			if (this.CurToken.Value != "...") {
				this.ThrowExpect("varargs literal", this.CurToken);
			}

			this.Move();
			return VarargsLiteral.Instance;
		}


		public BoolLiteral ReadBoolLiteral() {
			bool value = false;
			if (this.CurToken.Value == "true") {
				value = true;
			} else if (this.CurToken.Value == "false") {
				value = false;
			} else {
				this.ThrowExpect("bool literal", this.CurToken);
			}

			this.Move();
			return value ? BoolLiteral.TrueInstance : BoolLiteral.FalseInstance;
		}


		public VariableExpression ReadVariable() {
			if (this.CurToken.Type != TokenType.Identifier) {
				this.ThrowExpect("identifier", this.CurToken);
			}

			if (Tokenizer.RESERVED_KEYWORDS.Contains(this.CurToken.Value)) {
				this.Throw($"Cannot use reserved keyword '{this.CurToken.Value}' as variable name", this.CurToken);
			}

			string name = this.CurToken.Value;

			this.Move();
			return new VariableExpression { Name = name };
		}


		public StringLiteral ReadStringLiteral() {
			if (this.CurToken.Type != TokenType.QuotedString) {
				this.ThrowExpect("quoted string", this.CurToken);
			}

			string value = this.CurToken.Value;
			this.Move();
			return new StringLiteral { Value = value };
		}


		public NumberLiteral ReadNumberLiteral() {
			if (this.CurToken.Type != TokenType.Number) {
				this.ThrowExpect("number", this.CurToken);
			}

			if (this.CurToken.Value.StartsWith("0x", StringComparison.InvariantCulture)) {
				if (!int.TryParse(this.CurToken.Value.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier, null, out int hexvalue)) {
					this.ThrowExpect("hex number", this.CurToken);
				}

				this.Move();

				return new NumberLiteral { Value = hexvalue, HexFormat = true };
			}

			if (!double.TryParse(this.CurToken.Value, out double value)) {
				this.ThrowExpect("number", this.CurToken);
			}

			this.Move();
			return new NumberLiteral { Value = value };
		}


		public LuaJitLongLiteral ReadLuaJitLongLiteral() {
			if (this.CurToken.Type != TokenType.Number) {
				this.ThrowExpect("long number", this.CurToken);
			}

			if (this.CurToken.Value.StartsWith("0x", StringComparison.InvariantCulture)) {
				if (!long.TryParse(this.CurToken.Value, System.Globalization.NumberStyles.HexNumber | System.Globalization.NumberStyles.AllowHexSpecifier, null, out long hexvalue)) {
					this.ThrowExpect("hex number", this.CurToken);
				}

				this.Move();

				return new LuaJitLongLiteral { Value = hexvalue, HexFormat = true };
			}

			if (!long.TryParse(this.CurToken.Value, out long value)) {
				this.ThrowExpect("number", this.CurToken);
			}

			this.Move();
			if (!this.CurToken.IsIdentifier("LL")) {
				this.ThrowExpect("'LL' suffix", this.CurToken);
			}

			this.Move();
			return new LuaJitLongLiteral { Value = value };
		}


		public TableAccessExpression ReadTableAccess(IExpression table_expr, bool allow_colon = false) {
			TableAccessExpression table_node = null;

			if (this.CurToken.IsPunctuation(".") || (allow_colon && this.CurToken.IsPunctuation(":"))) {
				this.Move();
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				StringLiteral index = new StringLiteral { Value = this.CurToken.Value };
				this.Move();
				table_node = new TableAccessExpression { Table = table_expr, Index = index };
			} else if (this.CurToken.IsPunctuation("[")) {
				this.Move();
				IExpression index = this.ReadExpression();
				if (!this.CurToken.IsPunctuation("]")) {
					this.ThrowExpect("closing bracket", this.CurToken);
				}

				this.Move();
				table_node = new TableAccessExpression { Table = table_expr, Index = index };
			} else {
				this.ThrowExpect("table access", this.CurToken);
			}

			return table_node;
		}


		public FunctionCallExpression ReadFunctionCall(IExpression func_expr, IExpression self_expr = null) {
			if (!this.CurToken.IsPunctuation("(")) {
				this.ThrowExpect("start of argument list", this.CurToken);
			}

			this.Move();

			List<IExpression> args = new List<IExpression>();

			if (self_expr != null) {
				args.Add(self_expr);
			}

			if (!this.CurToken.IsPunctuation(")")) {
				args.Add(this.ReadExpression());
			}

			while (this.CurToken.IsPunctuation(",")) {
				this.Move();
				IExpression expr = this.ReadExpression();
				args.Add(expr);
				if (!this.CurToken.IsPunctuation(",") && !this.CurToken.IsPunctuation(")")) {
					this.ThrowExpect("comma or end of argument list", this.CurToken);
				}
			}
			if (!this.CurToken.IsPunctuation(")")) {
				this.ThrowExpect("end of argument list", this.CurToken);
			}

			this.Move();

			return new FunctionCallExpression { Function = func_expr, Arguments = args };
		}


		public TableConstructorExpression.Entry ReadTableConstructorEntry() {
			if (this.CurToken.Type == TokenType.Identifier) {
				Token eq = this.PeekToken;
				if (eq.IsPunctuation("=")) {
					// { a = ... }

					StringLiteral key = new StringLiteral { Value = this.CurToken.Value };
					this.Move();
					this.Move(); // =
					IExpression value = this.ReadExpression();
					return new TableConstructorExpression.Entry { ExplicitKey = true, Key = key, Value = value };
				} else {
					// { a }
					IExpression value = this.ReadExpression();
					return new TableConstructorExpression.Entry { ExplicitKey = false, Value = value };
					// Note - Key is null
					// This is filled in in ReadTableConstructor
				}
			} else if (this.CurToken.IsPunctuation("[")) {
				// { [expr] = ... }
				this.Move();
				IExpression key = this.ReadExpression();
				if (!this.CurToken.IsPunctuation("]")) {
					this.ThrowExpect("end of key", this.CurToken);
				}

				this.Move();
				if (!this.CurToken.IsPunctuation("=")) {
					this.ThrowExpect("assignment", this.CurToken);
				}

				this.Move();
				IExpression value = this.ReadExpression();
				return new TableConstructorExpression.Entry { ExplicitKey = true, Key = key, Value = value };
			} else {
				// { expr }
				return new TableConstructorExpression.Entry { ExplicitKey = false, Value = this.ReadExpression() };
				// Note - Key is null
				// This is filled in in ReadTableConstructor
			}
		}


		public TableConstructorExpression ReadTableConstructor() {
			if (!this.CurToken.IsPunctuation("{")) {
				this.ThrowExpect("table constructor", this.CurToken);
			}

			this.Move();

			List<TableConstructorExpression.Entry> entries = new List<TableConstructorExpression.Entry>();

			int cur_sequential_idx = 1;

			if (!this.CurToken.IsPunctuation("}")) {
				TableConstructorExpression.Entry ent = this.ReadTableConstructorEntry();
				if (this.ParserSettings.AutofillSequentialKeysInTableConstructor && ent.Key == null) {
					ent.Key = new NumberLiteral { Value = cur_sequential_idx };
					cur_sequential_idx += 1;
				}
				entries.Add(ent);
			}

			while (this.CurToken.IsPunctuation(",") || this.CurToken.IsPunctuation(";")) {
				this.Move();
				if (this.CurToken.IsPunctuation("}")) {
					break; // trailing comma
				}

				TableConstructorExpression.Entry ent = this.ReadTableConstructorEntry();
				if (this.ParserSettings.AutofillSequentialKeysInTableConstructor && ent.Key == null) {
					ent.Key = new NumberLiteral { Value = cur_sequential_idx };
					cur_sequential_idx += 1;
				}
				entries.Add(ent);
				if (!this.CurToken.IsPunctuation(",") && !this.CurToken.IsPunctuation(";") && !this.CurToken.IsPunctuation("}")) {
					this.ThrowExpect("comma or end of entry list", this.CurToken);
				}
			}

			if (!this.CurToken.IsPunctuation("}")) {
				this.ThrowExpect("end of entry list", this.CurToken);
			}

			this.Move();

			return new TableConstructorExpression { Entries = entries };
		}


		public FunctionDefinition ReadFunctionDefinition(bool start_from_params = false, bool self = false) {
			if (!start_from_params) {
				if (!this.CurToken.IsPunctuation("function")) {
					this.ThrowExpect("function", this.CurToken);
				}

				this.Move();
			}

			if (!this.CurToken.IsPunctuation("(")) {
				this.ThrowExpect("start of argument name list", this.CurToken);
			}

			this.Move();

			bool varargs = false;
			List<string> args = new List<string>();

			if (self) {
				args.Add("self");
			}

			if (!this.CurToken.IsPunctuation(")")) {
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				args.Add(this.CurToken.Value);
				this.Move();
			}

			while (this.CurToken.IsPunctuation(",")) {
				this.Move();
				if (this.CurToken.IsPunctuation("...")) {
					varargs = true;
					this.Move();
					break;
				}
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				args.Add(this.CurToken.Value);
				this.Move();
			}

			if (!this.CurToken.IsPunctuation(")")) {
				this.ThrowExpect("end of argument name list", this.CurToken);
			}

			this.Move();

			this.SkipSemicolons();

			List<IStatement> statements = new List<IStatement>();
			while (!this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}

			this.Move();

			return new FunctionDefinition {
				ArgumentNames = args,
				Block = new BlockStatement { Statements = statements },
				AcceptsVarargs = varargs,
				ImplicitSelf = self
			};
		}


		// Primary expression:
		// - Does not depend on any expressions.
		public IExpression ReadPrimaryExpression() {
			if (this.CurToken.Type == TokenType.QuotedString) {
				return this.ReadStringLiteral();
			}

			if (this.CurToken.Type == TokenType.Number) {
				if (this.ParserSettings.EnableLuaJitLongs && this.PeekToken.IsIdentifier("LL")) {
					return this.ReadLuaJitLongLiteral();
				} else {
					return this.ReadNumberLiteral();
				}
			}

			if (this.CurToken.Type == TokenType.Punctuation) {
				if (this.CurToken.Value == "{") {
					return this.ReadTableConstructor();
				}

				if (this.CurToken.Value == "...") {
					return this.ReadVarargsLiteral();
				}

				if (this.CurToken.Value == "nil") {
					return this.ReadNilLiteral();
				}

				if (this.CurToken.Value == "true" || this.CurToken.Value == "false") {
					return this.ReadBoolLiteral();
				}
				if (this.CurToken.Value == "function") {
					return this.ReadFunctionDefinition();
				}

			} else if (this.CurToken.Type == TokenType.Identifier) {
				return this.ReadVariable();
			}

			this.ThrowExpect("expression", this.CurToken);
			throw new Exception("unreachable");
		}


		public OperatorInfo? GetBinaryOperator(Token tok) {
			if (tok.Value == null) {
				return null;
			}

			OperatorInfo? op = OperatorInfo.FromToken(tok);
			if (op == null) {
				return null;
			}

			if (!op.Value.IsBinary) {
				this.ThrowExpect("binary operator", tok);
			}

			return op.Value;
		}


		// Secondary expression:
		// - Depends on (alters the value of) *one* expression.
		public IExpression ReadSecondaryExpression() {
			OperatorInfo? unary_op = OperatorInfo.FromToken(this.CurToken);

			if (unary_op != null && unary_op.Value.IsUnary) {
				this.Move();
			}

			IExpression expr;

			if (this.CurToken.IsPunctuation("(")) {
				this.Move();
				IExpression complex = this.ReadComplexExpression(this.ReadSecondaryExpression(), 0, true);
				if (!this.CurToken.IsPunctuation(")")) {
					this.ThrowExpect("closing parenthesis", this.CurToken);
				}
				this.Move();
				expr = complex;
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = true;
				}
			} else {
				expr = this.ReadPrimaryExpression();
			}

			while (this.CurToken.IsPunctuation(".") || this.CurToken.IsPunctuation("[")) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				if (expr is StringLiteral && this.ParserSettings.MaintainSyntaxErrorCompatibility) {
					this.Throw($"syntax error compat: can't directly index strings, use parentheses", this.CurToken);
				}
				expr = this.ReadTableAccess(expr);
			}

			while (this.CurToken.IsPunctuation(":")) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				if (expr is StringLiteral && this.ParserSettings.MaintainSyntaxErrorCompatibility) {
					this.Throw($"syntax error compat: can't directly index strings, use parentheses", this.CurToken);
				}
				IExpression self_expr = expr;
				expr = this.ReadTableAccess(expr, allow_colon: true);
				expr = this.ReadFunctionCall(expr, self_expr);
			}

			if (this.CurToken.IsPunctuation("(")) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				if (expr is StringLiteral && this.ParserSettings.MaintainSyntaxErrorCompatibility) {
					this.Throw($"syntax error compat: can't directly call strings, use parentheses", this.CurToken);
				}
				expr = this.ReadFunctionCall(expr);
			} else if (this.CurToken.IsPunctuation("{")) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				if (expr is StringLiteral && this.ParserSettings.MaintainSyntaxErrorCompatibility) {
					this.Throw($"syntax error compat: can't directly call strings, use parentheses", this.CurToken);
				}
				expr = new FunctionCallExpression {
					Function = expr,
					Arguments = new List<IExpression> { this.ReadTableConstructor() }
				};
			} else if (this.CurToken.Type == TokenType.QuotedString) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				if (expr is StringLiteral && this.ParserSettings.MaintainSyntaxErrorCompatibility) {
					this.Throw($"syntax error compat: can't directly call strings, use parentheses", this.CurToken);
				}
				expr = new FunctionCallExpression {
					Function = expr,
					Arguments = new List<IExpression> { this.ReadStringLiteral() }
				};
			}

			if (unary_op != null && unary_op.Value.IsUnary) {
				if (expr is FunctionCallExpression) {
					((FunctionCallExpression)expr).ForceTruncateReturnValues = false;
				}

				expr = new UnaryExpression(unary_op.Value.UnaryOpType.Value, expr);
			}

			return expr;
		}


		// Complex expression:
		// - Depends on (alters the value of) *two* expressions.
		public IExpression ReadComplexExpression(IExpression lhs, int prev_op_prec, bool in_parens, int depth = 0) {
			OperatorInfo? lookahead = this.GetBinaryOperator(this.CurToken);
			if (lookahead == null) {
				return lhs;
			}

			//Console.WriteLine($"{new string(' ', depth)}RCE: lhs = {lhs} lookahead = {lookahead.Value.TokenValue} prev_op_prec = {prev_op_prec}");

			if (lhs is FunctionCallExpression) {
				((FunctionCallExpression)lhs).ForceTruncateReturnValues = false;
				// No need to force this (and produce extra parens),
				// because the binop truncates the return value anyway
			}

			while (lookahead.Value.Precedence >= prev_op_prec) {
				OperatorInfo? op = lookahead;
				this.Move();
				IExpression rhs = this.ReadSecondaryExpression();
				if (rhs is FunctionCallExpression) {
					((FunctionCallExpression)rhs).ForceTruncateReturnValues = false;
				}
				lookahead = this.GetBinaryOperator(this.CurToken);
				if (lookahead == null) {
					return new BinaryExpression(op.Value.BinaryOpType.Value, lhs, rhs);
				}
				//Console.WriteLine($"{new string(' ', depth)}OUT rhs = {rhs} lookahead = {lookahead.Value.TokenValue} prec = {lookahead.Value.Precedence}");

				while (lookahead.Value.RightAssociative ? (lookahead.Value.Precedence == op.Value.Precedence) : (lookahead.Value.Precedence > op.Value.Precedence)) {
					rhs = this.ReadComplexExpression(rhs, lookahead.Value.Precedence, in_parens, depth + 1);
					//Console.WriteLine($"{new string(' ', depth)}IN rhs = {rhs} lookahead = {lookahead.Value.TokenValue}");
					lookahead = this.GetBinaryOperator(this.CurToken);
					if (lookahead == null) {
						return new BinaryExpression(op.Value.BinaryOpType.Value, lhs, rhs);
					}
				}

				lhs = new BinaryExpression(op.Value.BinaryOpType.Value, lhs, rhs);
			}

			return lhs;
		}


		/// <summary>
		/// Reads a single expression.
		/// </summary>
		/// <returns>The expression.</returns>
		public IExpression ReadExpression() {
			IExpression expr = this.ReadSecondaryExpression();
			return this.ReadComplexExpression(expr, 0, false);
		}


		public BreakStatement ReadBreak() {
			if (!this.CurToken.IsPunctuation("break")) {
				this.ThrowExpect("break statement", this.CurToken);
			}

			this.Move();
			return new BreakStatement();
		}


		public ReturnStatement ReadReturn() {
			if (!this.CurToken.IsPunctuation("return")) {
				this.ThrowExpect("return statement", this.CurToken);
			}

			this.Move();

			List<IExpression> ret_vals = new List<IExpression>();

			if (!this.CurToken.IsPunctuation("end")) {
				ret_vals.Add(this.ReadExpression());
			}

			while (this.CurToken.IsPunctuation(",")) {
				this.Move();
				ret_vals.Add(this.ReadExpression());
			}

			return new ReturnStatement { Expressions = ret_vals };
		}


		public IfStatement ReadIf() {
			if (!this.CurToken.IsPunctuation("if")) {
				this.ThrowExpect("if statement", this.CurToken);
			}

			this.Move();

			IExpression cond = this.ReadExpression();

			if (!this.CurToken.IsPunctuation("then")) {
				this.ThrowExpect("'then' keyword", this.CurToken);
			}

			this.Move();

			List<IStatement> statements = new List<IStatement>();

			while (!this.CurToken.IsPunctuation("else") && !this.CurToken.IsPunctuation("elseif") && !this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}

			ConditionalBlockStatement mainif_cond_block = new ConditionalBlockStatement {
				Block = new BlockStatement { Statements = statements },
				Condition = cond
			};

			List<ConditionalBlockStatement> elseifs = new List<ConditionalBlockStatement>();

			while (this.CurToken.IsPunctuation("elseif")) {
				this.Move();
				IExpression elseif_cond = this.ReadExpression();
				if (!this.CurToken.IsPunctuation("then")) {
					this.ThrowExpect("'then' keyword", this.CurToken);
				}

				this.Move();
				List<IStatement> elseif_statements = new List<IStatement>();
				while (!this.CurToken.IsPunctuation("else") && !this.CurToken.IsPunctuation("elseif") && !this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
					elseif_statements.Add(this.ReadStatement());
				}

				elseifs.Add(new ConditionalBlockStatement {
					Block = new BlockStatement { Statements = elseif_statements },
					Condition = elseif_cond
				});
			}

			BlockStatement else_block = null;

			if (this.CurToken.IsPunctuation("else")) {
				this.Move();
				List<IStatement> else_statements = new List<IStatement>();
				while (!this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
					else_statements.Add(this.ReadStatement());
				}

				else_block = new BlockStatement { Statements = else_statements };
			}

			if (!this.CurToken.IsPunctuation("end")) {
				this.ThrowExpect("'end' keyword", this.CurToken);
			}

			this.Move();

			return new IfStatement {
				MainIf = mainif_cond_block,
				ElseIfs = elseifs,
				Else = else_block
			};
		}


		public void SkipSemicolons() {
			while (this.CurToken.IsPunctuation(";")) {
				this.Move();
			}
		}


		public WhileStatement ReadWhile() {
			if (!this.CurToken.IsPunctuation("while")) {
				this.ThrowExpect("while statement", this.CurToken);
			}

			this.Move();
			IExpression cond = this.ReadExpression();

			if (!this.CurToken.IsPunctuation("do")) {
				this.ThrowExpect("'do' keyword", this.CurToken);
			}

			this.Move();

			this.SkipSemicolons();

			List<IStatement> statements = new List<IStatement>();

			while (!this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}
			this.Move();

			return new WhileStatement {
				Condition = cond,
				Block = new BlockStatement { Statements = statements }
			};
		}


		public AssignmentStatement TryReadFullAssignment(bool certain_assign, IExpression start_expr, Token expr_token) {
			// certain_assign should be set to true if we know that
			// what we have is definitely an assignment
			// that allows us to handle implicit nil assignments (local
			// declarations without a value) as an Assignment node

			if (certain_assign || (this.CurToken.IsPunctuation("=") || this.CurToken.IsPunctuation(","))) {
				if (!(start_expr is IAssignable)) {
					this.ThrowExpect("assignable expression", expr_token);
				}

				List<IAssignable> assign_exprs = new List<IAssignable> { start_expr as IAssignable };

				while (this.CurToken.IsPunctuation(",")) {
					this.Move();
					start_expr = this.ReadExpression();
					if (!(start_expr is IAssignable)) {
						this.ThrowExpect("assignable expression", expr_token);
					}

					assign_exprs.Add(start_expr as IAssignable);
				}

				if (certain_assign && !this.CurToken.IsPunctuation("=")) {
					// implicit nil assignment/local declaration

					AssignmentStatement local_decl = new AssignmentStatement {
						IsLocal = true,
						Targets = assign_exprs
					};

					if (this.ParserSettings.AutofillValuesInLocalDeclaration) {
						// Match Values with NilLiterals
						for (int i = 0; i < assign_exprs.Count; i++) {
							local_decl.Values.Add(NilLiteral.Instance);
						}
					}

					return local_decl;
				}

				return this.ReadAssignment(assign_exprs);
			}

			return null;
		}


		public AssignmentStatement ReadAssignment(List<IAssignable> assignable_exprs, bool local = false) {
			if (!this.CurToken.IsPunctuation("=")) {
				this.ThrowExpect("assignment", this.CurToken);
			}

			this.Move();
			List<IExpression> value_exprs = new List<IExpression> { this.ReadExpression() };

			while (this.CurToken.IsPunctuation(",")) {
				this.Move();
				value_exprs.Add(this.ReadExpression());
			}

			return new AssignmentStatement {
				IsLocal = local,
				Targets = assignable_exprs,
				Values = value_exprs,
			};
		}


		public AssignmentStatement ReadNamedFunctionDefinition() {
			if (!this.CurToken.IsPunctuation("function")) {
				this.ThrowExpect("function", this.CurToken);
			}

			this.Move();
			if (this.CurToken.Type != TokenType.Identifier) {
				this.ThrowExpect("identifier", this.CurToken);
			}

			IAssignable expr = new VariableExpression { Name = this.CurToken.Value };
			this.Move();
			while (this.CurToken.IsPunctuation(".")) {
				this.Move();
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				expr = new TableAccessExpression {
					Table = expr as IExpression,
					Index = new StringLiteral { Value = this.CurToken.Value }
				};
				this.Move();
			}
			bool is_method_def = false;
			if (this.CurToken.IsPunctuation(":")) {
				is_method_def = true;
				this.Move();
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				expr = new TableAccessExpression {
					Table = expr as IExpression,
					Index = new StringLiteral { Value = this.CurToken.Value }
				};
				this.Move();
			}
			FunctionDefinition func_def = this.ReadFunctionDefinition(start_from_params: true, self: is_method_def);
			return new AssignmentStatement {
				Targets = new List<IAssignable> { expr },
				Values = new List<IExpression> { func_def }
			};
		}


		public RepeatStatement ReadRepeat() {
			if (!this.CurToken.IsPunctuation("repeat")) {
				this.ThrowExpect("repeat statement", this.CurToken);
			}

			this.Move();
			this.SkipSemicolons();
			List<IStatement> statements = new List<IStatement>();
			while (!this.CurToken.IsPunctuation("until") && !this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}

			if (!this.CurToken.IsPunctuation("until")) {
				this.ThrowExpect("'until' keyword", this.CurToken);
			}

			this.Move();

			IExpression cond = this.ReadExpression();

			return new RepeatStatement {
				Condition = cond,
				Block = new BlockStatement { Statements = statements }
			};
		}


		public BlockStatement ReadBlock(bool alone = false) {
			if (!this.CurToken.IsPunctuation("do")) {
				this.ThrowExpect("block", this.CurToken);
			}

			this.Move();
			this.SkipSemicolons();

			List<IStatement> statements = new List<IStatement>();
			while (!this.CurToken.IsPunctuation("end") && !this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}

			this.Move();

			return new BlockStatement { Statements = statements };
		}


		public GenericForStatement ReadGenericFor() {
			if (this.CurToken.Type != TokenType.Identifier) {
				this.ThrowExpect("identifier", this.CurToken);
			}

			List<string> var_names = new List<string> { this.CurToken.Value };
			this.Move();

			while (this.CurToken.IsPunctuation(",")) {
				this.Move();
				if (this.CurToken.Type != TokenType.Identifier) {
					this.ThrowExpect("identifier", this.CurToken);
				}

				var_names.Add(this.CurToken.Value);
				this.Move();
			}

			if (!this.CurToken.IsPunctuation("in")) {
				this.ThrowExpect("'in' keyword", this.CurToken);
			}

			this.Move();

			IExpression iterator = this.ReadExpression();
			BlockStatement block = this.ReadBlock();

			return new GenericForStatement {
				VariableNames = var_names,
				Iterator = iterator,
				Block = block
			};
		}


		public NumericForStatement ReadNumericFor() {
			if (this.CurToken.Type != TokenType.Identifier) {
				this.ThrowExpect("identifier", this.CurToken);
			}

			string var_name = this.CurToken.Value;
			this.Move();

			if (!this.CurToken.IsPunctuation("=")) {
				this.ThrowExpect("assignment", this.CurToken);
			}

			this.Move();

			IExpression start_pos = this.ReadExpression();
			if (!this.CurToken.IsPunctuation(",")) {
				this.ThrowExpect("end point expression", this.CurToken);
			}

			this.Move();
			IExpression end_pos = this.ReadExpression();

			IExpression step = null;
			if (this.CurToken.IsPunctuation(",")) {
				this.Move();
				step = this.ReadExpression();
			}

			if (step == null && this.ParserSettings.AutofillNumericForStep) {
				step = new NumberLiteral { Value = 1 };
			}

			BlockStatement block = this.ReadBlock();

			return new NumericForStatement {
				VariableName = var_name,
				StartPoint = start_pos,
				EndPoint = end_pos,
				Step = step,
				Block = block
			};
		}


		public ForStatement ReadFor() {
			if (!this.CurToken.IsPunctuation("for")) {
				this.ThrowExpect("for statement", this.CurToken);
			}

			this.Move();

			Token peek = this.PeekToken;
			if (peek.IsPunctuation(",") || peek.IsPunctuation("in")) {
				return this.ReadGenericFor();
			} else {
				return this.ReadNumericFor();
			}
		}


		public IStatement ReadPrimaryStatement() {
			if (this.CurToken.IsPunctuation("break")) {
				return this.ReadBreak();
			}

			if (this.CurToken.IsPunctuation("return")) {
				return this.ReadReturn();
			}

			if (this.CurToken.IsPunctuation("if")) {
				return this.ReadIf();
			}

			if (this.CurToken.IsPunctuation("while")) {
				return this.ReadWhile();
			}

			if (this.CurToken.IsPunctuation("function")) {
				return this.ReadNamedFunctionDefinition();
			}

			if (this.CurToken.IsPunctuation("repeat")) {
				return this.ReadRepeat();
			}

			if (this.CurToken.IsPunctuation("for")) {
				return this.ReadFor();
			}

			if (this.CurToken.IsPunctuation("do")) {
				return this.ReadBlock(alone: true);
			}

			if (this.CurToken.IsPunctuation("local")) {
				this.Move();
				if (this.CurToken.IsPunctuation("function")) {
					AssignmentStatement local_assign = this.ReadNamedFunctionDefinition();
					local_assign.IsLocal = true;
					return local_assign;
				} else {
					Token local_expr_token = this.CurToken;
					IExpression local_expr = this.ReadExpression();
					AssignmentStatement local_assign = this.TryReadFullAssignment(true, local_expr, local_expr_token);
					if (local_assign == null) {
						this.ThrowExpect("assignment statement", this.CurToken);
					}

					local_assign.IsLocal = true;
					return local_assign;
				}
			}

			Token expr_token = this.CurToken;
			IExpression expr = this.ReadExpression();
			AssignmentStatement assign = this.TryReadFullAssignment(false, expr, expr_token);
			if (assign != null) {
				return assign;
			}

			if (expr is FunctionCallExpression) {
				return expr as FunctionCallExpression;
			}

			this.ThrowExpect("statement", expr_token);
			throw new Exception("unreachable");
		}


		/// <summary>
		/// Reads a single statement.
		/// </summary>
		/// <returns>The statement.</returns>
		public IStatement ReadStatement() {
			IStatement stat = this.ReadPrimaryStatement();
			this.SkipSemicolons();
			return stat;
		}


		/// <summary>
		/// Reads a list of statements.
		/// </summary>
		/// <returns>`Block` node (`TopLevel` = `true`).</returns>
		public BlockStatement Read() {
			List<IStatement> statements = new List<IStatement>();

			while (!this.CurToken.IsEOF()) {
				statements.Add(this.ReadStatement());
			}

			return new BlockStatement { Statements = statements, TopLevel = true };
		}

	}

}
