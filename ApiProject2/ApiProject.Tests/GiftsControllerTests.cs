using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ApiProject.Controllers;
using ApiProject.Services.Interface;
using ApiProject.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GiftsControllerTests
{
    private readonly Mock<IGiftsService> _mockService;
    private readonly GiftsController _controller;

    public GiftsControllerTests()
    {
        _mockService = new Mock<IGiftsService>();
        _controller = new GiftsController(_mockService.Object);
    }

    [Fact]
    public async Task GetGifts_ShouldReturnOk_WithListOfGifts()
    {
        // Arrange
        var gifts = new List<GiftDto.GiftModelDto>
        {
            new GiftDto.GiftModelDto { Id = 1, Name = "Gift1" },
            new GiftDto.GiftModelDto { Id = 2, Name = "Gift2" }
        };

        _mockService.Setup(s => s.getAllGifts())
                    .ReturnsAsync(gifts);

        // Act
        var result = await _controller.GetGifts();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }
    [Fact]
    public async Task GetGiftById_ShouldReturnNotFound_WhenGiftDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetGiftById(1))
                    .ReturnsAsync((GiftDto.GiftModelDto)null);

        // Act
        var result = await _controller.GetGiftById(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
    [Fact]
    public async Task OrderByCategory_ShouldReturnNotFound_WhenCategoryEmpty()
    {
        // Arrange
        _mockService.Setup(s => s.SortByCategory(1))
                    .ReturnsAsync(new List<GiftDto.GiftModelDto>());

        // Act
        var result = await _controller.OrderByCategory(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
    [Fact]
    public async Task AddGift_ShouldReturnBadRequest_WhenGiftIsNull()
    {
        // Act
        var result = await _controller.AddGift(null);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
    [Fact]
    public async Task DeleteGift_ShouldReturnNotFound_WhenGiftDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteGift(1))
                    .ReturnsAsync((IEnumerable<GiftDto.GiftModelDto>)null);

        // Act
        var result = await _controller.DeleteGift(1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

}
