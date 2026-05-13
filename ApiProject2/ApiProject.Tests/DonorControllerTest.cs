using ApiProject.Controllers;
using ApiProject.DTO;
using ApiProject.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ApiProject.Controllers;
using ApiProject.Services.Interface;
using ApiProject.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Threading.Tasks;

namespace ApiProject.Tests
{
    public class DonorControllerTest
    {

        private readonly Mock<IDonorService> _mockService;
        private readonly DonorsController _controller;

        public DonorControllerTest()
        {
            _mockService = new Mock<IDonorService>();
            _controller = new DonorsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithDonors()
        {
            var donors = new List<DonorDto.DonorModelDto>
        {
            new DonorDto.DonorModelDto { Id = 1, FirstName = "Ella", LastName = "Cohen" }
        };

            _mockService.Setup(s => s.GetAllDonors()).ReturnsAsync(donors);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(donors);
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOk()
        {
            var donor = new DonorDto.DonorModelDto { Id = 1, FirstName = "Ella" };

            _mockService.Setup(s => s.GetDonorById(1)).ReturnsAsync(donor);

            var result = await _controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_WhenNotExists_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetDonorById(1)).ReturnsAsync((DonorDto.DonorModelDto)null);

            var result = await _controller.GetById(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_WithValidDto_ReturnsOk()
        {
            var dto = new DonorDto.AddDonorDto { FirstName = "Ella", LastName = "Cohen" };
            var resultDto = new DonorDto.DonorModelDto { Id = 1, FirstName = "Ella" };

            _mockService.Setup(s => s.AddDonor(dto)).ReturnsAsync(resultDto);

            var result = await _controller.Create(dto);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_WithNull_ReturnsBadRequest()
        {
            var result = await _controller.Create(null);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_WhenExists_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteDonor(1)).ReturnsAsync("Deleted");

            var result = await _controller.Delete(1);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_WhenNotExists_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteDonor(1)).ReturnsAsync((string)null);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}
