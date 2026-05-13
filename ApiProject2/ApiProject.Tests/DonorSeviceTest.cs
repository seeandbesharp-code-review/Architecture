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
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ApiProject.Tests
{
    public class DonorSeviceTest
    {
        private readonly Mock<IDonorRepository> _mockRepo;
        private readonly Mock<ILogger<DonorService>> _mockLogger;
        private readonly DonorService _service;

        public DonorSeviceTest()
        {
            _mockRepo = new Mock<IDonorRepository>();
            _mockLogger = new Mock<ILogger<DonorService>>();
            _service = new DonorService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetDonorById_WhenExists_ReturnsDonor()
        {
            var donor = new DonorModel
            {
                Id = 1,
                FirstName = "Ella",
                LastName = "Cohen"
            };

            _mockRepo.Setup(r => r.GetDonorById(1)).ReturnsAsync(donor);

            var result = await _service.GetDonorById(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetDonorById_WhenNotExists_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetDonorById(1)).ReturnsAsync((DonorModel)null);

            var result = await _service.GetDonorById(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteDonor_WhenExists_ReturnsMessage()
        {
            var donor = new DonorModel { Id = 1 };

            _mockRepo.Setup(r => r.GetDonorById(1)).ReturnsAsync(donor);
            _mockRepo.Setup(r => r.DeleteDonor(donor)).ReturnsAsync("Deleted");

            var result = await _service.DeleteDonor(1);

            result.Should().Be("Deleted");
        }

        [Fact]
        public async Task DeleteDonor_WhenNotExists_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetDonorById(1)).ReturnsAsync((DonorModel)null);

            var result = await _service.DeleteDonor(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllDonors_ReturnsList()
        {
            var donors = new List<DonorModel>
        {
            new DonorModel { Id = 1, FirstName = "Ella" }
        };

            _mockRepo.Setup(r => r.GetAllDonors()).ReturnsAsync(donors);

            var result = await _service.GetAllDonors();

            result.Should().HaveCount(1);
        }

    }
}
