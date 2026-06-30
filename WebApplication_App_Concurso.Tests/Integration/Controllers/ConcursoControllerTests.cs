// Integration/Controllers/ConcursoControllerTests.cs
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;
using WebApplication_App_Concurso.Repositories.Contexts;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Tests.Integration.Setup;
using Xunit;

namespace WebApplication_App_Concurso.Tests.Integration.Controllers;
public class ConcursoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ConcursoControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ===========================
    // GetConcursos
    // ===========================
    #region GetConcursos[Fact]
    [Fact]
    public async Task GetConcursos_DeveRetornar200_QuandoChamado()
    {
        var response = await _client.GetAsync("/api/Concurso");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetConcursos_DeveRetornarPaginado_QuandoExistiremConcursos()
    {
        var response = await _client.GetAsync("/api/Concurso?PageNumber=1&PageSize=10");
        var resultado = await response.Content.ReadFromJsonAsync<PaginacaoDTO<ConcursoDTO>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado.Should().NotBeNull();
        resultado!.Data.Should().NotBeEmpty();
        resultado.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetConcursos_DeveRetornarListaVazia_QuandoFiltroNaoEncontrar()
    {
        var response = await _client.GetAsync("/api/Concurso?PageNumber=1&PageSize=10&Titulo=XYZ_NAO_EXISTE");
        var resultado = await response.Content.ReadFromJsonAsync<PaginacaoDTO<ConcursoDTO>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado!.Data.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetConcursos_DeveRetornarFiltrado_QuandoFiltroEstadoForPassado()
    {
        var response = await _client.GetAsync("/api/Concurso?PageNumber=1&PageSize=10&Estados=AL");
        var resultado = await response.Content.ReadFromJsonAsync<PaginacaoDTO<ConcursoDTO>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado!.Data.Should().OnlyContain(c => c.Estado == "AL");
    }

    [Fact]
    public async Task GetConcursos_DeveRetornar400_QuandoPageNumberExcederTotalPages()
    {
        var response = await _client.GetAsync("/api/Concurso?PageNumber=99&PageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    #endregion

    // ===========================
    // GetEstados
    // ===========================
    #region GetEstados[Fact]
    [Fact]
    public async Task GetEstados_DeveRetornar200_QuandoChamado()
    {
        var response = await _client.GetAsync("/api/Concurso/estados");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEstados_DeveRetornarEstadosDisponiveis_QuandoExistiremConcursos()
    {
        var response = await _client.GetAsync("/api/Concurso/estados");
        var resultado = await response.Content.ReadFromJsonAsync<List<string>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado.Should().Contain("AL");
        resultado.Should().Contain("SP");
    }
    #endregion

    // ===========================
    // GetEstatisticas
    // ===========================
    #region GetEstatisticas[Fact]
    [Fact]
    public async Task GetEstatisticas_DeveRetornar200_QuandoChamado()
    {
        var response = await _client.GetAsync("/api/Concurso/Estatisticas");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEstatisticas_DeveRetornarEstatisticas_QuandoExistiremConcursos()
    {
        var response = await _client.GetAsync("/api/Concurso/Estatisticas");
        var resultado = await response.Content.ReadFromJsonAsync<EstatisticaDTO>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado!.TotalDeConcursos.Should().BeGreaterThan(0);
        resultado.TotalDeVagas.Should().BeGreaterThan(0);
        resultado.TotalDeFontes.Should().BeGreaterThan(0);
    }
    #endregion

    // ===========================
    // GetAllConcursos
    // ===========================
    #region GetAllConcursos[Fact]
    [Fact]
    public async Task GetAllConcursos_DeveRetornar200_QuandoChamado()
    {
        var response = await _client.GetAsync("/api/Concurso/All");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllConcursos_DeveRetornarTodosOsConcursos()
    {
        // Act
        var response = await _client.GetAsync("/api/Concurso/All");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resultado = await response.Content.ReadFromJsonAsync<List<ConcursoDTO>>();

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(3);
    }
    #endregion

    // ===========================
    // GetAllFontesConcurso
    // ===========================
    #region GetAllFontesConcurso[Fact]
    [Fact]
    public async Task GetAllFontesConcurso_DeveRetornar200_QuandoChamado()
    {
        var response = await _client.GetAsync("/api/Concurso/ConcursoFontes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllFontesConcurso_DeveRetornarFontesDisponiveis()
    {
        // Act
        var response = await _client.GetAsync("/api/Concurso/ConcursoFontes");
        var resultado = await response.Content.ReadFromJsonAsync<List<FonteDTO>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        resultado.Should().NotBeEmpty();
        resultado.Should().Contain(f => f.FonteNome == "PCIConcursos");
        resultado.Should().Contain(f => f.FonteNome == "ConcursosBrasil");
    }
    #endregion
}
