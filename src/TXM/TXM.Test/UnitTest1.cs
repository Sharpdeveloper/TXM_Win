namespace TXM.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var p = new Player("TKundNobody");
        p.Firstname = "Martin";
        Assert.Equal("Martin \"TKundNobody\"", p.DisplayName);
    }
}