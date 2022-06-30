using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relua.Deserialization {
	
	public interface IExpressionBase {
		void Write(IndentAwareTextWriter writer);
		void Accept(IVisitor visitor);
		string ToString(bool one_line);
	}

}
