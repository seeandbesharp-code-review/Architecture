using ApiProject.DTO;
using ApiProject.Models;
using ApiProject.Repositories.Interface;
using ApiProject.Services.Implement;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using ApiProject.Services.Implement;
using ApiProject.Repositories.Interface;
using ApiProject.Models;
using ApiProject.DTO;


namespace ApiProject.Tests
{
    public class CategoryControllerTest
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly CategoryService _service;

        public CategoryControllerTest()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _service = new CategoryService(_repositoryMock.Object, _loggerMock.Object);
        }

        // ==========================
        // GetAllCategories
        // ==========================
        [Fact]
        public async Task GetAllCategories_ReturnsMappedCategories()
        {
            // Arrange
            var categories = new List<CategoryModel>
        {
            new CategoryModel { Id = 1, Name = "Food" },
            new CategoryModel { Id = 2, Name = "Tech" }
        };

            _repositoryMock.Setup(r => r.GetAllCategories())
                           .ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllCategories();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Food");
        }

        // ==========================
        // GetCategoryById
        // ==========================
        [Fact]
        public async Task GetCategoryById_ReturnsCategory_WhenExists()
        {
            var category = new CategoryModel { Id = 1, Name = "Food" };

            _repositoryMock.Setup(r => r.GetCategoryById(1))
                           .ReturnsAsync(category);

            var result = await _service.GetCategoryById(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Food");
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNull_WhenNotExists()
        {
            _repositoryMock.Setup(r => r.GetCategoryById(99))
                           .ReturnsAsync((CategoryModel)null);

            var result = await _service.GetCategoryById(99);

            result.Should().BeNull();
        }

        // ==========================
        // AddCategory
        // ==========================
        [Fact]
        public async Task AddCategory_ReturnsMappedList()
        {
            var dto = new CategoryDto { Name = "NewCategory" };

            var categories = new List<CategoryModel>
        {
            new CategoryModel { Id = 1, Name = "NewCategory" }
        };

            _repositoryMock.Setup(r => r.AddCategory(It.IsAny<CategoryModel>()))
                           .ReturnsAsync(categories);

            var result = await _service.AddCategory(dto);

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("NewCategory");
        }

        // ==========================
        // UpdateCategory
        // ==========================
        [Fact]
        public async Task UpdateCategory_ReturnsUpdatedCategory_WhenExists()
        {
            var dto = new CategoryDto { Name = "Updated" };

            var updatedModel = new CategoryModel { Id = 1, Name = "Updated" };

            _repositoryMock.Setup(r => r.UpdateCategoryR(It.IsAny<CategoryModel>(), 1))
                           .ReturnsAsync(updatedModel);

            var result = await _service.UpdateCategory(dto, 1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNull_WhenNotExists()
        {
            var dto = new CategoryDto { Name = "Updated" };

            _repositoryMock.Setup(r => r.UpdateCategoryR(It.IsAny<CategoryModel>(), 1))
                           .ReturnsAsync((CategoryModel)null);

            var result = await _service.UpdateCategory(dto, 1);

            result.Should().BeNull();
        }

        // ==========================
        // DeleteCategory
        // ==========================
        [Fact]
        public async Task DeleteCategory_ReturnsList_WhenExists()
        {
            var categories = new List<CategoryModel>
        {
            new CategoryModel { Id = 1, Name = "Food" }
        };

            _repositoryMock.Setup(r => r.DeleteCategory(1))
                           .ReturnsAsync(categories);

            var result = await _service.DeleteCategory(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNull_WhenNotExists()
        {
            _repositoryMock.Setup(r => r.DeleteCategory(99))
                           .ReturnsAsync((IEnumerable<CategoryModel>)null);

            var result = await _service.DeleteCategory(99);

            result.Should().BeNull();
        }

    }
}
