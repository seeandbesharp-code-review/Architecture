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
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ApiProject.Controllers;
using ApiProject.Services.Interface;
using ApiProject.DTO;
using System.Threading.Tasks;


namespace ApiProject.Tests
{
    public  class AuthControllerTest
    {
        private readonly Mock<IAuthService> _mockService;
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _mockService = new Mock<IAuthService>();
            _controller = new AuthController(_mockService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserCreated()
        {
            // Arrange
            var dto = new AuthorDto.RegisterDto
            {
                Email = "test@test.com",
                Password = "1234",
                FirstName = "Ella",
                LastName = "Cohen",
                Phone = "0501234567"
            };

            var userDto = new AuthorDto.UserModelDto
            {
                Id = 1,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Role = "customer",
                Token = "fake-token"
            };

            _mockService.Setup(s => s.Register(dto))
                        .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var dto = new AuthorDto.RegisterDto
            {
                Email = "existing@test.com",
                Password = "1234"
            };

            _mockService.Setup(s => s.Register(dto))
                        .ReturnsAsync((AuthorDto.UserModelDto)null);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginSuccessful()
        {
            // Arrange
            var dto = new AuthorDto.LoginDto
            {
                Email = "test@test.com",
                Password = "1234",
                Phone = "0501234567"
            };

            var userDto = new AuthorDto.UserModelDto
            {
                Id = 1,
                Email = dto.Email,
                FirstName = "Ella",
                LastName = "Cohen",
                Role = "customer",
                Token = "fake-token"
            };

            _mockService.Setup(s => s.Login(dto))
                        .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
        {
            // Arrange
            var dto = new AuthorDto.LoginDto
            {
                Email = "wrong@test.com",
                Password = "wrong",
                Phone = "000"
            };

            _mockService.Setup(s => s.Login(dto))
                        .ReturnsAsync((AuthorDto.UserModelDto)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

    }
}
