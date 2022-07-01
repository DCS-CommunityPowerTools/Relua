using System.IO;




namespace Relua {

	/// <summary>
	/// Representation of mutable indentation state.
	/// </summary>
	public class Indent {

		private const string NO_INDENT = "";




		public int Amount;
		public char Character;
		public int Size;

		private string _CachedString;




		public Indent(char c, int size) {
			this.Amount = 0;
			this.Character = c;
			this.Size = size;
		}




		public static implicit operator string(Indent i)
			=> i.ToString();




		public void Increase() {
			this.Amount += 1;
			this._CachedString = null;
		}

		public void Decrease() {
			this.Amount -= 1;
			if (this.Amount < 0) {
				this.Amount = 0;
			}

			this._CachedString = null;
		}




		public override string ToString() {
			if (this._CachedString != null) {
				return this._CachedString;
			}

			if (this.Amount == 0) {
				return NO_INDENT;
			}

			this._CachedString = new string(this.Character, this.Amount * this.Size);
			return this._CachedString;
		}

	}

}
