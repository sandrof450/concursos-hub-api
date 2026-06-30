// Unit/Services/EstadoNormalizadorServiceTests.cs
using FluentAssertions;
using WebApplication_App_Concurso.Services;

namespace WebApplication_App_Concurso.Tests.Unit.Services;

public class EstadoNormalizadorServiceTests
{
    private readonly EstadoNormalizadorService _sut;

    public EstadoNormalizadorServiceTests()
    {
        _sut = new EstadoNormalizadorService();
    }

    // ✅ nulo e vazio
    [Fact]
    public void Normalizar_DeveRetornarNull_QuandoEstadoForNulo()
    {
        var resultado = _sut.Normalizar(null);
        resultado.Should().BeNull();
    }

    [Fact]
    public void Normalizar_DeveRetornarNull_QuandoEstadoForVazio()
    {
        var resultado = _sut.Normalizar("");
        resultado.Should().BeNull();
    }

    [Fact]
    public void Normalizar_DeveRetornarNull_QuandoEstadoForEspacoEmBranco()
    {
        var resultado = _sut.Normalizar("   ");
        resultado.Should().BeNull();
    }

    // ✅ siglas
    [Theory]
    [InlineData("AL", "AL")]
    [InlineData("al", "AL")]
    [InlineData("SP", "SP")]
    [InlineData("sp", "SP")]
    [InlineData("RJ", "RJ")]
    public void Normalizar_DeveRetornarSiglaEmMaiusculo_QuandoReceberSigla(string entrada, string esperado)
    {
        var resultado = _sut.Normalizar(entrada);
        resultado.Should().Be(esperado);
    }

    // ✅ nomes completos
    [Theory]
    [InlineData("Alagoas", "AL")]
    [InlineData("alagoas", "AL")]
    [InlineData("ALAGOAS", "AL")]
    [InlineData("São Paulo", "SP")]
    [InlineData("Minas Gerais", "MG")]
    [InlineData("Rio de Janeiro", "RJ")]
    public void Normalizar_DeveRetornarSigla_QuandoReceberNomeCompleto(string entrada, string esperado)
    {
        var resultado = _sut.Normalizar(entrada);
        resultado.Should().Be(esperado);
    }

    // ✅ variações com sigla junto
    [Theory]
    [InlineData("Alagoas - AL", "AL")]
    [InlineData("AL - Alagoas", "AL")]
    [InlineData("Alagoas AL", "AL")]
    [InlineData("São Paulo - SP", "SP")]
    [InlineData("SP - São Paulo", "SP")]
    public void Normalizar_DeveRetornarSigla_QuandoReceberVariacaoComSigla(string entrada, string esperado)
    {
        var resultado = _sut.Normalizar(entrada);
        resultado.Should().Be(esperado);
    }

    // ✅ Nacional
    [Theory]
    [InlineData("Nacional", "NACIONAL")]
    [InlineData("nacional", "NACIONAL")]
    [InlineData("NACIONAL", "NACIONAL")]
    [InlineData("Brasil", "NACIONAL")]
    [InlineData("Todo o Brasil", "NACIONAL")]
    [InlineData("Federal", "NACIONAL")]
    public void Normalizar_DeveRetornarNACIONAL_QuandoReceberVariacaoNacional(string entrada, string esperado)
    {
        var resultado = _sut.Normalizar(entrada);
        resultado.Should().Be(esperado);
    }

    // ✅ não reconhecido
    [Theory]
    [InlineData("XYZ")]
    [InlineData("Estado Inexistente")]
    [InlineData("123")]
    public void Normalizar_DeveRetornarNull_QuandoNaoReconhecer(string entrada)
    {
        var resultado = _sut.Normalizar(entrada);
        resultado.Should().BeNull();
    }
}