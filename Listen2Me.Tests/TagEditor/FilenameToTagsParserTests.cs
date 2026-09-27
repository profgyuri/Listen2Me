using Listen2Me.MVVM.TagEditor;

namespace Listen2Me.Tests.TagEditor;

[TestClass]
public class FilenameToTagsParserTests
{
    IFilenameToTagsParser _sut;

    [TestInitialize]
    public void Setup()
    {
        _sut = new FilenameToTagsParser(new MaskParser(), new MaskMatcher());
    }
    
    [TestMethod]
    public void FilenameToTagsParser_GetsCorrectTags()
    {
        var formula = "%artist% - %title%";
        var filename = "Wubbaduck - Danger";

        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNotNull(dictionary);
        Assert.AreEqual("Wubbaduck", dictionary["artist"]);
        Assert.AreEqual("Danger", dictionary["title"]);
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsNull_IfRegexDoesNotMatch()
    {
        var formula = "%artist% + %title%";
        var filename = "Wubbaduck - Danger";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNull(dictionary);
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsEmpty_IfMoreFormulaIsPresentThanNecessary()
    {
        var formula = "%artist% - %title%";
        var filename = "Wubbaduck";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNull(dictionary);
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsEmpty_IfFormulaIsMissing()
    {
        var formula = "";
        var filename = "Wubbaduck - Danger";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNull(dictionary);
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsEmpty_IfFormulaHasNoDivider()
    {
        var formula = "%artist%%title%";
        var filename = "Wubbaduck - Danger";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNull(dictionary);   
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsEmpty_IfFormulaIsInvalid()
    {
        var formula = "%artist";
        var filename = "Wubbaduck - Danger";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNull(dictionary);  
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsCorrectly_IfFormulaStartsWithLiteral()
    {
        var formula = "01 %artist% - %title%";
        var filename = "01 Wubbaduck - Danger";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNotNull(dictionary);
        Assert.AreEqual("Wubbaduck", dictionary["artist"]);
    }
    
    [TestMethod]
    public void FilenameToTagsParser_ReturnsCorrectly_IfFormulaEndsWithLiteral()
    {
        var formula = "%artist% - %title% 01";
        var filename = "Wubbaduck - Danger 01";
        
        var dictionary = _sut.Parse(filename, formula);
        
        Assert.IsNotNull(dictionary);
        Assert.AreEqual("Danger", dictionary["title"]);   
    }
}