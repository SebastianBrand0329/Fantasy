namespace Fantasy.Shared.Entities;

public class SetListaNegra
{
	public List<ListaNegraWebV2>? Placas { get; set; }
	public string ClaveCargaArchivo { get; set; } = null!;
	public string Ruta { get; set; } = null!;
}

public class ListaNegraWeb
{
	public int Id { get; set; }
	public string Placa { get; set; } = null!;
	public string Comentario { get; set; } = null!;
	public string Fecha { get; set; } = null!;
	public string? PlacaDesencriptada { get; set; }
	public string Soat { get; set; } = null!;
	public string Rtm { get; set; } = null!;
}

public class ListaNegraWebV1
{
	public int Id { get; set; }
	public string Placa { get; set; } = null!;
	public string Comentario { get; set; } = null!;
	public string Fecha { get; set; } = null!;
	public string? PlacaDesencriptada { get; set; }
	public string Soat { get; set; } = null!;
	public string Rtm { get; set; } = null!;
}

public class ListaNegraWebV2
{
	public int Id { get; set; }
	public string Placa { get; set; } = null!;
	public string Comentario { get; set; } = null!;
	public string Fecha { get; set; } = null!;
	public string? PlacaDesencriptada { get; set; }
	public string Soat { get; set; } = null!;
	public string Rtm { get; set; } = null!;
}