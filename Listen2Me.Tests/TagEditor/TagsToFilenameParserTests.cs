using Listen2Me.MVVM.TagEditor;

namespace Listen2Me.Tests.TagEditor;

[ TestClass]
public class TagsToFilenameParserTests
{
    ITagsToFilenameParser _sut;
    
    [TestInitialize]
    public void Setup()
    {
        _sut = new TagsToFilenameParser(new MaskParser());
    }
    
    [TestMethod]
    public void ValidTagsWithValidFormula_ReturnsFilename()
    {
        var formula = "%artist% - %title%";
        var tags = new Dictionary<string, string> {{"artist", "Wubbaduck"}, {"title", "Danger"}};
        var expected = "Wubbaduck - Danger";
        
        var actual = _sut.Generate(tags, formula);
        
        Assert.AreEqual(expected, actual);
    }
}