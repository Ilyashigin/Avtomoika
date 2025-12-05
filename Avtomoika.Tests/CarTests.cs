using Moq;
using Avtomoika.Api.Controllers;
using Xunit;
using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;


public class CarTests
{
    [Fact]
    public async Task Car_Get_all()
    {
        var repoMock = new Mock<IRepository<Car>>();

        var car1 = new Car() {Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = 1};
        var car2 = new Car() {Id = 2, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = 2};
        
        repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Car>{car1, car2});

        var conrol = new CarController(repoMock.Object);
        var result = await conrol.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var cars = Assert.IsAssignableFrom<IEnumerable<Car>>(ok.Value);
        
        Assert.Equal(2, cars.Count());
        Assert.Equal(car1, cars.ElementAt(0));
        Assert.Equal(car2, cars.ElementAt(1));
        
    }

    [Fact]
    public async Task Car_get_by_id()
    {
        var repoMock = new Mock<IRepository<Car>>();

        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});

        var controler = new CarController(repoMock.Object);
        var result = await controler.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var car = Assert.IsType<Car>(ok.Value);
        
        //Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = 1
        Assert.Equal(1, car.Id);
        Assert.Equal("aaa", car.Marka);
        Assert.Equal("aasd", car.Model);
        Assert.Equal("aaa", car.Number);
        
        Assert.Equal(client.Id, car.Client.Id);
        Assert.Equal(client, car.Client);
    }

    [Fact]
    public async Task car_get_by_wrong_id()
    {
        var repoMock = new Mock<IRepository<Car>>();
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});

        var controler = new CarController(repoMock.Object);
        var result = await controler.GetById(3);

        var ok = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task car_post()
    {
        var repoMock = new Mock<IRepository<Car>>();
        
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.AddAsync(It.IsAny<Car>()))
            .Returns(Task.CompletedTask);
        
        var controler = new CarController(repoMock.Object);
        var result = await controler.Create(new Car()
            { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client });

        var ok = Assert.IsType<CreatedAtActionResult>(result);
        var car = Assert.IsType<Car>(ok.Value);
        
        Assert.Equal(1, car.Id);
        Assert.Equal("aaa", car.Marka);
        Assert.Equal("aasd", car.Model);
        Assert.Equal("aaa", car.Number);
        
        Assert.Equal(client.Id, car.Client.Id);
        Assert.Equal(client, car.Client);
        
    }

    [Fact]
    public async Task car_put()
    {
        var repoMock = new Mock<IRepository<Car>>();
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Car>()))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var newcar = new Car()
            { Id = 1, Marka = "bbb", Model = "bbb", Number = "bbb", ClientId = client.Id, Client = client };
        
        var controler = new CarController(repoMock.Object);
        var result = await controler.Update(1, newcar);

        Assert.IsType<NoContentResult>(result);
        
        repoMock.Verify(r => r.UpdateAsync(newcar), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task car_put_wrong_id()
    {
        var repoMock = new Mock<IRepository<Car>>();
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Car>()))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var newcar = new Car()
            { Id = 1, Marka = "bbb", Model = "bbb", Number = "bbb", ClientId = client.Id, Client = client };
        
        var controler = new CarController(repoMock.Object);
        var result = await controler.Update(5, newcar);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task car_delete()
    {
        var repoMock = new Mock<IRepository<Car>>();
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});
        repoMock.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new CarController(repoMock.Object);
        var result = await controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
        repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
    
    
    [Fact]
    public async Task car_delete_wrong_id()
    {
        var repoMock = new Mock<IRepository<Car>>();
        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client});
        repoMock.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new CarController(repoMock.Object);
        var result = await controller.Delete(5);
        Assert.IsType<NotFoundResult>(result);
    }
}