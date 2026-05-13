using ApiProject.Controllers;
using ApiProject.DTO;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ApiProject.Controllers;
using ApiProject.Services.Interface;
using ApiProject.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ApiProject.Tests
{
    public class CartControllerTest
    {
        private readonly Mock<ICartService> _mockService;
        private readonly CartController _controller;

        public CartControllerTest()
        {
            _mockService = new Mock<ICartService>();
            _controller = new CartController(_mockService.Object);
        }

        [Fact]
        public async Task GetCartById_ShouldReturnOk_WhenCartExists()
        {
            // Arrange
            int userId = 1;

            var cartDto = new CartDto.CartModelDto
            {
                Id = 10,
                UserId = userId,
                Items = new List<CartDto.CartItemDto>()
            };

            _mockService.Setup(s => s.GetCartByUserId(userId))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.GetCartById(userId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AddGiftToCart_ShouldReturnOk_WhenGiftAdded()
        {
            // Arrange
            var dto = new CartDto.AddCartItemDto
            {
                UserId = 1,
                GiftId = 5
            };

            var cartDto = new CartDto.CartModelDto
            {
                Id = 1,
                UserId = 1,
                Items = new List<CartDto.CartItemDto>()
            };

            _mockService.Setup(s => s.AddGiftToCart(dto))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.AddGiftToCart(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AddGiftToCart_ShouldReturnBadRequest_WhenGiftCannotBeAdded()
        {
            // Arrange
            var dto = new CartDto.AddCartItemDto
            {
                UserId = 1,
                GiftId = 5
            };

            _mockService.Setup(s => s.AddGiftToCart(dto))
                        .ReturnsAsync((CartDto.CartModelDto)null);

            // Act
            var result = await _controller.AddGiftToCart(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PlusOne_ShouldReturnOk_WhenQuantityIncreased()
        {
            // Arrange
            var dto = new CartDto.AddCartItemDto
            {
                UserId = 1,
                GiftId = 5
            };

            var cartDto = new CartDto.CartModelDto
            {
                Id = 1,
                UserId = 1
            };

            _mockService.Setup(s => s.PlusOneGiftToCart(dto))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.PlusOne(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task MinusOne_ShouldReturnOk_WhenQuantityDecreased()
        {
            // Arrange
            var dto = new CartDto.AddCartItemDto
            {
                UserId = 1,
                GiftId = 5
            };

            var cartDto = new CartDto.CartModelDto
            {
                Id = 1,
                UserId = 1
            };

            _mockService.Setup(s => s.MinusOneGiftToCart(dto))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.MinusOne(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteGiftFromCart_ShouldReturnOk_WhenGiftDeleted()
        {
            // Arrange
            int userId = 1;
            int giftId = 5;

            var cartDto = new CartDto.CartModelDto
            {
                Id = 1,
                UserId = userId
            };

            _mockService.Setup(s => s.DeleteGiftFromCart(userId, giftId))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.DeleteGiftFromCart(userId, giftId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateGiftToCart_ShouldReturnOk_WhenGiftUpdated()
        {
            // Arrange
            var dto = new CartDto.UpdateCartItemDto
            {
                UserId = 1,
                GiftId = 5,
                Quantity = 3
            };

            var cartDto = new CartDto.CartModelDto
            {
                Id = 1,
                UserId = 1
            };

            _mockService.Setup(s => s.UpdateGiftToCart(dto))
                        .ReturnsAsync(cartDto);

            // Act
            var result = await _controller.UpdateGiftToCart(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PurchaseCart_ShouldReturnOk_WhenPurchaseSuccessful()
        {
            // Arrange
            int userId = 1;

            var carts = new List<CartDto.CartModelDto>
        {
            new CartDto.CartModelDto { Id = 1, UserId = userId }
        };

            _mockService.Setup(s => s.PurchaseCart(userId))
                        .ReturnsAsync(carts);

            // Act
            var result = await _controller.PurchaseCart(userId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

    }
}
