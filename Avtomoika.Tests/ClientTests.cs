using Moq;
using Avtomoika.Api.Controllers;
using Xunit;
using Avtomoika.Application.Interfaces;
using Avtomoika.Domain.Entities;
using Microsoft.AspNetCore.Mvc;


public class ClientTests
{
    [Fact]
    public async Task Client_get_all()
    {
        var repoMock = new Mock<IRepository<Client>>();

        var cl1 = new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" };
        var cl2 = new Client { Id = 2, Name = "Test", Number = "88888888888", Email = "Test" };
        
        repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Client> {cl1, cl2 });


        var controller = new ClientController(repoMock.Object);
        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var client = Assert.IsAssignableFrom<List<Client>>(ok.Value);
        Assert.NotNull(client);
        Assert.Equal(2, client.Count());
        Assert.Equal(cl1, client.ElementAt(0));
        Assert.Equal(cl2, client.ElementAt(1));
    }

    [Fact]
    public async Task Client_get_by_id()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });

        var controller = new ClientController(repoMock.Object);
        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var client = Assert.IsType<Client>(ok.Value);


        //Id = 1,
        //Name = "Test",
        //Number = "88888888888",
        //Email = "Test"
        Assert.Equal(1, client.Id);
        Assert.Equal("Test", client.Name);
        Assert.Equal("88888888888", client.Number);
        Assert.Equal("Test", client.Email);
    }

    [Fact]
    public async Task Client_get_wrong_id()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });

        var controller = new ClientController(repoMock.Object);
        var result = await controller.GetById(5);

        var ok = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Client_post()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Client>()))
            .Returns(Task.CompletedTask);

        var cont = new ClientController(repoMock.Object);
        var result = await cont.Create(new Client() { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });

        var ok = Assert.IsType<CreatedAtActionResult>(result);
        var client = Assert.IsType<Client>(ok.Value);

        Assert.Equal(1, client.Id);
        Assert.Equal("Test", client.Name);
        Assert.Equal("88888888888", client.Number);
        Assert.Equal("Test", client.Email);
    }

    [Fact]
    public async Task Client_put()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });
        
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);


        var newclient = new Client() { Id = 1, Name = "Test5", Number = "55555", Email = "Test5" };
        var cont = new ClientController(repoMock.Object);
        var result = await cont.Update(1, newclient);

        Assert.IsType<NoContentResult>(result);
        
        
        repoMock.Verify(r => r.UpdateAsync(newclient),  Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        
    }
    
    [Fact]
    public async Task Client_put_wrong_id()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new Client { Id = 5, Name = "Test", Number = "88888888888", Email = "Test" });
        
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Client>()))
            .Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var cont = new ClientController(repoMock.Object);
        var result = await cont.Update(5, new Client(){Id = 1, Name = "Test5", Number = "55555", Email = "Test5" });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Client_delete()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });
        repoMock.Setup(r => r.DeleteAsync(1));
        repoMock.Setup(r => r.SaveChangesAsync());
        
        var cont = new ClientController(repoMock.Object);
        var result = await cont.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task Client_delete_wrong_id()
    {
        var repoMock = new Mock<IRepository<Client>>();
        repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Client { Id = 1, Name = "Test", Number = "88888888888", Email = "Test" });
        repoMock.Setup(r => r.DeleteAsync(5));
        repoMock.Setup(r => r.SaveChangesAsync());
        
        var cont = new ClientController(repoMock.Object);
        var result = await cont.Delete(5);
        Assert.IsType<NotFoundResult>(result);
    }
}