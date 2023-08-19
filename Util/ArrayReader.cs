using System.Text;
namespace ParseKit.Util;
/// <summary>
/// Generic Class for Array based parsing.
/// </summary>
/// <typeparam name="T">List Item Type</typeparam>
public class ArrayReader<T> {
	T[] arr;
	int i;
	public ArrayReader(T[] arr) {
		this.arr = arr;
		this.i = 0;
	}
	public T Peekc() {
		if(i >= arr.Length) {
			throw new Exception($"IndexOutOfRangeException() -> '{ToString()}'");
			throw new IndexOutOfRangeException();
		}
		return arr[i];
	}
	public bool Peekc(T value) {
		if(i >= arr.Length) {
			PrintError();
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		if(arr[i].Equals(value)) return true; else return false;
	}
	public T Peekn() {
		if(i+1 >= arr.Length) {
			foreach (var item in arr) {
				Console.Write(item + " | ");
			}
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		return arr[i+1];
	}
	public bool Peekn(T value) {
		if(i >= arr.Length) {
			foreach (var item in arr) {
				Console.Write(item + " | ");
			}
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		if(arr[i+1].Equals(value)) return true; else return false;
	}
	public T Consume(string from = null) {
		if(i >= arr.Length) {
			foreach (var item in arr) {
				Console.Write(item + " | ");
			}
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		return arr[i++];
	}
	public void Incr() {
		i++;
	}
	public bool Incr(T expected, bool ret = false) {
		if (!ret) {
			if (i >= arr.Length) throw new Exception($"Expected: '{expected}' got 'index out of bounds'");
			if (arr[i].Equals(expected)) {
				i++;
			} else {
				PrintError();
				throw new Exception($"Expected: '{expected}' got '{arr[i]}'");
			}
			return true;
		} else {
			try {
				if (i >= arr.Length) throw new Exception($"Expected: '{expected}' got 'index out of bounds'");
				if (arr[i].Equals(expected)) {
					i++;
				} else {
					PrintError();
					throw new Exception($"Expected: '{expected}' got '{arr[i]}'");
				}
				return true;
			}
			catch (Exception e) {
				return false;
			}
		}
	}
	public void Incr(params T[] expected) {
		foreach (var item in expected) {
			Incr(item);
		}
	}
	public void set(int i) {
		this.i = i;
	}
	public int get() {
		return this.i;
	}
	public bool IsEOF() {
		if (i < arr.Length) {
			return true;
		} else {
			return false;
		}
	}
	public void PrintError() {
		Console.WriteLine($"i: {i}");
		Console.WriteLine($"current: {Peekc()}");
	}
	public string PrintErrorS() {
		StringBuilder sb = new();
		sb.Append($"i: {i}");
		sb.Append($"current: {Peekc()}");
		return sb.ToString();
	}
	public override string ToString() {
		StringBuilder sb = new();
		foreach (var item in arr) {
			sb.Append(item + "|");
		}
		return sb.ToString();
	}
	public void ContinueUntil(T str) {
		while (!arr[i].Equals(str)) {
			i++;
		}
	}
	public bool LineContainsNumber() {
		var searchset = arr[i..(i+15)];
		if(arr is string[]) //gatekeep
		foreach (var item in searchset) {
			if (item.Equals("\n")) {
				return false;
			}
			if (item is string) {
				if (item.ToString().ContainsNumbers())
					return true;
			}
		}
		return false;
	}
	public int CountUntil(T str) { //FIXME: Zeitfresser
		var tmp_idx = i;
		for (int j = i; j < arr.Length; j++) {
			if (arr[j].Equals(str)) {
				return j - tmp_idx;
			}
		}
		return -1;
	}
}
