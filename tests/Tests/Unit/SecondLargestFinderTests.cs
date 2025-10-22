namespace VivaAssessment.Tests.Unit;

public static class SecondLargestFinder
{
    public static int Find(IEnumerable<int> input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        var distinct = input.Distinct().OrderByDescending(x => x).ToList();
        if (distinct.Count < 2) throw new InvalidOperationException("Need at least two distinct values.");
        return distinct[1];
    }
}

public class SecondLargestFinderTests
{
    [Theory]
    [InlineData(new[] { 1, 5, 3, 5, 2 }, 3)]
    [InlineData(new[] { 10, 10, 9 }, 9)]
    [InlineData(new[] { -10, 0, -1, 0, 2 }, 0)]
    [InlineData(new[] { int.MinValue, int.MaxValue }, int.MinValue)]
    public void Find_ReturnsSecondLargest_WhenValid(int[] input, int expected)
        => Assert.Equal(expected, SecondLargestFinder.Find(input));

    [Theory]
    [InlineData(new[] { 42 })]
    [InlineData(new[] { 7, 7, 7 })]
    [InlineData(new int[] { })]
    public void ThrowsOnInsufficientDistincts(int[] input)
        => Assert.ThrowsAny<Exception>(() => SecondLargestFinder.Find(input));
}