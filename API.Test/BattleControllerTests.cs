using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Test;

public class BattleControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public BattleControllerTests()
    {
        this._repository = new Mock<IBattleOfMonstersRepository>();
    }

    [Fact]
    public async void Get_OnSuccess_ReturnsListOfBattles()
    {
        this._repository
            .Setup(x => x.Battles.GetAllAsync())
            .ReturnsAsync(BattlesFixture.GetBattlesMock());

        BattleController sut = new BattleController(this._repository.Object);
        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Battle[]>();
    }

    [Fact]
    public async Task Post_BadRequest_When_StartBattle_With_nullMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle b = new Battle()
        {
            MonsterA = null,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(this._repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult)result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Missing ID", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnNoMonsterFound_When_StartBattle_With_NonexistentMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle b = new Battle()
        {
            MonsterA = 54812668,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(this._repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult)result;
        result.Should().BeOfType<BadRequestObjectResult>();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning()
    {
        var sut = new BattleController(_repository.Object);

        var battlesMock = BattlesFixture.GetBattlesMock().ToArray();
        var monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var monsterA = monstersMock[1];
        var monsterB = monstersMock[2];

        var battle = battlesMock[0];
        battle.MonsterA = monsterA.Id;
        battle.MonsterB = monsterB.Id;

        _repository.Setup(x => x.Battles.AddAsync(battle));

        _repository.Setup(x => x.Monsters.FindAsync(monsterA.Id)).ReturnsAsync(monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(monsterB.Id)).ReturnsAsync(monsterB);

        var result = await sut.Add(battle);

        OkObjectResult objectResults = (OkObjectResult)result;

        Assert.Equal("Monster A winner.", objectResults.Value);
    }


    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning()
    {
        var sut = new BattleController(_repository.Object);

        var battlesMock = BattlesFixture.GetBattlesMock().ToArray();
        var monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var monsterA = monstersMock[0];
        var monsterB = monstersMock[1];

        var battle = battlesMock[0];
        battle.MonsterA = monsterA.Id;
        battle.MonsterB = monsterB.Id;

        _repository.Setup(x => x.Battles.AddAsync(battle));

        _repository.Setup(x => x.Monsters.FindAsync(monsterA.Id)).ReturnsAsync(monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(monsterB.Id)).ReturnsAsync(monsterB);

        var result = await sut.Add(battle);

        OkObjectResult objectResults = (OkObjectResult)result;

        Assert.Equal("Monster B winner.", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirSpeedsSame_And_MonsterA_Has_Higher_Attack()
    {
        var sut = new BattleController(_repository.Object);

        var battlesMock = BattlesFixture.GetBattlesMock().ToArray();
        var monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var monsterA = monstersMock[3];
        var monsterB = monstersMock[4];

        var battle = battlesMock[0];
        battle.MonsterA = monsterA.Id;
        battle.MonsterB = monsterB.Id;

        _repository.Setup(x => x.Battles.AddAsync(battle));

        _repository.Setup(x => x.Monsters.FindAsync(monsterA.Id)).ReturnsAsync(monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(monsterB.Id)).ReturnsAsync(monsterB);


        var result = await sut.Add(battle);

        OkObjectResult objectResults = (OkObjectResult)result;

        Assert.Equal("Monster A winner.", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning_When_TheirSpeedsSame_And_MonsterB_Has_Higher_Attack()
    {
        var sut = new BattleController(_repository.Object);

        var battlesMock = BattlesFixture.GetBattlesMock().ToArray();
        var monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var monsterA = monstersMock[4];
        var monsterB = monstersMock[3];

        var battle = battlesMock[0];
        battle.MonsterA = monsterA.Id;
        battle.MonsterB = monsterB.Id;

        _repository.Setup(x => x.Battles.AddAsync(battle));

        _repository.Setup(x => x.Monsters.FindAsync(monsterA.Id)).ReturnsAsync(monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(monsterB.Id)).ReturnsAsync(monsterB);

        var result = await sut.Add(battle);

        OkObjectResult objectResults = (OkObjectResult)result;

        Assert.Equal("Monster B winner.", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirDefensesSame_And_MonsterA_Has_Higher_Speed()
    {
        var sut = new BattleController(_repository.Object);

        var battlesMock = BattlesFixture.GetBattlesMock().ToArray();
        var monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var monsterA = monstersMock[0];
        var monsterB = monstersMock[2];

        var battle = battlesMock[0];
        battle.MonsterA = monsterA.Id;
        battle.MonsterB = monsterB.Id;

        _repository.Setup(x => x.Battles.AddAsync(battle));

        _repository.Setup(x => x.Monsters.FindAsync(monsterA.Id)).ReturnsAsync(monsterA);
        _repository.Setup(x => x.Monsters.FindAsync(monsterB.Id)).ReturnsAsync(monsterB);

        var result = await sut.Add(battle);

        OkObjectResult objectResults = (OkObjectResult)result;

        Assert.Equal("Monster A winner.", objectResults.Value);

    }

    [Fact]
    public async Task Delete_OnSuccess_RemoveBattle()
    {
        const int id = 1;
        Battle[] battles = BattlesFixture.GetBattlesMock().ToArray();

        this._repository
            .Setup(x => x.Battles.FindAsync(id))
            .ReturnsAsync(battles[0]);

        this._repository
           .Setup(x => x.Battles.RemoveAsync(id));

        BattleController sut = new BattleController(this._repository.Object);

        ActionResult result = await sut.Remove(id);
        OkResult objectResults = (OkResult)result;
        objectResults.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Delete_OnNoBattleFound_Returns404()
    {
        const int id = 20584;

        this._repository
            .Setup(x => x.Battles.FindAsync(id))
            .ReturnsAsync(() => null);

        this._repository
           .Setup(x => x.Battles.RemoveAsync(id));

        BattleController sut = new BattleController(this._repository.Object);

        ActionResult result = await sut.Remove(id);
        NotFoundObjectResult objectResults = (NotFoundObjectResult)result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The Battle with ID = {id} not found.", objectResults.Value);
    }
}
