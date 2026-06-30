using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Models.Filters;
using WebApplication_App_Concurso.Repositories;
using WebApplication_App_Concurso.Repositories.Contexts;

namespace WebApplication_App_Concurso.Tests.Unit.Repositories
{
    public class ConcursoRepositoryTests
    {
        private readonly DataContext _context;
        private readonly ConcursoRepository _sut;

        public ConcursoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);

            _sut = new ConcursoRepository(_context);
        }

        // ===========================
        // GetAllConcursosAsync
        // ===========================
        #region GetAllConcursosAsync[Fact]
        [Fact]
        public async Task GetAllConcursosAsync_DeveRetornarTodosConcursos()
        {
            // Arrange
            _context.Concursos.AddRange(
                new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Titulo = "Concurso SC",
                    Estado = "SC"
                },
                new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Titulo = "Concurso PR",
                    Estado = "PR"
                },
                new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Titulo = "Concurso SP",
                    Estado = "SP"
                });

            await _context.SaveChangesAsync();

            // Act
            var resultado = await _sut.GetAllConcursosAsync();

            // Assert
            resultado.Should().HaveCount(3);
            resultado.Select(c => c.Titulo)
                     .Should()
                     .Contain(new[] { "Concurso SC", "Concurso PR", "Concurso SP" });
        }

        [Fact]
        public async Task GetAllConcursosAsync_DeveRetornarListaVazia()
        {
            // Act
            var resultado = await _sut.GetAllConcursosAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }
        #endregion

        // ===========================
        // GetConcursosAsync
        // ===========================
        #region GetConcursosAsync[Fact]
        [Fact]
        public async Task GetConcursosAsync_DeveRetornarTodos_QuandoNaoHouverFiltros()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "A", Estado = "SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "B", Estado = "PR" },
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "C", Estado = "SP" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().HaveCount(3);
            resultado.totalCounts.Should().Be(3);
        }

        [Fact]
        public async Task GetConcursosAsync_DeveFiltrarPorTitulo()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Prefeitura SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Polícia Civil" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                Titulo = "Prefeitura",
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().ContainSingle();
            resultado.concursos.First().Titulo.Should().Contain("Prefeitura");
        }

        [Fact]
        public async Task GetConcursosAsync_DeveFiltrarPorOrgao()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Orgao = "TJSC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Orgao = "TRF4" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                Orgao = "TJSC",
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().ContainSingle();
            resultado.concursos.First().Orgao.Should().Be("TJSC");
        }

        [Fact]
        public async Task GetConcursosAsync_DeveFiltrarPorArea()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Area = "TI" },
                new Concurso { ConcursoId = Guid.NewGuid(), Area = "Saúde" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                Area = "TI",
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().ContainSingle();
        }

        [Fact]
        public async Task GetConcursosAsync_DeveFiltrarPorFonte()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Fonte = "PCI" },
                new Concurso { ConcursoId = Guid.NewGuid(), Fonte = "ConcursosBrasil" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                Fonte = "PCI",
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().ContainSingle();
        }

        [Fact]
        public async Task GetConcursosAsync_DeveFiltrarPorEstados()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "PR" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "SP" }
            );

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                Estados = new List<string> { "SC", "SP" },
                PageNumber = 1,
                PageSize = 10
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetConcursosAsync_DeveRetornarPaginaCorreta()
        {
            for (int i = 1; i <= 5; i++)
            {
                _context.Concursos.Add(new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Estado = $"E{i}",
                    Titulo = $"Concurso {i}"
                });
            }

            await _context.SaveChangesAsync();

            var filtro = new ConcursoFilter
            {
                PageNumber = 2,
                PageSize = 2
            };

            var resultado = await _sut.GetConcursosAsync(filtro);

            resultado.concursos.Should().HaveCount(2);
            resultado.totalCounts.Should().Be(5);
        }
        #endregion

        // ===========================
        // GetAllFontesConcursoAsync
        // ===========================
        #region GetAllFontesConcursoAsync[Fact]
        [Fact]
        public async Task GetAllFontesConcursoAsync_DeveRetornarFontesDistintas()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Fonte = "PCI" },
                new Concurso { ConcursoId = Guid.NewGuid(), Fonte = "PCI" },
                new Concurso { ConcursoId = Guid.NewGuid(), Fonte = "Brasil" }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetAllFontesConcursoAsync();

            resultado.Should().HaveCount(2);
            resultado.Should().Contain("PCI");
            resultado.Should().Contain("Brasil");
        }

        [Fact]
        public async Task GetAllFontesConcursoAsync_DeveRetornarListaVazia()
        {
            var resultado = await _sut.GetAllFontesConcursoAsync();

            resultado.Should().BeEmpty();
        }
        #endregion

        // ===========================
        // GetEstadosDisponiveisAsync
        // ===========================
        #region GetEstadosDisponiveisAsync[Fact]
        [Fact]
        public async Task GetEstadosDisponiveisAsync_DeveRetornarEstadosDistintos()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "PR" }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetEstadosDisponiveisAsync();

            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetEstadosDisponiveisAsync_DeveIgnorarEstadosNulos()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "SC" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = null! }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetEstadosDisponiveisAsync();

            resultado.Should().ContainSingle();
        }

        [Fact]
        public async Task GetEstadosDisponiveisAsync_DeveIgnorarEstadosVazios()
        {
            _context.Concursos.AddRange(
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "" },
                new Concurso { ConcursoId = Guid.NewGuid(), Estado = "PR" }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetEstadosDisponiveisAsync();

            resultado.Should().ContainSingle();
        }

        [Fact]
        public async Task GetEstadosDisponiveisAsync_DeveConverterParaMaiusculo()
        {
            _context.Concursos.Add(
                new Concurso
                {
                    ConcursoId = Guid.NewGuid(),
                    Estado = "sc"
                });

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetEstadosDisponiveisAsync();

            resultado.Should().Contain("SC");
        }
        #endregion

        // ===========================
        // GetTotalConcursosAsync
        // ===========================
        #region GetTotalConcursosAsync[Fact]
        [Fact]
        public async Task GetTotalConcursosAsync_DeveRetornarQuantidade()
        {
            _context.Concursos.AddRange(
                new Concurso(),
                new Concurso(),
                new Concurso()
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetTotalConcursosAsync();

            resultado.Should().Be(3);
        }

        [Fact]
        public async Task GetTotalConcursosAsync_DeveRetornarZero()
        {
            var resultado = await _sut.GetTotalConcursosAsync();

            resultado.Should().Be(0);
        }
        #endregion

        // ===========================
        // GetTotalVagasAsync
        // ===========================
        #region GetTotalVagasAsync[Fact]
        [Fact]
        public async Task GetTotalVagasAsync_DeveSomarVagas()
        {
            _context.Concursos.AddRange(
                new Concurso { Vagas = 10 },
                new Concurso { Vagas = 20 },
                new Concurso { Vagas = 5 }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetTotalVagasAsync();

            resultado.Should().Be(35);
        }

        [Fact]
        public async Task GetTotalVagasAsync_DeveIgnorarVagasNulas()
        {
            _context.Concursos.AddRange(
                new Concurso { Vagas = 10 },
                new Concurso { Vagas = null },
                new Concurso { Vagas = 20 }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetTotalVagasAsync();

            resultado.Should().Be(30);
        }

        [Fact]
        public async Task GetTotalVagasAsync_DeveRetornarZero()
        {
            var resultado = await _sut.GetTotalVagasAsync();

            resultado.Should().Be(0);
        }
        #endregion

        // ===========================
        // GetTotalFontesAsync
        // ===========================
        #region GetTotalFontesAsync[Fact]
        [Fact]
        public async Task GetTotalFontesAsync_DeveContarFontesDistintas()
        {
            _context.Concursos.AddRange(
                new Concurso { Fonte = "PCI" },
                new Concurso { Fonte = "PCI" },
                new Concurso { Fonte = "Brasil" }
            );

            await _context.SaveChangesAsync();

            var resultado = await _sut.GetTotalFontesAsync();

            resultado.Should().Be(2);
        }

        [Fact]
        public async Task GetTotalFontesAsync_DeveRetornarZero()
        {
            var resultado = await _sut.GetTotalFontesAsync();

            resultado.Should().Be(0);
        }
        #endregion

        // ===========================
        // SaveConcursosAsync
        // ===========================
        #region SaveConcursosAsync[Fact]
        [Fact]
        public async Task SaveConcursosAsync_DeveSalvarConcursos()
        {
            var concursos = new List<Concurso>
            {
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Teste" }
            };

            var resultado = await _sut.SaveConcursosAsync(concursos);

            resultado.Should().HaveCount(1);
            _context.Concursos.Should().HaveCount(1);
        }

        [Fact]
        public async Task SaveConcursosAsync_DeveRetornarListaSalva()
        {
            var concursos = new List<Concurso>
            {
                new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Concurso A" }
            };

            var resultado = await _sut.SaveConcursosAsync(concursos);

            resultado.Should().BeEquivalentTo(concursos);
        }
        #endregion
    }
}
