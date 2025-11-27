using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Avtomoika.Api.Controllers;
using Avtomoika.Application.Interfaces; 
using Avtomoika.Domain.Entities; 


public class ServiceControllerTests
{
    [Fact]
    public async Task Get_all_service()
    {
        var repoMock = new Mock<IRepository<Service>>();
        
        var serv1 = new Service
        {
            Id = 1,
            Name = "чистка",
            Description = "ляляля",
            Price = 15
        };
        var serv2 = new Service
        {
            Id = 2,
            Name = "чистка",
            Description = "ляляля",
            Price = 15
        };
        
        repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Service>{serv1, serv2});
        
        
        var controller = new ServiceController(repoMock.Object);
        var result = await controller.GetAll();
        
        var ok =  Assert.IsType<OkObjectResult>(result);
        var service = Assert.IsAssignableFrom<List<Service>>(ok.Value);
        Assert.NotNull(service);
        Assert.Equal(2, service.Count());
        Assert.Equal(serv1, service.ElementAt(0));
        Assert.Equal(serv2, service.ElementAt(1));
    }
    
    

    [Fact]
    public async Task Get_service_by_id()
    {
        var repoMock = new Mock<IRepository<Service>>();
        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service {Id = 20, Name = "чистка", Description = "ляляля", Price = 15});
        
        var controller = new ServiceController(repoMock.Object);
        var result = await controller.GetById(20);

        var ok = Assert.IsType<OkObjectResult>(result);
        var service = Assert.IsType<Service>(ok.Value);
        
        //{Id = 20, Name = "чистка", Description = "ляляля", Price = 15}
        Assert.Equal(20, service.Id);
        Assert.Equal("чистка", service.Name);
        Assert.Equal("ляляля", service.Description);
        Assert.Equal(15, service.Price);
    }
    
    [Fact]
    public async Task Get_service_by_id_wrong_id()
    {
        var repoMock = new Mock<IRepository<Service>>();
        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service {Id = 20, Name = "чистка", Description = "ляляля", Price = 15});
        
        var controller = new ServiceController(repoMock.Object);
        var result = await controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
        
        
    }

    [Fact]
    public async Task Post_service()
    {
        var repoMock = new Mock<IRepository<Service>>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Service>()))
            .Returns(Task.CompletedTask);

        var controller = new ServiceController(repoMock.Object);
        var newService = new Service()
        {
            Id = 20,
            Name = "чистка",
            Description = "ляляля",
            Price = 15

        };

        var result = await controller.Create(newService);
        var ok = Assert.IsType<CreatedAtActionResult>(result);
        var service = Assert.IsType<Service>(ok.Value);
        
        
        Assert.Equal(20, service.Id);
        Assert.Equal("чистка", service.Name);
        Assert.Equal("ляляля", service.Description);
        Assert.Equal(15, service.Price);
    }
    

    [Fact]
    public async Task Put_Service_Returns()
    {
        var repoMock = new Mock<IRepository<Service>>();

        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service { Id = 20, Name = "чистка", Description = "ляляля", Price = 15 });

        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Service>()))
            .Returns(Task.CompletedTask);

        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var updated = new Service { Id = 20, Name = "NEW", Description = "UPDATED", Price = 50 };
        var controller = new ServiceController(repoMock.Object);
        var result = await controller.Update(20, updated);
        
        Assert.IsType<NoContentResult>(result);

        repoMock.Verify(r => r.UpdateAsync(
            It.Is<Service>(s =>
                s.Id == updated.Id &&
                s.Name == updated.Name &&
                s.Description == updated.Description &&
                s.Price == updated.Price
            )
        ), Times.Once);

        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Put_Service_Wrong_Id_Returns()
    {
        var repoMock = new Mock<IRepository<Service>>();

        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service
            {
                Id = 20,
                Name = "чистка",
                Description = "ляляля",
                Price = 15
            });

        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Service>()))
            .Returns(Task.CompletedTask);

        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new ServiceController(repoMock.Object);

        var updated = new Service
        {
            Id = 15,
            Name = "NEW",
            Description = "UPDATED",
            Price = 50
        };
        
        var result = await controller.Update(20, updated);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_Service()
    {
        var repoMock = new Mock<IRepository<Service>>();
        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service
            {
                Id = 20,
                Name = "чистка",
                Description = "ляляля",
                Price = 15
            });
        
        repoMock.Setup(r => r.DeleteAsync(20))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        

        var controller = new ServiceController(repoMock.Object);
        var result = await controller.Delete(20);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_Service_Wrong_Id()
    {
        var repoMock = new Mock<IRepository<Service>>();
        repoMock.Setup(r => r.GetByIdAsync(20))
            .ReturnsAsync(new Service
            {
                Id = 20,
                Name = "чистка",
                Description = "ляляля",
                Price = 15
            });
        
        repoMock.Setup(r => r.DeleteAsync(20))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new ServiceController(repoMock.Object);
        var result = await controller.Delete(15);
        Assert.IsType<NotFoundResult>(result);
    }
    
}