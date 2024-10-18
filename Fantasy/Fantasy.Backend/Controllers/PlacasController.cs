using EFCore.BulkExtensions;
using Fantasy.Backend.Data;
using Fantasy.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacasController : ControllerBase
{
	private readonly DataContext _context;

	public PlacasController(DataContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> PostAsync(SetListaNegra listaNegra)
	{
		var date = DateTime.Now;
		var placasProcesadas = await SaveList(listaNegra);
		var finish = DateTime.Now;
		if (placasProcesadas == 0)
			return NotFound();
		var dates = finish - date;
		return Ok(new { mensaje = "Registros procesados", cantidad = placasProcesadas });
	}

	[HttpGet]
	public async Task<IActionResult> GetAsync()
	{
		return Ok(await _context.Placas.ToListAsync());
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetAsync(string id)
	{
		return Ok(await _context.Placas.FirstOrDefaultAsync(x => x.Placa == id));
	}

	private async Task<int> SaveList(SetListaNegra listaNegra)
	{
		char[] chrSeparadores = { ';' };
		int batchSize = 10000; // Tamaño del lote
		int totalProcesadas = 0;

		try
		{
			using (StreamReader sr = new StreamReader(listaNegra.Ruta))
			{
				string linea;
				var lstPlacas = new List<ListaNegraWebV2>();

				while ((linea = await sr.ReadLineAsync()) != null)
				{
					var arrFila = linea.Split(chrSeparadores);
					if (await EsFilaValida(arrFila))
					{
						var placaDes = Encryt.Encrypt(arrFila[0], "1nt3l*s0lucion3$MoviLp4rk&N6");
						var placa = new ListaNegraWebV2()
						{
							Placa = placaDes,
							PlacaDesencriptada = arrFila[0],
							Comentario = arrFila[2],
							Fecha = arrFila[3],
							Soat = arrFila[2] == "SIN SOAT" ? "0" : "1",
							Rtm = arrFila[2] == "SIN RTM" ? "0" : "1",
						};
						lstPlacas.Add(placa);

						if (lstPlacas.Count >= batchSize)
						{
							//await _context.PlacasPruebas.AddRangeAsync(lstPlacas);
							await _context.BulkInsertAsync(lstPlacas);
							await _context.SaveChangesAsync();
							totalProcesadas += lstPlacas.Count;
							lstPlacas.Clear(); // Limpiar lista después de insertar
						}
					}
				}

				// Guardar el resto de registros
				if (lstPlacas.Any())
				{
					//await _context.PlacasPruebas.AddRangeAsync(lstPlacas);
					await _context.BulkInsertAsync(lstPlacas);
					await _context.SaveChangesAsync();
					totalProcesadas += lstPlacas.Count;
				}
			}

			return totalProcesadas;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error al procesar el archivo: " + ex.Message);
			return 0;
		}
	}

	private async Task<bool> EsFilaValida(string[] arrFila)
	{
		return arrFila.Length > 1 &&
			   !string.IsNullOrEmpty(arrFila[0]) &&
			   !string.IsNullOrEmpty(arrFila[1]) &&
			   !string.IsNullOrEmpty(arrFila[2]);
	}
}