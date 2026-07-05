namespace SinistrosApi.Application.Sinistros.Dtos;

public record SinistrosPaginadosDto(
    IReadOnlyList<SinistroDto> Itens,
    int Total,
    int Pagina,
    int TamanhoPagina);
