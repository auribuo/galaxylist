namespace Galaxylist.Models;

using System.Text.Json;

public abstract class JsonStringer
{
	public override string ToString() =>
		JsonSerializer.Serialize(this,
								 new JsonSerializerOptions()
								 {
									 WriteIndented = true,
								 }
		);
}