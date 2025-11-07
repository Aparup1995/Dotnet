using FluentAssertions;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;
//AssesmentTest t = new AssesmentTest();

//var res = t.GetData("",null,null,null,null,1,20);
public class AssesmentTest
{
    private IEnumerable<Item> Source1 { get; set; }
    private IEnumerable<Item> Source2 { get; set; }
    // do not change
    public AssesmentTest()
    {
        Source1 = JsonSerializer.Deserialize<IEnumerable<Item>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "source1.json")))!;
        Source2 = JsonSerializer.Deserialize<IEnumerable<Item>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "source2.json")))!;
    }

    /// <summary>
    /// This method should fetch data from Source1 and Source2 and filter by the parameters
    /// The response from this method should be a paginated set of data based on the page (0 based index) pageSize and orderBy
    /// 
    /// Please do implement it.
    /// 
    /// </summary>
    /// <param name="name">if provided, name filter should work partially and should be case insensitive</param>
    /// <param name="from">if provided, only results born after that date (including time) should be returned</param>
    /// <param name="to">if provided, only results born until that date (including time) should be returned</param>
    /// <param name="source">if provided, only results from that source should be returned</param>
    /// <param name="orderBy">the column in which the results should be sumeorted by, sorting should always be lesser to greater (an ascending order) and on source it should go OriginA before OriginB</param>
    /// <param name="page">the page you want to return</param>
    /// <param name="pageSize">number of items per page</param>
    /// <returns></returns>
   public IEnumerable<Item> GetData(string name, DateTime? from, DateTime? to, Source? source, string orderBy, int page = 0, int pageSize = 20)
    {
        return null;
    }
}


public enum Source
{
    OriginA = 0,
    OriginB = 1,
}

public class Item()
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public Source Source { get; set; }
}

public class TestFixture
{
    public TestFixture()
    {
        AssesmentTest = new();
    }

    public AssesmentTest AssesmentTest { get; set; }
}

public class PublicTests : IClassFixture<TestFixture>
{
    public AssesmentTest Assesment { get; }

    public PublicTests(TestFixture fixture)
    {
        this.Assesment = fixture.AssesmentTest;
    }

    [Fact]
    public void ReturnsData()
    {
        var data = Assesment.GetData(null, null, null, null, null);
        data.Should().NotBeEmpty();
    }

    [Fact]
    public void FiltersByName()
    {
        var data = Assesment.GetData("Abraham", null, null, null, null);
        data.Should()
            .HaveCount(2);
    }

    [Theory]
    [InlineData(2005, 01, 01)]
    [InlineData(2000, 01, 01)]
    [InlineData(1994, 01, 01)]
    public void FilterOlderThan(int year, int month, int day)
    {
        var date = new DateTime(year, month, day);
        var data = Assesment.GetData(null, date, null, null, null);
        data.Should()
            .AllSatisfy(d => d.BirthDate.Should().BeAfter(date));
    }

    [Theory]
    [InlineData(2005, 01, 01)]
    [InlineData(2000, 01, 01)]
    [InlineData(1994, 01, 01)]
    public void FilterYoungerThan(int year, int month, int day)
    {
        var date = new DateTime(year, month, day);
        var data = Assesment.GetData(null, null, date, null, null);
        data.Should()
            .AllSatisfy(d => d.BirthDate.Should().BeBefore(date));
    }

    [Theory]
    [InlineData(Source.OriginA)]
    [InlineData(Source.OriginB)]
    public void FilterBySource(Source source)
    {
        var data = Assesment.GetData(null, null, null, source, null);
        data.Should()
            .AllSatisfy(d => d.Source.Should().Be(source));
    }


    [Theory]
    [InlineData(nameof(Item.Id))]
    [InlineData(nameof(Item.Name))]
    [InlineData(nameof(Item.Source))]
    [InlineData(nameof(Item.BirthDate))]
    [InlineData("bIrThDatE")]
    public void SortByCorrectly(string column)
    {
        Func<Item, Item, int> comparison = column.ToLower() switch
        {
            "id" => (a, b) => Comparer<int>.Default.Compare(a.Id, b.Id),
            "name" => (a, b) => Comparer<string>.Default.Compare(a.Name, b.Name),
            "source" => (a, b) => Comparer<int>.Default.Compare((int)a.Source, (int)b.Source),
            "birthDate" => (a, b) => Comparer<DateTime>.Default.Compare(a.BirthDate, b.BirthDate),
            _ => (a, b) => Comparer<int>.Default.Compare(a.Id, b.Id),
        };

        var data = Assesment.GetData(null, null, null, null, column);
        data.Should().HaveCountGreaterThanOrEqualTo(2);
        data.Should()
            .BeInAscendingOrder(comparison);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(66)]
    public void PageWorksCorrectly(int pageSize)
    {
        var data = Assesment.GetData(null, null, null, null, null, 0, pageSize);
        data.Should().HaveCount(pageSize);
    }
}