// Unit/Services/ConcursoServiceTests.cs
using FluentAssertions;
using Moq;
using WebApplication_App_Concurso.DTOs;
using WebApplication_App_Concurso.Exceptions;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Models.Filters;
using WebApplication_App_Concurso.Repositories.Interfaces;
using WebApplication_App_Concurso.Scrapers.Interfaces;
using WebApplication_App_Concurso.Services;

namespace WebApplication_App_Concurso.Tests.Unit.Services;

public class ConcursoServiceTests
{
    private readonly Mock<IConcursoRepository> _repositoryMock;
    private readonly Mock<IScrapingService> _scraperMock;
    private readonly EstadoNormalizadorService _normalizador;
    private readonly ConcursoService _sut;

    public ConcursoServiceTests()
    {
        _repositoryMock = new Mock<IConcursoRepository>();
        _scraperMock = new Mock<IScrapingService>();
        _normalizador = new EstadoNormalizadorService();

        _sut = new ConcursoService(
            new[] { _scraperMock.Object },
            _repositoryMock.Object,
            _normalizador
        );
    }
    // ===========================
    // GetAllConcursosAsync
    // ===========================
    #region GetAllConcursosAsync
    [Fact]
    public async Task GetAllConcursosAsync_DeveRetornarListaDTO_QuandoExistiremConcursos()
    {
        // Arrange
        var concursos = new List<Concurso>
    {
        new Concurso
        {
            ConcursoId = Guid.NewGuid(),
            Titulo = "TJSC",
            Estado = "SC",
            Fonte = "PCI"
        }
    };

        _repositoryMock
            .Setup(r => r.GetAllConcursosAsync())
            .ReturnsAsync(concursos);

        // Act
        var resultado = await _sut.GetAllConcursosAsync();

        // Assert
        resultado.Should().HaveCount(1);

        resultado.First().Titulo.Should().Be("TJSC");
        resultado.First().Estado.Should().Be("SC");
        resultado.First().Fonte.Should().Be("PCI");
    }

    [Fact]
    public async Task GetAllConcursosAsync_DeveLancarException_QuandoListaVazia()
    {
        _repositoryMock
            .Setup(r => r.GetAllConcursosAsync())
            .ReturnsAsync(new List<Concurso>());

        Func<Task> act = async () => await _sut.GetAllConcursosAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("No concursos found in the repository.");
    }

    [Fact]
    public async Task GetAllConcursosAsync_DeveLancarException_QuandoRepositorioRetornarNull()
    {
        _repositoryMock
            .Setup(r => r.GetAllConcursosAsync())
            .ReturnsAsync((List<Concurso>)null!);

        Func<Task> act = async () => await _sut.GetAllConcursosAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("No concursos found in the repository.");
    }

    [Fact]
    public async Task GetAllConcursosAsync_DeveLancarException_QuandoExistirConcursoNull()
    {
        var concursos = new List<Concurso?>
    {
        new Concurso
        {
            ConcursoId = Guid.NewGuid(),
            Titulo = "TJSC"
        },
        null
    };

        _repositoryMock
            .Setup(r => r.GetAllConcursosAsync())
            .ReturnsAsync(concursos!);

        Func<Task> act = async () => await _sut.GetAllConcursosAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("One or more concursos in the repository are null.");
    }

    [Fact]
    public async Task GetAllConcursosAsync_DeveEncapsularException()
    {
        _repositoryMock
            .Setup(r => r.GetAllConcursosAsync())
            .ThrowsAsync(new Exception("Erro BD"));

        Func<Task> act = async () => await _sut.GetAllConcursosAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("Error while fetching concursos.");
    }

    #endregion

    // ===========================
    // GetConcursosAsync
    // ===========================
    #region GetConcursosAsync
    [Fact]
    public async Task GetConcursosAsync_DeveRetornarPaginado_QuandoExistiremConcursos()
    {
        // Arrange
        var filterDTO = new ConcursoFilterDTO { PageNumber = 1, PageSize = 10 };
        var concursos = new List<Concurso>
        {
            new() { ConcursoId = Guid.NewGuid(), Titulo = "Concurso 1", Estado = "AL" },
            new() { ConcursoId = Guid.NewGuid(), Titulo = "Concurso 2", Estado = "SP" },
        };

        _repositoryMock
            .Setup(r => r.GetConcursosAsync(It.IsAny<ConcursoFilter>()))
            .ReturnsAsync((concursos, 2));

        // Act
        var resultado = await _sut.GetConcursosAsync(filterDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().HaveCount(2);
        resultado.TotalCount.Should().Be(2);
        resultado.TotalPages.Should().Be(1);
        resultado.PageNumber.Should().Be(1);
        resultado.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetConcursosAsync_DeveRetornarListaVazia_QuandoNaoExistiremConcursos()
    {
        // Arrange
        var filterDTO = new ConcursoFilterDTO { PageNumber = 1, PageSize = 10 };

        _repositoryMock
            .Setup(r => r.GetConcursosAsync(It.IsAny<ConcursoFilter>()))
            .ReturnsAsync((new List<Concurso>(), 0));

        // Act
        var resultado = await _sut.GetConcursosAsync(filterDTO);

        // Assert
        resultado.Data.Should().BeEmpty();
        resultado.TotalCount.Should().Be(0);
        resultado.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetConcursosAsync_DeveLancarExcecao_QuandoPageNumberExcederTotalPages()
    {
        // Arrange
        var filterDTO = new ConcursoFilterDTO { PageNumber = 99, PageSize = 10 };

        _repositoryMock
            .Setup(r => r.GetConcursosAsync(It.IsAny<ConcursoFilter>()))
            .ReturnsAsync((new List<Concurso>(), 5));

        // Act
        var act = async () => await _sut.GetConcursosAsync(filterDTO);

        // Assert
        await act.Should().ThrowAsync<ConcursoServiceException>()
            .WithMessage("*exceeds total pages*");
    }

    [Fact]
    public async Task GetConcursosAsync_DeveNormalizarEstados_QuandoFiltroEstadoForPassado()
    {
        // Arrange
        var filterDTO = new ConcursoFilterDTO
        {
            PageNumber = 1,
            PageSize = 10,
            Estados = new List<string> { "sp", "al" }
        };

        _repositoryMock
            .Setup(r => r.GetConcursosAsync(It.IsAny<ConcursoFilter>()))
            .ReturnsAsync((new List<Concurso>(), 0));

        // Act
        await _sut.GetConcursosAsync(filterDTO);

        // Assert — verifica que chegou normalizado no repository
        _repositoryMock.Verify(r => r.GetConcursosAsync(
            It.Is<ConcursoFilter>(f =>
                f.Estados != null &&
                f.Estados.Contains("SP") &&
                f.Estados.Contains("AL")
            )
        ), Times.Once);
    }
    #endregion

    // ===========================
    // CreateConcursosAsync
    // ===========================
    #region CreateConcursosAsync
    [Fact]
    public async Task CreateConcursosAsync_DeveSalvarConcursos_QuandoScraperRetornarDados()
    {
        // Arrange
        var concursosDTO = new List<ConcursoDTO>
        {
            new() { Titulo = "Concurso AL", Estado = "Alagoas", Vagas = "10", Fonte = "PCIConcursos" },
            new() { Titulo = "Concurso SP", Estado = "São Paulo", Vagas = "20", Fonte = "PCIConcursos" },
        };

        var entidadesSalvas = concursosDTO.Select(c => new Concurso
        {
            ConcursoId = Guid.NewGuid(),
            Titulo = c.Titulo,
            Estado = c.Estado,
        }).ToList();

        _scraperMock
            .Setup(s => s.GetAllConcursosAsync())
            .ReturnsAsync(concursosDTO);

        _repositoryMock
            .Setup(r => r.ClearAllConcursosAsync())
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveConcursosAsync(It.IsAny<List<Concurso>>()))
            .ReturnsAsync(entidadesSalvas);

        // Act
        var resultado = await _sut.CreateConcursosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.SaveConcursosAsync(It.IsAny<List<Concurso>>()), Times.Once);
    }

    [Fact]
    public async Task CreateConcursosAsync_DeveLancarExcecao_QuandoScraperNaoRetornarDados()
    {
        // Arrange
        _scraperMock
            .Setup(s => s.GetAllConcursosAsync())
            .ReturnsAsync(new List<ConcursoDTO>());

        _repositoryMock
            .Setup(r => r.ClearAllConcursosAsync())
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _sut.CreateConcursosAsync();

        // Assert
        await act.Should().ThrowAsync<ConcursoServiceException>()
            .WithMessage("*No concursos found*");
    }

    [Fact]
    public async Task CreateConcursosAsync_DeveNormalizarEstados_QuandoSalvarConcursos()
    {
        // Arrange
        var concursosDTO = new List<ConcursoDTO>
        {
            new() { Titulo = "Concurso", Estado = "Alagoas", Vagas = "10", Fonte = "PCIConcursos" },
        };

        _scraperMock
            .Setup(s => s.GetAllConcursosAsync())
            .ReturnsAsync(concursosDTO);

        _repositoryMock
            .Setup(r => r.ClearAllConcursosAsync())
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveConcursosAsync(It.IsAny<List<Concurso>>()))
            .ReturnsAsync(new List<Concurso> { new() { ConcursoId = Guid.NewGuid(), Titulo = "Concurso", Estado = "AL" } });

        // Act
        await _sut.CreateConcursosAsync();

        // Assert — verifica que o estado foi normalizado antes de salvar
        _repositoryMock.Verify(r => r.SaveConcursosAsync(
            It.Is<List<Concurso>>(list =>
                list.Any(c => c.Estado == "AL")
            )
        ), Times.Once);
    }
    #endregion

    // ===========================
    // GetEstatisticasAsync
    // ===========================
    #region GetEstatisticasAsync
    [Fact]
    public async Task GetEstatisticasAsync_DeveRetornarEstatisticas_QuandoChamado()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetTotalConcursosAsync()).ReturnsAsync(100);
        _repositoryMock.Setup(r => r.GetTotalVagasAsync()).ReturnsAsync(5000);
        _repositoryMock.Setup(r => r.GetTotalFontesAsync()).ReturnsAsync(2);

        // Act
        var resultado = await _sut.GetEstatisticasAsync();

        // Assert
        resultado.TotalDeConcursos.Should().Be(100);
        resultado.TotalDeVagas.Should().Be(5000);
        resultado.TotalDeFontes.Should().Be(2);
    }
    #endregion

    // ===========================
    // GetEstadosDisponiveisAsync
    // ===========================
    #region GetEstadosDisponiveisAsync
    [Fact]
    public async Task GetEstadosDisponiveisAsync_DeveRetornarEstados_QuandoExistiremConcursos()
    {
        // Arrange
        var estados = new List<string> { "AL", "SP", "NACIONAL" };

        _repositoryMock
            .Setup(r => r.GetEstadosDisponiveisAsync())
            .ReturnsAsync(estados);

        // Act
        var resultado = await _sut.GetEstadosDisponiveisAsync();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().Contain("AL");
        resultado.Should().Contain("SP");
        resultado.Should().Contain("NACIONAL");
    }

    [Fact]
    public async Task GetEstadosDisponiveisAsync_DeveRetornarListaVazia_QuandoNaoExistiremConcursos()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetEstadosDisponiveisAsync())
            .ReturnsAsync(new List<string>());

        // Act
        var resultado = await _sut.GetEstadosDisponiveisAsync();

        // Assert
        resultado.Should().BeEmpty();
    }
    #endregion

    // ===========================
    // GetAllFontesConcursoAsync
    // ===========================
    #region GetAllFontesConcursoAsync
    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveRetornarListaDTO()
    {
        var fontes = new List<string>
    {
        "PCI",
        "Concursos no Brasil"
    };

        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ReturnsAsync(fontes);

        var resultado = await _sut.GetAllFontesConcursoAsync();

        resultado.Should().HaveCount(2);

        resultado[0].FonteNome.Should().Be("PCI");
        resultado[1].FonteNome.Should().Be("Concursos no Brasil");
    }

    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveLancarException_QuandoListaVazia()
    {
        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ReturnsAsync(new List<string>());

        Func<Task> act = async () => await _sut.GetAllFontesConcursoAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("No fontes found in the repository.");
    }

    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveLancarException_QuandoRepositorioRetornarNull()
    {
        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ReturnsAsync((List<string>)null!);

        Func<Task> act = async () => await _sut.GetAllFontesConcursoAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("No fontes found in the repository.");
    }

    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveLancarException_QuandoFonteVazia()
    {
        var fontes = new List<string>
    {
        "PCI",
        ""
    };

        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ReturnsAsync(fontes);

        Func<Task> act = async () => await _sut.GetAllFontesConcursoAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("One or more fontes in the repository are null or empty.");
    }

    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveLancarException_QuandoFonteNull()
    {
        var fontes = new List<string?>
    {
        "PCI",
        null
    };

        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ReturnsAsync(fontes!);

        Func<Task> act = async () => await _sut.GetAllFontesConcursoAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("One or more fontes in the repository are null or empty.");
    }

    [Fact]
    public async Task GetAllFontesConcursoAsync_DeveEncapsularException()
    {
        _repositoryMock
            .Setup(r => r.GetAllFontesConcursoAsync())
            .ThrowsAsync(new Exception("Erro"));

        Func<Task> act = async () => await _sut.GetAllFontesConcursoAsync();

        await act.Should()
            .ThrowAsync<ConcursoServiceException>()
            .WithMessage("Error while fetching fontes.");
    }
    #endregion
}