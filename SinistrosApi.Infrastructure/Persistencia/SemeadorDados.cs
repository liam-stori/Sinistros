using Microsoft.EntityFrameworkCore;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Infrastructure.Persistencia;

public static class SemeadorDados
{
    public static async Task SemearAsync(SinistrosApiDbContext context)
    {
        if (await context.Clientes.AnyAsync())
            return;

        var clientes = CriarClientes();
        await context.Clientes.AddRangeAsync(clientes);

        var apolices = CriarApolices(clientes);
        await context.Apolices.AddRangeAsync(apolices);

        var sinistros = CriarSinistros(apolices);
        await context.Sinistros.AddRangeAsync(sinistros);

        await context.SaveChangesAsync();
    }

    private static List<Cliente> CriarClientes() =>
    [
        // Pessoas físicas
        CriarCliente("João",     "Silva",       TipoDocumento.Cpf,  "52998224725",    new DateOnly(1985,  3, 15)),
        CriarCliente("Maria",    "Santos",      TipoDocumento.Cpf,  "98765432100",    new DateOnly(1990,  7, 22)),
        CriarCliente("Carlos",   "Oliveira",    TipoDocumento.Cpf,  "91289316023",    new DateOnly(1978, 11,  5)),
        CriarCliente("Pedro",    "Costa",       TipoDocumento.Cpf,  "35719264809",    new DateOnly(1982,  6, 10)),
        CriarCliente("Lucia",    "Ferreira",    TipoDocumento.Cpf,  "47283561919",    new DateOnly(1976,  9,  3)),
        CriarCliente("Roberto",  "Almeida",     TipoDocumento.Cpf,  "81649375255",    new DateOnly(1989,  4, 25)),
        CriarCliente("Fernanda", "Lima",        TipoDocumento.Cpf,  "23854716982",    new DateOnly(1993, 12,  8)),
        CriarCliente("Eduardo",  "Martins",     TipoDocumento.Cpf,  "64927835146",    new DateOnly(1971,  8, 17)),
        CriarCliente("Rafael",   "Gonçalves",   TipoDocumento.Cpf,  "58329176428",    new DateOnly(1987,  5, 30)),
        CriarCliente("Camila",   "Rodrigues",   TipoDocumento.Cpf,  "72584619300",    new DateOnly(1994,  2, 18)),
        CriarCliente("Marcos",   "Carvalho",    TipoDocumento.Cpf,  "36915842764",    new DateOnly(1980, 10, 12)),
        CriarCliente("Patrícia", "Nascimento",  TipoDocumento.Cpf,  "94726385119",    new DateOnly(1975,  7,  4)),
        CriarCliente("Thiago",   "Barbosa",     TipoDocumento.Cpf,  "18547926364",    new DateOnly(1991,  1, 27)),
        CriarCliente("Juliana",  "Cardoso",     TipoDocumento.Cpf,  "63851497228",    new DateOnly(1983, 11, 14)),
        CriarCliente("André",    "Moreira",     TipoDocumento.Cpf,  "49637258191",    new DateOnly(1977,  3, 22)),
        CriarCliente("Vanessa",  "Ribeiro",     TipoDocumento.Cpf,  "27493615837",    new DateOnly(1996,  8,  9)),
        CriarCliente("Felipe",   "Araújo",      TipoDocumento.Cpf,  "85274163955",    new DateOnly(1988,  6, 16)),
        CriarCliente("Larissa",  "Mendes",      TipoDocumento.Cpf,  "14763952846",    new DateOnly(1992,  4,  3)),
        CriarCliente("Bruno",    "Teixeira",    TipoDocumento.Cpf,  "69258374100",    new DateOnly(1979, 12, 20)),
        CriarCliente("Isabela",  "Campos",      TipoDocumento.Cpf,  "38169427509",    new DateOnly(1997,  9, 11)),
        // Pessoas jurídicas
        CriarCliente("Ana",      "Pereira",     TipoDocumento.Cnpj, "11222333000181", new DateOnly(1995,  1, 30)),
        CriarCliente("Beatriz",  "Souza",       TipoDocumento.Cnpj, "45678912000155", new DateOnly(1988,  2, 14)),
        CriarCliente("Ricardo",  "Monteiro",    TipoDocumento.Cnpj, "98765432000198", new DateOnly(2000,  5,  1)),
        CriarCliente("Sandra",   "Figueiredo",  TipoDocumento.Cnpj, "23456789000195", new DateOnly(2005,  3, 15)),
        CriarCliente("Diego",    "Fonseca",     TipoDocumento.Cnpj, "56789123000108", new DateOnly(2010,  7, 20)),
    ];

    private static Cliente CriarCliente(string nome, string sobrenome, TipoDocumento tipo, string documento, DateOnly nascimento)
    {
        var resultado = Cliente.Criar(nome, sobrenome, tipo, documento, nascimento);
        if (resultado.EhFalha)
            throw new InvalidOperationException($"Falha ao criar cliente de seed: {resultado.Erro}");
        return resultado.Valor;
    }

    private static Apolice CriarApolice(string numero, Guid clienteId, Ramo ramo, DateOnly inicio, DateOnly fim)
    {
        var resultado = Apolice.Criar(numero, clienteId, ramo, inicio, fim);
        if (resultado.EhFalha)
            throw new InvalidOperationException($"Falha ao criar apólice de seed: {resultado.Erro}");
        return resultado.Valor;
    }

    private static List<Apolice> CriarApolices(List<Cliente> clientes)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        // Intervalos variados: ativa (1 ano), recente (6 meses), longa (2 anos), vencida (encerrou há 6 meses), antiga (encerrou há 1 ano)
        var ativa = new PeriodoVigencia(hoje.AddYears(-1), hoje.AddYears(1));
        var recente = new PeriodoVigencia(hoje.AddMonths(-6), hoje.AddMonths(18));
        var longa = new PeriodoVigencia(hoje.AddYears(-2), hoje.AddYears(1));
        var vencida = new PeriodoVigencia(hoje.AddYears(-2), hoje.AddMonths(-6));
        var antiga = new PeriodoVigencia(hoje.AddYears(-3), hoje.AddYears(-1));

        List<Apolice> apolices =
        [
            // João Silva [0]
            CriarApolice("APL-001", clientes[0].Id, Ramo.Auto,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-002", clientes[0].Id, Ramo.Residencial, longa.Inicio,   longa.Fim),
            // Maria Santos [1]
            CriarApolice("APL-003", clientes[1].Id, Ramo.Vida,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-004", clientes[1].Id, Ramo.Saude,       ativa.Inicio,   ativa.Fim),
            // Carlos Oliveira [2]
            CriarApolice("APL-005", clientes[2].Id, Ramo.Auto,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-006", clientes[2].Id, Ramo.Residencial, antiga.Inicio,  antiga.Fim),
            // Pedro Costa [3]
            CriarApolice("APL-007", clientes[3].Id, Ramo.Auto,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-008", clientes[3].Id, Ramo.Residencial, recente.Inicio, recente.Fim),
            // Lucia Ferreira [4]
            CriarApolice("APL-009", clientes[4].Id, Ramo.Vida,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-010", clientes[4].Id, Ramo.Saude,       vencida.Inicio, vencida.Fim),
            // Roberto Almeida [5]
            CriarApolice("APL-011", clientes[5].Id, Ramo.Auto,        longa.Inicio,   longa.Fim),
            CriarApolice("APL-012", clientes[5].Id, Ramo.Residencial, antiga.Inicio,  antiga.Fim),
            // Fernanda Lima [6]
            CriarApolice("APL-013", clientes[6].Id, Ramo.Saude,       recente.Inicio, recente.Fim),
            CriarApolice("APL-014", clientes[6].Id, Ramo.Vida,        ativa.Inicio,   ativa.Fim),
            // Eduardo Martins [7]
            CriarApolice("APL-015", clientes[7].Id, Ramo.Auto,        longa.Inicio,   longa.Fim),
            // Rafael Gonçalves [8]
            CriarApolice("APL-016", clientes[8].Id, Ramo.Auto,        ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-017", clientes[8].Id, Ramo.Vida,        vencida.Inicio, vencida.Fim),
            // Camila Rodrigues [9]
            CriarApolice("APL-018", clientes[9].Id, Ramo.Saude,       recente.Inicio, recente.Fim),
            // Marcos Carvalho [10]
            CriarApolice("APL-019", clientes[10].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-020", clientes[10].Id, Ramo.Residencial,longa.Inicio,   longa.Fim),
            // Patrícia Nascimento [11]
            CriarApolice("APL-021", clientes[11].Id, Ramo.Vida,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-022", clientes[11].Id, Ramo.Saude,      antiga.Inicio,  antiga.Fim),
            // Thiago Barbosa [12]
            CriarApolice("APL-023", clientes[12].Id, Ramo.Auto,       recente.Inicio, recente.Fim),
            // Juliana Cardoso [13]
            CriarApolice("APL-024", clientes[13].Id, Ramo.Residencial,ativa.Inicio,   ativa.Fim),
            // André Moreira [14]
            CriarApolice("APL-025", clientes[14].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-026", clientes[14].Id, Ramo.Saude,      vencida.Inicio, vencida.Fim),
            // Vanessa Ribeiro [15]
            CriarApolice("APL-027", clientes[15].Id, Ramo.Vida,       longa.Inicio,   longa.Fim),
            CriarApolice("APL-028", clientes[15].Id, Ramo.Residencial,recente.Inicio, recente.Fim),
            // Felipe Araújo [16]
            CriarApolice("APL-029", clientes[16].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            // Larissa Mendes [17]
            CriarApolice("APL-030", clientes[17].Id, Ramo.Saude,      antiga.Inicio,  antiga.Fim),
            CriarApolice("APL-031", clientes[17].Id, Ramo.Vida,       ativa.Inicio,   ativa.Fim),
            // Bruno Teixeira [18]
            CriarApolice("APL-032", clientes[18].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-033", clientes[18].Id, Ramo.Residencial,recente.Inicio, recente.Fim),
            // Isabela Campos [19]
            CriarApolice("APL-034", clientes[19].Id, Ramo.Saude,      longa.Inicio,   longa.Fim),
            // Ana Pereira CNPJ [20]
            CriarApolice("APL-035", clientes[20].Id, Ramo.Vida,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-036", clientes[20].Id, Ramo.Saude,      recente.Inicio, recente.Fim),
            CriarApolice("APL-037", clientes[20].Id, Ramo.Auto,       longa.Inicio,   longa.Fim),
            // Beatriz Souza CNPJ [21]
            CriarApolice("APL-038", clientes[21].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-039", clientes[21].Id, Ramo.Residencial,vencida.Inicio, vencida.Fim),
            CriarApolice("APL-040", clientes[21].Id, Ramo.Vida,       recente.Inicio, recente.Fim),
            // Ricardo Monteiro CNPJ [22]
            CriarApolice("APL-041", clientes[22].Id, Ramo.Auto,       longa.Inicio,   longa.Fim),
            CriarApolice("APL-042", clientes[22].Id, Ramo.Residencial,ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-043", clientes[22].Id, Ramo.Vida,       antiga.Inicio,  antiga.Fim),
            CriarApolice("APL-044", clientes[22].Id, Ramo.Saude,      recente.Inicio, recente.Fim),
            // Sandra Figueiredo CNPJ [23]
            CriarApolice("APL-045", clientes[23].Id, Ramo.Auto,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-046", clientes[23].Id, Ramo.Residencial,longa.Inicio,   longa.Fim),
            // Diego Fonseca CNPJ [24]
            CriarApolice("APL-047", clientes[24].Id, Ramo.Vida,       ativa.Inicio,   ativa.Fim),
            CriarApolice("APL-048", clientes[24].Id, Ramo.Auto,       recente.Inicio, recente.Fim),
        ];

        // Apólices com vigência encerrada por prazo -> Vencida
        foreach (var i in new[] { 9, 16, 25, 38 })
            apolices[i].Vencer();

        // Apólices encerradas há mais tempo -> Inativa
        foreach (var i in new[] { 5, 11, 21, 29, 42 })
            apolices[i].Inativar();

        return apolices;
    }

    private static List<Sinistro> CriarSinistros(List<Apolice> apolices)
    {
        var sinistros = new List<Sinistro>();
        var agora = DateTime.UtcNow;

        // APL-001 João / Auto -> Encerrado (aberto há 18 meses)
        var d1 = agora.AddMonths(-18);
        var s1 = AbrirSinistro(apolices[0], 8500m, d1);
        Transitar(s1, StatusSinistro.EmAnalise,  data: d1.AddDays(5));
        Transitar(s1, StatusSinistro.Aprovado,   data: d1.AddDays(12));
        Transitar(s1, StatusSinistro.Encerrado,  data: d1.AddDays(20), valorAprovado: 7200m);
        sinistros.Add(s1);

        // APL-002 João / Residencial -> Negado (aberto há 14 meses)
        var d2 = agora.AddMonths(-14);
        var s2 = AbrirSinistro(apolices[1], 15000m, d2);
        Transitar(s2, StatusSinistro.EmAnalise, data: d2.AddDays(7));
        Transitar(s2, StatusSinistro.Negado,    data: d2.AddDays(15), motivo: "Dano pré-existente não coberto pela apólice.");
        sinistros.Add(s2);

        // APL-003 Maria / Vida -> EmAnalise (aberto há 10 meses)
        var d3 = agora.AddMonths(-10);
        var s3 = AbrirSinistro(apolices[2], 50000m, d3);
        Transitar(s3, StatusSinistro.EmAnalise, data: d3.AddDays(6));
        sinistros.Add(s3);

        // APL-004 Maria / Saude -> Aprovado (aberto há 8 meses)
        var d4 = agora.AddMonths(-8);
        var s4 = AbrirSinistro(apolices[3], 3200m, d4);
        Transitar(s4, StatusSinistro.EmAnalise, data: d4.AddDays(4));
        Transitar(s4, StatusSinistro.Aprovado,  data: d4.AddDays(10));
        sinistros.Add(s4);

        // APL-005 Carlos / Auto -> Encerrado (aberto há 23 meses)
        var d5 = agora.AddMonths(-23);
        var s5 = AbrirSinistro(apolices[4], 12000m, d5);
        Transitar(s5, StatusSinistro.EmAnalise, data: d5.AddDays(8));
        Transitar(s5, StatusSinistro.Aprovado,  data: d5.AddDays(16));
        Transitar(s5, StatusSinistro.Encerrado, data: d5.AddDays(25), valorAprovado: 10500m);
        sinistros.Add(s5);

        // APL-005 Carlos / Auto -> Aberto (segundo sinistro, aberto há 2 meses)
        sinistros.Add(AbrirSinistro(apolices[4], 4300m, agora.AddMonths(-2)));

        // APL-007 Pedro / Auto -> Negado (aberto há 16 meses)
        var d7 = agora.AddMonths(-16);
        var s7 = AbrirSinistro(apolices[6], 6800m, d7);
        Transitar(s7, StatusSinistro.EmAnalise, data: d7.AddDays(6));
        Transitar(s7, StatusSinistro.Negado,    data: d7.AddDays(14), motivo: "Acidente ocorrido fora da área de cobertura contratada.");
        sinistros.Add(s7);

        // APL-011 Roberto / Auto -> Encerrado (aberto há 21 meses)
        var d8 = agora.AddMonths(-21);
        var s8 = AbrirSinistro(apolices[10], 18000m, d8);
        Transitar(s8, StatusSinistro.EmAnalise, data: d8.AddDays(10));
        Transitar(s8, StatusSinistro.Aprovado,  data: d8.AddDays(18));
        Transitar(s8, StatusSinistro.Encerrado, data: d8.AddDays(30), valorAprovado: 15000m);
        sinistros.Add(s8);

        // APL-013 Fernanda / Saude -> EmAnalise (aberto há 3 meses)
        var d9 = agora.AddMonths(-3);
        var s9 = AbrirSinistro(apolices[12], 2100m, d9);
        Transitar(s9, StatusSinistro.EmAnalise, data: d9.AddDays(5));
        sinistros.Add(s9);

        // APL-016 Rafael / Auto -> Aberto (aberto há 1 mês)
        sinistros.Add(AbrirSinistro(apolices[15], 9500m, agora.AddMonths(-1)));

        // APL-019 Marcos / Auto -> EmAnalise (aberto há 6 meses)
        var d11 = agora.AddMonths(-6);
        var s11 = AbrirSinistro(apolices[18], 7300m, d11);
        Transitar(s11, StatusSinistro.EmAnalise, data: d11.AddDays(7));
        sinistros.Add(s11);

        // APL-021 Patrícia / Vida -> Negado (aberto há 12 meses)
        var d12 = agora.AddMonths(-12);
        var s12 = AbrirSinistro(apolices[20], 45000m, d12);
        Transitar(s12, StatusSinistro.EmAnalise, data: d12.AddDays(9));
        Transitar(s12, StatusSinistro.Negado,    data: d12.AddDays(20), motivo: "Documentação insuficiente para comprovação do sinistro.");
        sinistros.Add(s12);

        // APL-025 André / Auto -> Aprovado (aberto há 5 meses)
        var d13 = agora.AddMonths(-5);
        var s13 = AbrirSinistro(apolices[24], 11000m, d13);
        Transitar(s13, StatusSinistro.EmAnalise, data: d13.AddDays(6));
        Transitar(s13, StatusSinistro.Aprovado,  data: d13.AddDays(13));
        sinistros.Add(s13);

        // APL-027 Vanessa / Vida -> Encerrado (aberto há 20 meses)
        var d14 = agora.AddMonths(-20);
        var s14 = AbrirSinistro(apolices[26], 30000m, d14);
        Transitar(s14, StatusSinistro.EmAnalise, data: d14.AddDays(7));
        Transitar(s14, StatusSinistro.Aprovado,  data: d14.AddDays(15));
        Transitar(s14, StatusSinistro.Encerrado, data: d14.AddDays(22), valorAprovado: 28500m);
        sinistros.Add(s14);

        // APL-029 Felipe / Auto -> Aberto (aberto há 3 semanas)
        sinistros.Add(AbrirSinistro(apolices[28], 5600m, agora.AddDays(-21)));

        // APL-032 Bruno / Auto -> EmAnalise (aberto há 4 meses)
        var d16 = agora.AddMonths(-4);
        var s16 = AbrirSinistro(apolices[31], 13500m, d16);
        Transitar(s16, StatusSinistro.EmAnalise, data: d16.AddDays(8));
        sinistros.Add(s16);

        // APL-035 Ana Pereira / Vida -> Negado (aberto há 9 meses)
        var d17 = agora.AddMonths(-9);
        var s17 = AbrirSinistro(apolices[34], 25000m, d17);
        Transitar(s17, StatusSinistro.EmAnalise, data: d17.AddDays(10));
        Transitar(s17, StatusSinistro.Negado,    data: d17.AddDays(21), motivo: "Período de carência não cumprido para este tipo de cobertura.");
        sinistros.Add(s17);

        // APL-041 Ricardo Monteiro / Auto -> Encerrado (aberto há 22 meses)
        var d18 = agora.AddMonths(-22);
        var s18 = AbrirSinistro(apolices[40], 55000m, d18);
        Transitar(s18, StatusSinistro.EmAnalise, data: d18.AddDays(12));
        Transitar(s18, StatusSinistro.Aprovado,  data: d18.AddDays(22));
        Transitar(s18, StatusSinistro.Encerrado, data: d18.AddDays(35), valorAprovado: 48000m);
        sinistros.Add(s18);

        // APL-045 Sandra Figueiredo / Auto -> Aberto (aberto há 2 semanas)
        sinistros.Add(AbrirSinistro(apolices[44], 32000m, agora.AddDays(-14)));

        // APL-047 Diego Fonseca / Vida -> EmAnalise (aberto há 7 meses)
        var d20 = agora.AddMonths(-7);
        var s20 = AbrirSinistro(apolices[46], 80000m, d20);
        Transitar(s20, StatusSinistro.EmAnalise, data: d20.AddDays(9));
        sinistros.Add(s20);

        // APL-023 Thiago / Auto -> Negado (aberto há 5 meses)
        var da1 = agora.AddMonths(-5);
        var sa1 = AbrirSinistro(apolices[22], 9200m, da1);
        Transitar(sa1, StatusSinistro.EmAnalise, data: da1.AddDays(3));
        Transitar(sa1, StatusSinistro.Negado,    data: da1.AddDays(10), motivo: "Condutor sem habilitação no momento do acidente.");
        sinistros.Add(sa1);

        // APL-015 Eduardo / Auto -> Negado (aberto há 3 meses)
        var da2 = agora.AddMonths(-3);
        var sa2 = AbrirSinistro(apolices[14], 7800m, da2);
        Transitar(sa2, StatusSinistro.EmAnalise, data: da2.AddDays(4));
        Transitar(sa2, StatusSinistro.Negado,    data: da2.AddDays(11), motivo: "Sinistro ocorrido fora da cobertura territorial.");
        sinistros.Add(sa2);

        // APL-008 Pedro / Residencial -> Negado (aberto há 4 meses)
        var da3 = agora.AddMonths(-4);
        var sa3 = AbrirSinistro(apolices[7], 22000m, da3);
        Transitar(sa3, StatusSinistro.EmAnalise, data: da3.AddDays(5));
        Transitar(sa3, StatusSinistro.Negado,    data: da3.AddDays(12), motivo: "Imóvel desocupado, em desacordo com as condições gerais da apólice.");
        sinistros.Add(sa3);

        // APL-024 Juliana / Residencial -> EmAnalise (aberto há 3 meses)
        var da4 = agora.AddMonths(-3);
        var sa4 = AbrirSinistro(apolices[23], 18000m, da4);
        Transitar(sa4, StatusSinistro.EmAnalise, data: da4.AddDays(4));
        sinistros.Add(sa4);

        // APL-018 Camila / Saude -> Negado (aberto há 3 meses)
        var da5 = agora.AddMonths(-3);
        var sa5 = AbrirSinistro(apolices[17], 4500m, da5);
        Transitar(sa5, StatusSinistro.EmAnalise, data: da5.AddDays(3));
        Transitar(sa5, StatusSinistro.Negado,    data: da5.AddDays(8), motivo: "Procedimento eletivo não coberto pelo plano contratado.");
        sinistros.Add(sa5);

        // APL-036 Ana / Saude -> EmAnalise (aberto há 2 meses)
        var da6 = agora.AddMonths(-2);
        var sa6 = AbrirSinistro(apolices[35], 22000m, da6);
        Transitar(sa6, StatusSinistro.EmAnalise, data: da6.AddDays(3));
        sinistros.Add(sa6);

        // APL-014 Fernanda / Vida -> Negado (aberto há 2 meses)
        var da7 = agora.AddMonths(-2);
        var sa7 = AbrirSinistro(apolices[13], 35000m, da7);
        Transitar(sa7, StatusSinistro.EmAnalise, data: da7.AddDays(5));
        Transitar(sa7, StatusSinistro.Negado,    data: da7.AddDays(14), motivo: "Exclusão contratual por preexistência não declarada.");
        sinistros.Add(sa7);

        // APL-031 Larissa / Vida -> EmAnalise (aberto há 5 meses)
        var da8 = agora.AddMonths(-5);
        var sa8 = AbrirSinistro(apolices[30], 45000m, da8);
        Transitar(sa8, StatusSinistro.EmAnalise, data: da8.AddDays(4));
        sinistros.Add(sa8);

        // APL-009 Lucia / Vida -> Aprovado (aberto há 11 meses)
        var da9 = agora.AddMonths(-11);
        var sa9 = AbrirSinistro(apolices[8], 60000m, da9);
        Transitar(sa9, StatusSinistro.EmAnalise, data: da9.AddDays(5));
        Transitar(sa9, StatusSinistro.Aprovado,  data: da9.AddDays(12));
        sinistros.Add(sa9);

        // APL-038 Beatriz / Auto -> EmAnalise (aberto há 5 meses)
        var da10 = agora.AddMonths(-5);
        var sa10 = AbrirSinistro(apolices[37], 28000m, da10);
        Transitar(sa10, StatusSinistro.EmAnalise, data: da10.AddDays(6));
        sinistros.Add(sa10);

        // APL-042 Ricardo / Residencial -> Aprovado (aberto há 6 meses)
        var da11 = agora.AddMonths(-6);
        var sa11 = AbrirSinistro(apolices[41], 42000m, da11);
        Transitar(sa11, StatusSinistro.EmAnalise, data: da11.AddDays(7));
        Transitar(sa11, StatusSinistro.Aprovado,  data: da11.AddDays(14));
        sinistros.Add(sa11);

        // APL-020 Marcos / Residencial -> Encerrado (aberto há 18 meses)
        var da12 = agora.AddMonths(-18);
        var sa12 = AbrirSinistro(apolices[19], 22000m, da12);
        Transitar(sa12, StatusSinistro.EmAnalise, data: da12.AddDays(8));
        Transitar(sa12, StatusSinistro.Aprovado,  data: da12.AddDays(16));
        Transitar(sa12, StatusSinistro.Encerrado, data: da12.AddDays(28), valorAprovado: 19500m);
        sinistros.Add(sa12);

        // APL-046 Sandra / Residencial -> Encerrado (aberto há 17 meses)
        var da13 = agora.AddMonths(-17);
        var sa13 = AbrirSinistro(apolices[45], 55000m, da13);
        Transitar(sa13, StatusSinistro.EmAnalise, data: da13.AddDays(9));
        Transitar(sa13, StatusSinistro.Aprovado,  data: da13.AddDays(17));
        Transitar(sa13, StatusSinistro.Encerrado, data: da13.AddDays(28), valorAprovado: 50000m);
        sinistros.Add(sa13);

        // APL-034 Isabela / Saude -> Encerrado (aberto há 19 meses)
        var da14 = agora.AddMonths(-19);
        var sa14 = AbrirSinistro(apolices[33], 8500m, da14);
        Transitar(sa14, StatusSinistro.EmAnalise, data: da14.AddDays(6));
        Transitar(sa14, StatusSinistro.Aprovado,  data: da14.AddDays(13));
        Transitar(sa14, StatusSinistro.Encerrado, data: da14.AddDays(15), valorAprovado: 8000m);
        sinistros.Add(sa14);

        return sinistros;
    }

    private static Sinistro AbrirSinistro(Apolice apolice, decimal valorEstimado, DateTime dataAbertura)
    {
        var resultado = Sinistro.Abrir(apolice, valorEstimado, dataAbertura);
        if (resultado.EhFalha)
            throw new InvalidOperationException($"Falha ao abrir sinistro de seed: {resultado.Erro}");
        return resultado.Valor;
    }

    private static void Transitar(Sinistro sinistro, StatusSinistro novoStatus, DateTime data, string? motivo = null, decimal? valorAprovado = null)
    {
        var resultado = sinistro.AtualizarStatus(novoStatus, motivo, valorAprovado, data);
        if (resultado.EhFalha)
            throw new InvalidOperationException($"Falha ao transitar sinistro de seed [{novoStatus}]: {resultado.Erro}");
    }
}

internal record struct PeriodoVigencia(DateOnly Inicio, DateOnly Fim);