using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization {

	/// <summary>
	/// `TextWriter` wrapper which automatically inserts indentation on
	/// `WriteLine` calls. If `ForceOneLine` is `true`, `WriteLine` calls will
	/// produce a single space as opposed to newline + indent.
	/// </summary>
	public class IndentAwareTextWriter {

		public Indent Indent;
		public TextWriter Writer;

		public bool ForceOneLine;




		public IndentAwareTextWriter(TextWriter writer, Indent indent = null) {
			this.Indent = indent ?? new Indent(' ', 4);
			this.Writer = writer;
		}




		public static implicit operator IndentAwareTextWriter(TextWriter w)
			=> new IndentAwareTextWriter(w);




		public void Write(string s)
			=> this.Writer.Write(s);


		public void Write(object o)
			=> this.Writer.Write(o);




		public void WriteLine() {
			if (this.ForceOneLine) {
				this.Writer.Write(" ");
				return;
			}
			this.Writer.WriteLine();
			this.Writer.Write(this.Indent.ToString());
		}


		public void WriteLine(string s) {
			if (this.ForceOneLine) {
				this.Writer.Write(" ");
				return;
			}
			this.Writer.WriteLine(s);
			this.Writer.Write(this.Indent.ToString());
		}


		public void WriteLine(object o) {
			if (this.ForceOneLine) {
				this.Writer.Write(" ");
				return;
			}
			this.Writer.WriteLine(o);
			this.Writer.Write(this.Indent.ToString());
		}


		public void WriteIndent() {
			if (!this.ForceOneLine) {
				this.Writer.Write(this.Indent);
			}
		}




		public void IncreaseIndent()
			=> this.Indent.Increase();


		public void DecreaseIndent()
			=> this.Indent.Decrease();

	}

}
