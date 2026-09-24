using Listen2Me.MVVM.Messages.Queuing;

namespace Listen2Me.Tests.Messages.Queuing;

[TestClass]
public class MessageQueueTests
{
    [TestMethod]
    public void Enqueue_AddsMessage()
    {
        var sut = new MessageQueue();
        sut.Enqueue(new object());

        var queue = sut.GetQueue();
        
        Assert.HasCount(1, queue);
    }
    
    [TestMethod]
    public void Enqueue_CanAddMixedTypes()
    {
        var sut = new MessageQueue();
        sut.Enqueue(new object());
        sut.Enqueue("");
        sut.Enqueue(new Stack<int>());

        var queue = sut.GetQueue();
        
        Assert.HasCount(3, queue);
        Assert.IsInstanceOfType<object>(queue[0]);
        Assert.IsInstanceOfType<string>(queue[1]);
        Assert.IsInstanceOfType<Stack<int>>(queue[2]);
    }

    [TestMethod]
    public void Dequeue_RemovesItemFromQueue()
    {
        var sut = new MessageQueue();
        sut.Enqueue(new object());
        var returned = sut.Dequeue<object>();
        
        var queue = sut.GetQueue();
        
        Assert.IsEmpty(queue);   
        Assert.IsNotNull(returned);
    }
    
    [TestMethod]
    public void Dequeue_ReturnsNullIfQueueIsEmpty()
    {
        var sut = new MessageQueue();
        var returned = sut.Dequeue<object>();
        
        Assert.IsNull(returned);
    }
    
    [TestMethod]
    public void Dequeue_ReturnsNullIfMessageIsNotOfType()
    {
        var sut = new MessageQueue();
        sut.Enqueue(new object());
        var returned = sut.Dequeue<string>();
        
        Assert.IsNull(returned);
    }
    
    [TestMethod]
    public void Dequeue_ReturnsFirstMessageOfType()
    {
        var sut = new MessageQueue();
        sut.Enqueue("m1");
        sut.Enqueue("m2");
        sut.Enqueue("m3");
        var returned = sut.Dequeue<object>();
        
        Assert.IsNotNull(returned);
        Assert.IsInstanceOfType<string>(returned);
        Assert.AreEqual("m1", returned);
        Assert.HasCount(2, sut.GetQueue());
        Assert.AreEqual("m2", sut.Dequeue<object>());
        Assert.AreEqual("m3", sut.Dequeue<object>());
    }

    [TestMethod]
    public void Dequeue_ReturnsFirstMessageOfType_FromMixedTypes()
    {
        var sut = new MessageQueue();
        sut.Enqueue("m1");
        sut.Enqueue(new object());
        sut.Enqueue(new Stack<int>());
        var returned = sut.Dequeue<Stack<int>>();
        
        Assert.IsNotNull(returned);
        Assert.IsInstanceOfType<Stack<int>>(returned);
        Assert.HasCount(2, sut.GetQueue());
    }
}