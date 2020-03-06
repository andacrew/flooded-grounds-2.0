using System.IO;
using UnityEngine;

public abstract class NetworkResponse {
	
	public MemoryStream dataStream { get; set; }
	
	public abstract void parse();
	public abstract ExtendedEventArgs process();
}
