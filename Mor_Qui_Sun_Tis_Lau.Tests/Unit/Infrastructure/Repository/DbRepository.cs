using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Infrastructure.Repository;

public class DbRepositoryTests
{
    /*
        We are testing the class Order since it inherits from BaseEntity and contains a list.
        The list is required to test the include methods in the generic repository

        Every class except user and admin inherits from BaseEntity, so in theory should every repository which implements
        the generic repository work correctly
    */

    private readonly DbRepository<Order> _dbRepository;

    public DbRepositoryTests()
    {
        _dbRepository = new(DbTest.CreateContext());
    }

    /* 
        GetByIdAsync method is tested regularly, since its a central method and
        to confirm that the entity is modified or added correctly to the database

        The AddAsync method is also regularly tested since data in the database is
        required to test the different methods
    */

    [Fact]
    public async Task AddAsync_And_GetByIdAsync()
    {
        var testEntity = await AddEmptyEntity();
        var testEntityInDb = await GetTestEntity(testEntity.Id);

        Assert.NotNull(testEntityInDb);
        Assert.Equal(testEntity, testEntityInDb);
    }

    [Fact]
    public async Task Remove_AnyAsync_And_All()
    {
        var testEntity = await AddEmptyEntity();

        Assert.NotEmpty(_dbRepository.All());

        await _dbRepository.Remove(testEntity.Id);

        Assert.False(await _dbRepository.AnyAsync());
        Assert.Empty(_dbRepository.All());
    }

    [Fact]
    public async Task Update()
    {
        var testEntity = await AddEmptyEntity();
        var tip = 20m;

        testEntity.SetTip(tip);

        await _dbRepository.Update(testEntity);

        var testEntityInDb = await GetTestEntity(testEntity.Id);

        Assert.NotNull(testEntityInDb);
        Assert.Equal(testEntity.Tip, testEntityInDb.Tip);
    }

    [Fact]
    public async Task UpdateRange()
    {
        var testEntity = await AddEmptyEntity();
        var testEntity1 = await AddEmptyEntity();

        List<Order> testEntities = [testEntity, testEntity1];

        var tip = 20m;
        testEntity.SetTip(tip);
        testEntity1.SetTip(tip);

        await _dbRepository.UpdateRange(testEntities);

        var testEntityInDb = await GetTestEntity(testEntity.Id);
        var testEntity1InDb = await GetTestEntity(testEntity1.Id);

        Assert.NotNull(testEntityInDb);
        Assert.NotNull(testEntity1InDb);

        Assert.Equal(testEntity.Tip, testEntityInDb.Tip);
        Assert.Equal(testEntity1.Tip, testEntity1InDb.Tip);
    }

    [Fact]
    public async Task All_And_AnyAsync()
    {
        var (entitiesCount, testEntities) = await AddMultipleEmptyEntities();

        var entitiesInDb = _dbRepository.All();

        Assert.Equal(testEntities, entitiesInDb);
        Assert.True(await _dbRepository.AnyAsync());
        Assert.Equal(entitiesCount, entitiesInDb.Count);
    }

    [Fact]
    public async Task IncludeToListAsync()
    {
        var testEntity = await AddEntityWithAEntryInList();

        var testEntitiesInDb = await _dbRepository.IncludeToListAsync(t => t.OrderLines);

        Assert.Equal([testEntity], testEntitiesInDb);
        Assert.NotEmpty(testEntitiesInDb[0].OrderLines);
    }

    [Fact]
    public async Task WhereFirstOrDefaultAsync()
    {
        var testEntity = await AddEmptyEntity();

        // Add decoy 
        await AddEmptyEntity();

        var testEntityInDb = await _dbRepository.WhereFirstOrDefaultAsync(t => t.Id == testEntity.Id);

        Assert.NotNull(testEntityInDb);
        Assert.Equal(testEntity, testEntityInDb);
    }

    [Fact]
    public async Task IncludeWhereFirstOrDefaultAsync()
    {
        var testEntity = await AddEntityWithAEntryInList();

        var testEntityInDb = await _dbRepository.IncludeWhereFirstOrDefaultAsync(t => t.OrderLines, t => t.Id == testEntity.Id);

        // Add decoy 
        await AddEmptyEntity();

        Assert.NotNull(testEntityInDb);
        Assert.Equal(testEntity, testEntityInDb);
        Assert.NotEmpty(testEntityInDb.OrderLines);
    }

    [Fact]
    public async Task IncludeOrderByToListAsync()
    {
        var (entitiesCount, testEntities) = await AddMultipleEntitiesWithAEntryInList();

        var testEntitiesInDb = await _dbRepository.IncludeOrderByToListAsync(t => t.OrderLines, t => t.CreatedAt);

        Assert.Equal(testEntities, testEntitiesInDb);
        Assert.NotNull(testEntitiesInDb[0].OrderLines);
        Assert.Equal(entitiesCount, testEntitiesInDb.Count);
    }

    [Fact]
    public async Task IncludeWhereToListAsync()
    {
        var testEntity = await AddEntityWithAEntryInList();

        // Add decoy 
        await AddEmptyEntity();

        var testEntityInDb = await _dbRepository.IncludeWhereToListAsync(t => t.OrderLines, t => t.OrderLines.Count() == 1);

        Assert.Equal([testEntity], testEntityInDb);
        Assert.NotEmpty(testEntityInDb[0].OrderLines);
    }

    [Fact]
    public async Task WhereOrderByToListAsync()
    {
        var (entitiesCount, testEntities) = await AddMultipleEmptyEntities();

        // Add decoy 
        await AddEntityWithAEntryInList();

        var testEntitiesInDb = await _dbRepository.WhereOrderByToListAsync(t => t.OrderLines.Count() == 0, t => t.CreatedAt);

        Assert.Equal(testEntities, testEntitiesInDb);
        Assert.Empty(testEntitiesInDb[0].OrderLines);
        Assert.Equal(entitiesCount, testEntitiesInDb.Count);
    }

    [Fact]
    public async Task IncludeWhereOrderByToListAsync()
    {
        var (entitiesCount, testEntities) = await AddMultipleEntitiesWithAEntryInList();

        // Add decoy 
        await AddEmptyEntity();

        var testEntitiesInDb = await _dbRepository.IncludeWhereOrderByToListAsync(t => t.OrderLines, t => t.OrderLines.Count() == 1, t => t.CreatedAt);

        Assert.Equal(testEntities, testEntitiesInDb);
        Assert.NotEmpty(testEntitiesInDb[0].OrderLines);
        Assert.Equal(entitiesCount, testEntitiesInDb.Count);
    }


    private async Task<Order?> GetTestEntity(Guid testEntityId)
    {
        return await _dbRepository.GetByIdAsync(testEntityId);
    }

    private static Order CreateTestEntity()
    {
        return new Order(Guid.NewGuid());
    }

    private async Task<Order> AddEmptyEntity()
    {
        var testEntity = CreateTestEntity();

        await _dbRepository.AddAsync(testEntity);

        return testEntity;
    }

    private async Task<(int, List<Order>)> AddMultipleEmptyEntities()
    {
        var entitiesCount = 3;
        List<Order> testEntities = [];

        for (var i = 0; i < entitiesCount; i++)
        {
            testEntities.Add(await AddEmptyEntity());
        }

        return (entitiesCount, testEntities);
    }

    private async Task<Order> AddEntityWithAEntryInList()
    {
        var testEntity = CreateTestEntity();

        testEntity.AddOrderLine("orderName", 20, 10, "stripeProductId");

        await _dbRepository.AddAsync(testEntity);

        return testEntity;
    }

    private async Task<(int, List<Order>)> AddMultipleEntitiesWithAEntryInList()
    {
        var entitiesCount = 3;
        List<Order> testEntities = [];

        for (var i = 0; i < entitiesCount; i++)
        {
            testEntities.Add(await AddEntityWithAEntryInList());
        }

        return (entitiesCount, testEntities);
    }
}




