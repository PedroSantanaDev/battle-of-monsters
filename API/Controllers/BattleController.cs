using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class BattleController : BaseApiController
{
    private readonly IBattleOfMonstersRepository _repository;

    public BattleController(IBattleOfMonstersRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<Battle> battles = await _repository.Battles.GetAllAsync();
        return Ok(battles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Add([FromBody] Battle battle)
    {
        if (battle.MonsterA == null || battle.MonsterB == null)
        {
            return BadRequest("Missing ID");
        }

        await _repository.Battles.AddAsync(battle);

        var monsterA = await _repository.Monsters.FindAsync(battle.MonsterA);
        var monsterB = await _repository.Monsters.FindAsync(battle.MonsterB);

        if (monsterA == null || monsterB == null)
        {
            return BadRequest("Two monster are required to start a battle");
        }

        var monsterAWins = await ComputeBattleOutcome(battle);

        if (monsterAWins)
        {
            return Ok("Monster A winner.");
        }
        else
        {
            return Ok("Monster B winner.");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Remove(int id)
    {
        var battle = await _repository.Battles.FindAsync(id);

        if (battle == null)
        {
            return NotFound($"The Battle with ID = {id} not found.");
        }

        await _repository.Battles.RemoveAsync(id);
        await _repository.Save();
        return Ok();
    }


    private async Task<bool> ComputeBattleOutcome(Battle battle)
    {
        var monsterA = await _repository.Monsters.FindAsync(battle.MonsterA);
        var monsterB = await _repository.Monsters.FindAsync(battle.MonsterB);

        var speedA = monsterA.Speed;
        var speedB = monsterB.Speed;

        // Determine which monster attacks first based on speed and attack
        Monster attackingMonster;
        Monster defendingMonster;

        if (speedA > speedB || (speedA == speedB && monsterA.Attack > monsterB.Attack))
        {
            attackingMonster = monsterA;
            defendingMonster = monsterB;
        }
        else
        {
            attackingMonster = monsterB;
            defendingMonster = monsterA;
        }


        // Battle until one monster wins
        while (monsterA.Hp > 0 && monsterB.Hp > 0)
        {
            // Calculate damage and update HP
            var damage = Math.Max(1, attackingMonster.Attack - defendingMonster.Defense);
            defendingMonster.Hp -= damage;

            // Swap roles for the next turn
            if (attackingMonster == monsterA)
            {
                attackingMonster = monsterB;
                defendingMonster = monsterA;
            }
            else
            {
                attackingMonster = monsterA;
                defendingMonster = monsterB;
            }
        }

        // Determine the winner
        var winner = monsterA.Hp <= 0 ? monsterB : monsterA;

        // Check if monsterA is winner
        return winner == monsterA;
    }
}


