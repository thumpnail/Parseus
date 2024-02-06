using System.Text;
namespace Parseus.Util;
/// <summary>
/// Generic Class for Array based parsing.
/// </summary>
/// <typeparam name="T">List Item Type</typeparam>
public class ArrayReader<T> {
	T[] arr;
	int idx;
	public ArrayReader(T[] arr) {
		this.arr = arr;
		this.idx = 0;
	}
	public T Peekc() {
		if(idx >= arr.Length) {
			throw new Exception($"IndexOutOfRangeException() -> '{ToString()}'");
			throw new IndexOutOfRangeException();
		}
		return arr[idx];
	}
	public bool Peekc(int value) {
		if(idx >= arr.Length) {
			PrintError();
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		if(arr[idx].Equals(value)) return true; else return false;
	}
	public T Peekn() {
		if(idx+1 >= arr.Length) {
			foreach (var item in arr) {
				Console.Write(item + " | ");
			}
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		return arr[idx+1];
	}
	public bool Peekn(int value) {
		if(idx >= arr.Length) {
			foreach (var item in arr) {
				Console.Write(item + " | ");
			}
			Console.WriteLine();
			throw new IndexOutOfRangeException();
		}
		if(arr[idx+1].Equals(value)) return true; else return false;
	}
	public T Consume() {
		if(idx >= arr.Length) {
			throw new IndexOutOfRangeException();
		}
		return arr[idx++];
	}
	public void Incr() {
		idx++;
	}
	public bool Incr(T expected, bool ret = false) {
		if (!ret) {
			if (idx >= arr.Length) throw new Exception($"Expected: '{expected}' got 'index out of bounds'");
			if (arr[idx].Equals(expected)) {
				idx++;
			} else {
				PrintError();
				throw new Exception($"Expected: '{expected}' got '{arr[idx]}'");
			}
			return true;
		} else {
			try {
				if (idx >= arr.Length) throw new Exception($"Expected: '{expected}' got 'index out of bounds'");
				if (arr[idx].Equals(expected)) {
					idx++;
				} else {
					PrintError();
					throw new Exception($"Expected: '{expected}' got '{arr[idx]}'");
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
		this.idx = i;
	}
	public int get() {
		return this.idx;
	}
	public bool IsEOF() {
		if (idx < arr.Length) {
			return true;
		} else {
			return false;
		}
	}
	public void PrintError() {
		Console.WriteLine($"i: {idx}");
		Console.WriteLine($"current: {Peekc()}");
	}
	public string PrintErrorS() {
		StringBuilder sb = new();
		sb.Append($"i: {idx}");
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

	//##################################################################################
	//##################################################################################
	//##################################################################################
	public void ContinueUntil(int str) {
		while (!arr[idx].Equals(str)) {
			idx++;
		}
	}

	public bool LineContainsNumber() {
		var searchset = arr[idx..(idx+15)];
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
	public int CountUntil(int str) { //FIXME: Zeitfresser
		var tmp_idx = idx;
		for (int j = idx; j < arr.Length; j++) {
			if (arr[j].Equals(str)) {
				return j - tmp_idx;
			}
		}
		return -1;
	}
}
