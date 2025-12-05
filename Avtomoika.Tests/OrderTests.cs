using Moq;
using Avtomoika.Api.Controllers;
using Avtomoika.Api.DTOs;
using Xunit;
using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

public class OrderTests()
{
    [Fact]
    public async Task Order_Get_all()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();


        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var car = new Car() { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client};
        var serv1 = new Service { Id = 1, Name = "чистка", Description = "ляляля", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "чистка", Description = "ляляля", Price = 10 };
        
        var order1 = new Order(){Id = 1, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car, Status = "test", TotalPrice = serv1.Price + serv2.Price};
        var prder2 = new Order() {Id = 2, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car, Status = "test2"};
        
        orderRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Order>{order1, prder2});

        var controller = new OrderController(orderRepoMock.Object,  serviceRepoMock.Object);
        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(ok.Value);
        Assert.Equal(2, orders.Count());
        Assert.Equal(order1, orders.ElementAt(0));
        Assert.Equal(prder2, orders.ElementAt(1));
        Assert.Equal(client, orders.ElementAt(0).Client);
        Assert.Equal(client, orders.ElementAt(1).Client);
        Assert.Equal(car, orders.ElementAt(0).Car);
        Assert.Equal(car, orders.ElementAt(1).Car);
        Assert.Equal(25, orders.ElementAt(0).TotalPrice);
        Assert.Equal(serv1, orders.ElementAt(1).Services.ElementAt(0));
        Assert.Equal(serv2, orders.ElementAt(1).Services.ElementAt(1));
    }

    [Fact]
    public async Task Order_Get_by_id()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();


        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var car = new Car()
            { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client };
        var serv1 = new Service { Id = 1, Name = "чистка", Description = "ляляля", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "чистка", Description = "ляляля", Price = 10 };

        var order1 = new Order()
        {
            Id = 1, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car,
            Status = "test", TotalPrice = serv1.Price + serv2.Price
        };
        

        orderRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order1);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);
        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var order = Assert.IsType<Order>(ok.Value);
        
        Assert.Equal(1,  order.Id);
        Assert.Equal([serv1, serv2], order.Services);
        Assert.Equal(client, order.Client);
        Assert.Equal(car, order.Car);
        Assert.Equal("test", order.Status);
        Assert.Equal(25, order.TotalPrice);
    }
    
    [Fact]
    public async Task Order_Get_by_wrong_id()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();


        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var car = new Car()
            { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client };
        var serv1 = new Service { Id = 1, Name = "чистка", Description = "ляляля", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "чистка", Description = "ляляля", Price = 10 };

        var order1 = new Order()
        {
            Id = 1, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car,
            Status = "test", TotalPrice = serv1.Price + serv2.Price
        };
        

        orderRepoMock.Setup(r => r.GetByIdAsync(9))
            .ReturnsAsync(order1);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);
        var result = await controller.GetById(9);
        Assert.IsType<NotFoundResult>(result);

        
    }
    
    
    [Fact]
    public async Task Order_Post()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();
        
        orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        orderRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        
        var s1 = new Service { Id = 1, Name = "Услуга1", Price = 10 };
        var s2 = new Service { Id = 2, Name = "Услуга2", Price = 20 };
        
        serviceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(s1);
        serviceRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(s2);
        
        var dto = new CreateOrderDto
        {
            ClientId = 5,
            CarId = 3,
            ServiceIds = new List<int> { 1, 2 },
            OrderDate = DateTime.Now,
            Status = "Created"
        };

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);
        var result = await controller.Create(dto);
        
        var ok = Assert.IsType<CreatedAtActionResult>(result);
        var order = Assert.IsType<Order>(ok.Value);

        Assert.Equal(dto.ClientId, order.ClientId);
        Assert.Equal(dto.CarId, order.CarId);
        Assert.Equal(dto.Status, order.Status);
        Assert.Equal(2, order.Services.Count);
        Assert.Contains(order.Services, x => x.Id == 1);
        Assert.Contains(order.Services, x => x.Id == 2);
        Assert.Equal(30, order.TotalPrice);
        
        orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Order_Put()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();

        var client = new Client { Id = 1, Name = "Test" };
        var car = new Car { Id = 1, Marka = "aaa", Model = "bbb", ClientId = 1 };

        var serv1 = new Service { Id = 1, Name = "чистка", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "полировка", Price = 10 };

        var oldOrder = new Order
        {
            Id = 1,
            ClientId = 1,
            CarId = 1,
            Services = new List<Service> { serv1, serv2 },
            Status = "old"
        };

        
        var dto = new CreateOrderDto()
        {
            ClientId = 1,
            CarId = 1,
            ServiceIds = new List<int> { 1 },
            Status = "updated"
        };

        
        orderRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oldOrder);
        serviceRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(serv1);
        orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        orderRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);
        var result = await controller.Update(1, dto);
        
        Assert.IsType<NoContentResult>(result);

        orderRepoMock.Verify(r => r.UpdateAsync(It.Is<Order>(o =>
            o.Id == 1 &&
            o.Status == "updated" &&
            o.Services.Count == 1 &&
            o.Services[0].Id == 1
        )), Times.Once);

        orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task Order_Put_wrong_id()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();

        var client = new Client { Id = 1, Name = "Test" };
        var car = new Car { Id = 1, Marka = "aaa", Model = "bbb", ClientId = 1 };

        var serv1 = new Service { Id = 1, Name = "чистка", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "полировка", Price = 10 };

        var oldOrder = new Order
        {
            Id = 1,
            ClientId = 1,
            CarId = 1,
            Services = new List<Service> { serv1, serv2 },
            Status = "old"
        };

        
        var dto = new CreateOrderDto()
        {
            ClientId = 1,
            CarId = 1,
            ServiceIds = new List<int> { 1 },
            Status = "updated"
        };

        
        orderRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(oldOrder);
        serviceRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(serv1);
        orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        orderRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);
        var result = await controller.Update(5, dto);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Order_Delete()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();

        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var car = new Car()
            { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client };
        var serv1 = new Service { Id = 1, Name = "чистка", Description = "ляляля", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "чистка", Description = "ляляля", Price = 10 };

        var order = new Order()
        {
            Id = 1, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car,
            Status = "test", TotalPrice = serv1.Price + serv2.Price
        };

        orderRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order);

        orderRepoMock.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        orderRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);

        orderRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        orderRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Order_Delete_wrong_id()
    {
        var orderRepoMock = new Mock<IRepository<Order>>();
        var serviceRepoMock = new Mock<IRepository<Service>>();

        var client = new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var car = new Car()
            { Id = 1, Marka = "aaa", Model = "aasd", Number = "aaa", ClientId = client.Id, Client = client };
        var serv1 = new Service { Id = 1, Name = "чистка", Description = "ляляля", Price = 15 };
        var serv2 = new Service { Id = 2, Name = "чистка", Description = "ляляля", Price = 10 };

        var order = new Order()
        {
            Id = 1, Services = [serv1, serv2], ClientId = client.Id, Client = client, CarId = car.Id, Car = car,
            Status = "test", TotalPrice = serv1.Price + serv2.Price
        };

        orderRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order);

        orderRepoMock.Setup(r => r.DeleteAsync(9))
            .Returns(Task.CompletedTask);

        orderRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var controller = new OrderController(orderRepoMock.Object, serviceRepoMock.Object);

        var result = await controller.Delete(9);

        Assert.IsType<NotFoundResult>(result);
        
    }




}